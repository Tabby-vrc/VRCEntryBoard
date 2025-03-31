using System.Threading.Tasks;
using Supabase;
using Supabase.Postgrest;
using Supabase.Postgrest.Models;
using TableAttribute = Supabase.Postgrest.Attributes.TableAttribute;
using System.Collections.Generic;
using System;
using Supabase.Postgrest.Attributes;
using static Supabase.Realtime.PostgresChanges.PostgresChangesOptions;
using System.Text.Json;
using System.IO;
using Microsoft.Extensions.Logging;

using VRCEntryBoard.HMI.Exception;
using VRCEntryBoard.Domain.Interfaces;
using VRCEntryBoard.Domain.Model;

namespace VRCEntryBoard.Infra.SupabaseAdapter
{
    internal class SupabaseClient : IPlayerRepository
    {
        private readonly Supabase.Client _client;
        private readonly VRCEntryBoardConfig _config;
        private readonly string _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VRCEntryBoard-config.json");
        private readonly ILogger<SupabaseClient> _logger;
        private readonly IExceptionNotifier _exceptionNotifier;
        private List<Player> _players;

        public SupabaseClient(IExceptionNotifier exceptionNotifier)
        {
            _exceptionNotifier = exceptionNotifier ?? 
                throw new ArgumentNullException(nameof(exceptionNotifier));
            _logger = VRCEntryBoard.Infra.Logger.LoggerExtensions.GetLogger<SupabaseClient>();
            _players = new List<Player>();
            _config = LoadConfig();
            try
            {
                var options = new SupabaseOptions
                {
                    AutoConnectRealtime = true,
                };
                _client = new Supabase.Client(_config.Url, _config.Key, options);
            }
            catch (Exception)
            {
                HandleError("データベース接続エラー", 
                    "データベースへの接続に失敗しました。設定ファイルを確認してください。", null, true);
                throw;
            }
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
                    HandleError("設定ファイルエラー", 
                        $"設定ファイルが見つかりません", null, true);
                    throw new FileNotFoundException("設定ファイルが見つかりません");
                }

                string jsonContent = File.ReadAllText(_configPath);
                var config = JsonSerializer.Deserialize<VRCEntryBoardConfig>(jsonContent);
                
                if (config == null)
                {
                    HandleError("設定ファイルエラー", 
                        "設定ファイルの読み込みに失敗しました。ファイル形式を確認してください。", null, true);
                    throw new InvalidOperationException("設定ファイルの読み込みに失敗しました。");
                }

                // 設定の検証
                if (string.IsNullOrEmpty(config.Url) || string.IsNullOrEmpty(config.Key))
                {
                    HandleError("設定ファイルエラー", 
                        "設定が不完全です: URLまたはKeyが設定されていません。設定ファイルを確認してください。", null, true);
                    throw new InvalidOperationException("設定が不完全です: URLまたはKeyが設定されていません");
                }

                return config;
            }
            catch (JsonException ex)
            {
                HandleError("設定ファイルエラー", 
                    "設定ファイルのJSONフォーマットが不正です。設定ファイルを確認してください。", ex, true);
                throw new InvalidOperationException("設定ファイルのJSONフォーマットが不正です", ex);
            }
            catch (Exception ex) when (ex is not FileNotFoundException && ex is not InvalidOperationException)
            {
                HandleError("設定ファイルエラー", 
                    "設定ファイルの読み込み中に予期せぬエラーが発生しました。", ex, true);
                throw new InvalidOperationException("設定ファイルの読み込みに失敗しました。", ex);
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
            catch (Exception)
            {
                HandleError("データベース初期化エラー", 
                    "クライアントの初期化中にエラーが発生しました。ネットワーク接続を確認してください。", null, true);
                throw;
            }
        }

        /// <summary>
        /// プレイヤーリスト取得
        /// </summary>
        /// <returns>プレイヤーリスト</returns>
        public List<Player> GetPlayers()
        {
            return _players;
        }

