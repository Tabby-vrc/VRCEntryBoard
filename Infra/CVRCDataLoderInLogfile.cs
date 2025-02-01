using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VRCEntryBoard.Domain.Interfaces;
using VRCEntryBoard.Domain.Model;

namespace VRCEntryBoard.Infra
{
    internal class CVRCDataLoderInLogfile : IVRCDataLoder
    {
        /// <summary>最後のファイル長さ</summary>
        private long m_latesFileLength = 0;
        /// <summary>最後のファイル名</summary>
        private string m_latesFileName = string.Empty;

        /// <summary>プレイヤーリスト</summary>
        private List<Player> _joiningPlayerNameList = new List<Player>();

        private string _JoinKeyword = "[Behaviour] OnPlayerJoined";
        private string _LeftKeyword = "Removed player";
        private string _EnteringKeyword = "Finished entering world.";

        private string _logFolderPath { get { return new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)).FullName + @"Low\VRChat\VRChat"; } }
        private string _logFilePattern = "output_log_*.txt";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CVRCDataLoderInLogfile()
        {
            // 一番新しいログファイルパスを取得.
            string[] logFilePaths = Directory.GetFiles(this._logFolderPath, this._logFilePattern, SearchOption.TopDirectoryOnly);
            this.m_latesFileName = logFilePaths.OrderByDescending(file => new FileInfo(file).LastWriteTime).First();
        }

        /// <summary>
        /// プレイヤーリスト取得
        /// </summary>
        /// <returns>プレイヤーリスト</returns>
        public List<Player> GetPlayerList()
        {
            return this._joiningPlayerNameList;
        }

        /// <summary>
        /// プレイヤーリスト更新
        /// </summary>
        public void UpdatePlayerList()
        {
            this.AnalysisFile(this.m_latesFileName);

            // 一番新しいログファイルパスを取得.
            string[] logFilePaths = Directory.GetFiles(this._logFolderPath, this._logFilePattern, SearchOption.TopDirectoryOnly);
            string latestLogFilePath = logFilePaths.OrderByDescending(file => new FileInfo(file).LastWriteTime).First();

            // より新しいファイルが無ければ終了.
            if (latestLogFilePath == this.m_latesFileName) return;

            // 読み込み中のファイル情報をクリアし再読み込み.
            this.m_latesFileName = latestLogFilePath;
            this.m_latesFileLength = 0;
            this.AnalysisFile(this.m_latesFileName);
        }

        private void AnalysisFile(string fileName)
        {
            // ファイル解析開始.
            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                // ファイルサイズが増えていなければ場合のみスキップ.
                if (fs.Length < this.m_latesFileLength) return;

                byte[] bs = new byte[fs.Length - this.m_latesFileLength];
                fs.Seek(this.m_latesFileLength, SeekOrigin.Begin);

                // テキストエンコーディングにUTF-8を用いてstreamの読み込みを行うStreamReaderを作成する.
                var reader = new StreamReader(fs, Encoding.UTF8);
                while (-1 != reader.Peek())
                {
                    string line = reader.ReadLine() ?? string.Empty;
                    // プレイヤーがJoinした場合.
                    if (line.Contains(_JoinKeyword))
                    {
                        // Joinプレイヤーをリストに追加.
                        int joiinKeyLastIndex = line.IndexOf(_JoinKeyword) + this._JoinKeyword.Length;
                        int IDKeyIndex = line.LastIndexOf("(");
                        string playerName = line.Substring(joiinKeyLastIndex, IDKeyIndex - joiinKeyLastIndex).Trim();

                        var joinPayer = this._joiningPlayerNameList.FirstOrDefault(player => player.Name == playerName);

                        if (null != joinPayer)
                        {
                            // 離席プレイヤーの復帰.
                            joinPayer.JoinStatus = true;
                        }
                        else
                        {
                            Player addPlayer = new Player(playerName);
                            this._joiningPlayerNameList.Add(addPlayer);
                        }
                    }
                    // プレイヤーがLeftした場合.
                    else if (line.Contains(_LeftKeyword))
                    {
                        // Leftプレイヤーをリストから削除.
                        int index = line.IndexOf(_LeftKeyword);
                        string playerName = line.Substring(index + _LeftKeyword.Length).Trim();

                        if (!this._joiningPlayerNameList.Where(player => player.Name == playerName).Any()) continue;
                        var leftPayer = this._joiningPlayerNameList.First(player => player.Name == playerName);

                        if (emEntryStatus.Entry == leftPayer.EntryStatus ||
                            true == leftPayer.StaffStatus)
                        {
                            // Entry済み又はスタッフプレイヤーの離席はリスト除外しない.
                            leftPayer.JoinStatus = false;
                        }
                        else
                        {
                            this._joiningPlayerNameList.Remove(leftPayer);
                        }
                    }
                    // 自分がワールド移動した場合.
                    else if (line.Contains(_EnteringKeyword))
                    {
                        // 確認済みプレイヤーのみ残してリストリセット.
                        this._joiningPlayerNameList.RemoveAll(player => player.EntryStatus == emEntryStatus.AskMe);
                        //this._joiningPlayerNameList.RemoveAll(player => player.Status != EntryStatus.AskMe||
                        //                                                player.IsJoin != false);
                        foreach (var player in this._joiningPlayerNameList)
                        {
                            player.JoinStatus = false;
                        }
                    }
                }

                // 今回の読み取り場所を記録.
                this.m_latesFileLength = fs.Length;
            }
        }
    }
}
