using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;

using VRCEntryBoard.App.Services;

namespace VRCEntryBoard.Infra.VRChat
{
    internal sealed class CVRCDataLoderInLogfile : IVRCDataLoder
    {
        /// <summary>最後のファイル長さ</summary>
        private long _latesFileLength = 0;
        /// <summary>最後のファイル名</summary>
        private string _latesFileName = string.Empty;

        private string _JoinKeyword = "[Behaviour] OnPlayerJoined";
        private string _LeftKeyword = "Removed player";
        private string _EnteringKeyword = "Entering Room:";

        private string _logFolderPath { get { return new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)).FullName + @"Low\VRChat\VRChat"; } }
        private string _logFilePattern = "output_log_*.txt";

        private readonly ILogger<CVRCDataLoderInLogfile> _logger;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CVRCDataLoderInLogfile()
        {
            _logger = VRCEntryBoard.Infra.Logger.LoggerExtensions.GetLogger<CVRCDataLoderInLogfile>();

            try
            {
                // ログファイルパスの取得
                string[] logFilePaths = Directory.GetFiles(_logFolderPath, _logFilePattern, SearchOption.TopDirectoryOnly);                
                if (!logFilePaths.Any())
                {
                    _logger.LogWarning("VRChatのログファイルが見つかりません。");
                    return;
                }
                // 最後のログファイルパスを取得.
                _latesFileName = logFilePaths.OrderByDescending(file => new FileInfo(file).LastWriteTime).First();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError($"ログファイルへのアクセスが拒否されました: {ex.Message}");
                throw;
            }
            catch (DirectoryNotFoundException)
            {
                _logger.LogError("VRChatのログディレクトリが見つかりません。");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ログファイルの初期化中にエラーが発生しました");
                throw;
            }
        }

        /// <summary>
        /// ログファイルからデータを取得
        /// </summary>
        public List<VRChatLogFileModel> GetVRCData()
        {
            try
            {
                List<VRChatLogFileModel> logFileModels = new List<VRChatLogFileModel>();
                logFileModels.AddRange(AnalysisFile(_latesFileName));

                // 一番新しいログファイルパスを取得.
                string[] logFilePaths = Directory.GetFiles(_logFolderPath, _logFilePattern, SearchOption.TopDirectoryOnly);

                if (!logFilePaths.Any())
                {
                    _logger.LogWarning("有効なログファイルが見つかりませんでした。");
                    return null;
                }

                string latestLogFilePath = logFilePaths.OrderByDescending(file => new FileInfo(file).LastWriteTime).First();

                // より新しいファイルが無ければ終了.
                if (latestLogFilePath == _latesFileName) return logFileModels;

                // 読み込み中のファイル情報をクリアし再読み込み.
                _latesFileName = latestLogFilePath;
                _latesFileLength = 0;
                logFileModels.AddRange(AnalysisFile(_latesFileName));

                return logFileModels;
            }
            catch (Exception ex)
            {
                _logger.LogError($"プレイヤーリストの更新中にエラーが発生しました: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// ログファイルの解析
        /// </summary>
        /// <param name="fileName">ログファイル名</param>
        private List<VRChatLogFileModel> AnalysisFile(string fileName)
        {
            try
            {
                List<VRChatLogFileModel> logFileModels = new List<VRChatLogFileModel>();

                using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    // ファイルサイズが増えていなければスキップ.
                    if (fs.Length < _latesFileLength) return logFileModels;

                    byte[] bs = new byte[fs.Length - _latesFileLength];
                    fs.Seek(_latesFileLength, SeekOrigin.Begin);
                    fs.Read(bs, 0, bs.Length);

                    var reader = new StreamReader(new MemoryStream(bs), Encoding.UTF8);
                    while (-1 != reader.Peek())
                    {
                        string line = reader.ReadLine() ?? string.Empty;

                        try
                        {
                            // プレイヤーがJoinした場合.
                            if (line.Contains(_JoinKeyword))
                            {
                                int joiinKeyLastIndex = line.IndexOf(_JoinKeyword) + this._JoinKeyword.Length;
                                int IDKeyIndex = line.LastIndexOf("(");
                                if (-1 == IDKeyIndex)
                                {
                                    _logger.LogWarning($"不正なJoinログ形式: {line}");
                                    continue;
                                }
                                string playerName = line.Substring(joiinKeyLastIndex, IDKeyIndex - joiinKeyLastIndex).Trim();
                                VRChatLogFileModel logFileModel = new VRChatLogFileModel();
                                logFileModel.EventType = EventType.PlayerJoin;
                                logFileModel.PlayerName = playerName;
                                logFileModels.Add(logFileModel);
                            }
                            // プレイヤーがLeftした場合.
                            else if (line.Contains(_LeftKeyword))
                            {
                                int index = line.IndexOf(_LeftKeyword);
                                string playerName = line.Substring(index + _LeftKeyword.Length).Trim();
                                VRChatLogFileModel logFileModel = new VRChatLogFileModel();
                                logFileModel.EventType = EventType.PlayerLeft;
                                logFileModel.PlayerName = playerName;
                                logFileModels.Add(logFileModel);
                            }
                            // 自分がワールド移動した場合.
                            else if (line.Contains(_EnteringKeyword))
                            {
                                int index = line.IndexOf(_EnteringKeyword);
                                string worldName = line.Substring(index + _EnteringKeyword.Length).Trim();
                                VRChatLogFileModel logFileModel = new VRChatLogFileModel();
                                logFileModel.EventType = EventType.WorldMove;
                                logFileModel.WorldName = worldName;
                                logFileModels.Add(logFileModel);
                            }
                        }
                        catch (Exception lineEx)
                        {
                            _logger.LogError($"ログ行の処理中にエラー: {lineEx.Message}, ログ行: {line}");
                        }
                    }

                    // 今回の読み取り場所を記録.
                    this._latesFileLength = fs.Length;
                }

                return logFileModels;
            }
            catch (IOException ex)
            {
                _logger.LogError($"ファイル読み取り中にIOエラーが発生: {ex.Message}");
                throw;
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError($"ファイルへのアクセスが拒否されました: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"ファイル解析中に予期せぬエラーが発生: {ex.Message}");
                throw;
            }
        }
    }
}