        /// <summary>
        /// プレイヤー追加
        /// </summary>
        public async Task Insert(Player player)
        {
            if (player == null)
            {
                return;
            }

            try
            {
                var getPlayerModel = await _client.From<PlayerModel>()
                    .Where(x => x.Name == player.Name)
                    .Single();

                if (null == getPlayerModel)
                {
                    var addPlayerModel = new PlayerModel
                    {
                        Name = player.Name,
                        EntryStatus = (int)player.EntryStatus,
                        StaffStatus = player.StaffStatus,
                        ExpStatus = (int)player.ExpStatus,
                        JoinStatus = player.JoinStatus
                    };

                    var insertRet = await _client.From<PlayerModel>()
                        .Insert(addPlayerModel, new QueryOptions { Returning = QueryOptions.ReturnType.Representation });

                    try
                    {
                        var deserializedList = JsonSerializer.Deserialize<List<PlayerModel>>(insertRet.Content);
                        if (deserializedList != null && deserializedList.Count > 0)
                        {
                            player.ID = deserializedList[0].ID;
                        }
                        else
                        {
                            _logger.LogWarning("プレイヤー追加時の応答をデシリアライズできませんでした");
                        }
                    }
                    catch (JsonException jsonEx)
                    {
                        _logger.LogError(jsonEx, "プレイヤー追加時のレスポンスJSONの解析に失敗しました: {Content}", insertRet.Content);
                    }
                }
                else
                {
                    player.ID = getPlayerModel.ID;
                    player.EntryStatus = (emEntryStatus)getPlayerModel.EntryStatus;
                    player.StaffStatus = getPlayerModel.StaffStatus;
                    player.ExpStatus = (emExpStatus)getPlayerModel.ExpStatus;
                    player.JoinStatus = getPlayerModel.JoinStatus;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "プレイヤー追加中に予期せぬエラーが発生しました: {PlayerName}", player.Name);
                // アプリケーションの実行継続のため例外を再スローしない
            }
        }

        /// <summary>
        /// プレイヤーリスト追加
        /// </summary>
        public async Task Insert(List<Player> players)
        {
            if (players == null || players.Count == 0)
            {
                _logger.LogWarning("プレイヤーリストが空またはnullのため、一括追加処理をスキップします");
                return;
            }

            try
            {
                foreach (var player in players)
                {
                    _players.Add(player);
                    await Insert(player);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "プレイヤーリストの一括追加中にエラーが発生しました");
                // 各プレイヤーの追加処理内でエラーハンドリングを行っているため、
                // ここでの例外は予期せぬ例外のみ。アプリケーション継続のため再スローしない
            }
        }

