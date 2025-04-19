using System.Threading.Tasks;
using Supabase;
using System;
using System.Text.Json;
using System.IO;
using Microsoft.Extensions.Logging;

using VRCEntryBoard.Infra.Logger;
using VRCEntryBoard.HMI.Exception;
using VRCEntryBoard.Domain.Model;
using VRCEntryBoard.Domain.Exceptions;

namespace VRCEntryBoard.Infra
{
    internal class SupabaseClient
    {
        private readonly Supabase.Client _client;
        private readonly VRCEntryBoardConfig _config;
        private readonly string _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VRCEntryBoard-config.json");
        private readonly ILogger<SupabaseClient> _logger;

        public SupabaseClient(IExceptionNotifier exceptionNotifier)
        {
            _logger = LogManager.GetLogger<SupabaseClient>();
            
            try
            {
                _config = LoadConfig();
                var options = new SupabaseOptions
                {
                    AutoConnectRealtime = true,
                };
                
                // 接続テスト - オンラインかどうか確認
                _client = new Supabase.Client(_config.Url, _config.Key, options);
            }
            catch
            {
                // 例外をリスローして、ファクトリに伝える
                throw;
            }
        }

        /// <summary>
        /// クライアントを取得する
        /// </summary>
        public Supabase.Client GetClient()
        {
            return _client;
        }

        /// <summary>
        /// 設定ファイルの読み込み
        /// </summary>
        private VRCEntryBoardConfig LoadConfig()
        {
            try
            {
                if (!File.Exists(_configPath))
                {
                    throw new VRCApplicationException(
                        "設定ファイルエラー", 
                        "設定ファイルが見つかりません", 
                        isFatal: true);
                }

                string jsonContent = File.ReadAllText(_configPath);
                var config = JsonSerializer.Deserialize<VRCEntryBoardConfig>(jsonContent);
                
                if (config == null)
                {
                    throw new VRCApplicationException(
                        "設定ファイルエラー", 
                        "設定ファイルの読み込みに失敗しました。ファイル形式を確認してください。", 
                        isFatal: true);
                }

                // 設定の検証
                if (string.IsNullOrEmpty(config.Url) || string.IsNullOrEmpty(config.Key))
                {
                    throw new VRCApplicationException(
                        "設定ファイルエラー", 
                        "設定が不完全です: URLまたはKeyが設定されていません。設定ファイルを確認してください。", 
                        isFatal: true);
                }

                return config;
            }
            catch (JsonException ex)
            {
                throw new VRCApplicationException(
                    "設定ファイルエラー", 
                    "設定ファイルのJSONフォーマットが不正です。設定ファイルを確認してください。", 
                    isFatal: true, 
                    innerException: ex);
            }
            catch (Exception ex) when (ex is not VRCApplicationException && ex is not FileNotFoundException && ex is not InvalidOperationException)
            {
                throw new VRCApplicationException(
                    "設定ファイルエラー", 
                    "設定ファイルの読み込み中に予期せぬエラーが発生しました。", 
                    isFatal: true, 
                    innerException: ex);
            }
        }

        /// <summary>
        /// 非同期初期化
        /// </summary>
        private async Task Init()
        {
            try
            {
                await _client.InitializeAsync();
                _logger.LogInformation("Supabaseクライアントが非同期初期化されました");
            }
            catch (Exception ex)
            {
                throw new VRCApplicationException(
                    "データベース初期化エラー", 
                    "クライアントの初期化中にエラーが発生しました。ネットワーク接続を確認してください。", 
                    isFatal: true, 
                    innerException: ex);
            }
        }
    }
}