        /// <summary>
        /// エントリーステータス更新
        /// </summary>
        /// <param name="player">更新対象プレイヤー</param>
        public async Task UpdateEntryStatus(Player player)
        {
            if (null == player)
            {
                _logger.LogWarning("プレイヤーオブジェクトがnullのため、ステータス更新処理をスキップします");
                return;
            }

            try
            {
                var getPlayerModel = await _client.From<PlayerModel>()
                    .Where(x => x.ID == player.ID)
                    .Single();
                
                if (null == getPlayerModel)
                {
                    _logger.LogWarning("更新対象のプレイヤーが見つかりません: {PlayerName}, ID: {PlayerId}", player.Name, player.ID);
                    return;
                }

                getPlayerModel.EntryStatus = (int)player.EntryStatus;
                await getPlayerModel.Update<PlayerModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "プレイヤーのエントリーステータス更新中に予期せぬエラーが発生しました: {PlayerName}, ID: {PlayerId}", 
                    player.Name, player.ID);
            }
        }

        /// <summary>
        /// スタッフステータス更新
        /// </summary>
        /// <param name="player">更新対象プレイヤー</param>
        public async Task UpdateStaffStatus(Player player)
        {
            if (player == null)
            {
                _logger.LogWarning("プレイヤーオブジェクトがnullのため、スタッフステータス更新処理をスキップします");
                return;
            }

            try
            {
                var getPlayerModel = await _client.From<PlayerModel>()
                    .Where(x => x.ID == player.ID)
                    .Single();

                if (null == getPlayerModel)
                {
                    _logger.LogWarning("更新対象のプレイヤーが見つかりません: {PlayerName}, ID: {PlayerId}", player.Name, player.ID);
                    return;
                }

                getPlayerModel.StaffStatus = player.StaffStatus;
                await getPlayerModel.Update<PlayerModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "プレイヤーのスタッフステータス更新中に予期せぬエラーが発生しました: {PlayerName}, ID: {PlayerId}", 
                    player.Name, player.ID);
            }
        }

        /// <summary>
        /// 経験ステータス更新
        /// </summary>
        /// <param name="player">更新対象プレイヤー</param>
        public async Task UpdateExpStatus(Player player)
        {
            if (player == null)
            {
                _logger.LogWarning("プレイヤーオブジェクトがnullのため、経験ステータス更新処理をスキップします");
                return;
            }

            try
            {
                var getPlayerModel = await _client.From<PlayerModel>()
                    .Where(x => x.ID == player.ID)
                    .Single();

                if (null == getPlayerModel)
                {
                    _logger.LogWarning("更新対象のプレイヤーが見つかりません: {PlayerName}, ID: {PlayerId}", player.Name, player.ID);
                    return;
                }

                getPlayerModel.ExpStatus = (int)player.ExpStatus;
                await getPlayerModel.Update<PlayerModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "プレイヤーの経験ステータス更新中に予期せぬエラーが発生しました: {PlayerName}, ID: {PlayerId}", 
                    player.Name, player.ID);
            }
        }

        /// <summary>
        /// Joinステータス更新
        /// </summary>
        /// <param name="players">更新対象プレイヤーリスト</param>
        public async Task UpdateJoinStatus(List<Player> players)
        {
            if (players == null || players.Count == 0)
            {
                _logger.LogWarning("プレイヤーリストが空またはnullのため、Joinステータス更新処理をスキップします");
                return;
            }

            foreach (var player in players)
            {
                var getPlayerModel = await _client.From<PlayerModel>()
                    .Where(x => x.ID == player.ID)
                    .Single();

                if (null == getPlayerModel)
                {
                    _logger.LogWarning("更新対象のプレイヤーが見つかりません: {PlayerName}, ID: {PlayerId}", player.Name, player.ID);
                    continue;
                }

                getPlayerModel.JoinStatus = player.JoinStatus;
                await getPlayerModel.Update<PlayerModel>();
            }
        }

        /// <summary>
        /// プレイヤーデータの再同期
        /// </summary>
        public async Task ResyncPlayer()
        {
            try
            {
                var result = await _client.From<PlayerModel>().Get();

                if (result.Models == null || result.Models.Count == 0)
                {
                    _logger.LogWarning("再同期対象のプレイヤーデータが見つかりません");
                    return;
                }

                foreach (var player in result.Models)
                {
                    var updatePlayer = _players.Find(x => x.ID == player.ID);
                    if (null != updatePlayer)
                    {
                        updatePlayer.EntryStatus = (emEntryStatus)player.EntryStatus;
                        updatePlayer.StaffStatus = player.StaffStatus;
                        updatePlayer.ExpStatus = (emExpStatus)player.ExpStatus;
                        updatePlayer.JoinStatus = player.JoinStatus;
                    }
                    else
                    {
                        var addPlayer = new Player(player.Name);
                        addPlayer.ID = player.ID;
                        addPlayer.EntryStatus = (emEntryStatus)player.EntryStatus;
                        addPlayer.StaffStatus = player.StaffStatus;
                        addPlayer.ExpStatus = (emExpStatus)player.ExpStatus;
                        addPlayer.JoinStatus = player.JoinStatus;
                        _players.Add(addPlayer);
                    }
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "プレイヤーデータの再同期中にJSONデシリアライズエラーが発生しました");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "プレイヤーデータの再同期中に予期せぬエラーが発生しました");
            }
        }

        /// <summary>
        /// 更新の購読開始
        /// </summary>
        public async Task SubscribeUpdates()
        {
            try
            {
                // 未実装
            }
            catch (Exception ex)
            {
                HandleError("Supabaseリアルタイム購読エラー", 
                    "Supabaseリアルタイム更新の購読設定中にエラーが発生しました", ex);
            }
        }

        // エラー通知時に抽象化された通知インターフェースを使用
        private void HandleError(string title, string message, Exception ex, bool isFatal = false)
        {
            _logger.LogError(ex, message);
            
            if (isFatal)
            {
                _exceptionNotifier.NotifyFatalError(title, message, ex);
            }
            else
            {
                _exceptionNotifier.NotifyRecoverableError(title, message, ex);
            }
        }
    }

    [Table("Player")]
    internal class PlayerModel : BaseModel
    {
        [PrimaryKey("ID")]
        public int ID { get; set; }
        [Column("Name")]
        public string Name { get; set; }
        [Column("EntryStatus")]
        public int EntryStatus {  get; set; }
        [Column("StaffStatus")]
        public bool StaffStatus { get; set; }
        [Column("ExpStatus")]
        public int ExpStatus { get; set; }
        [Column("JoinStatus")]
        public bool JoinStatus {  get; set; }
    }
}
