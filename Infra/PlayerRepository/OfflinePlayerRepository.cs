using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using VRCEntryBoard.Infra.Logger;
using VRCEntryBoard.Domain.Interfaces;
using VRCEntryBoard.Domain.Model;
using VRCEntryBoard.HMI.Exception;
using VRCEntryBoard.Domain.Exceptions;

namespace VRCEntryBoard.Infra.PlayerRepository
{
    /// <summary>
    /// オフラインモード用プレイヤーリポジトリ
    /// </summary>
    internal class OfflinePlayerRepository : IPlayerRepository
    {
        private readonly string _cachePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PlayerCache.json");
        private readonly ILogger<OfflinePlayerRepository> _logger;
        private readonly IExceptionNotifier _exceptionNotifier;
        private List<Player> _players;

        public OfflinePlayerRepository(IExceptionNotifier exceptionNotifier)
        {
            _exceptionNotifier = exceptionNotifier ?? 
                throw new ArgumentNullException(nameof(exceptionNotifier));
            _logger = LogManager.GetLogger<OfflinePlayerRepository>();
            _players = new List<Player>();
            
            // 初期化時にキャッシュをロード
            LoadCache();
        }

        /// <summary>
        /// キャッシュのロード
        /// </summary>
        private void LoadCache()
        {
            try
            {
                if (File.Exists(_cachePath))
                {
                    string jsonContent = File.ReadAllText(_cachePath);
                    var cachedPlayers = JsonSerializer.Deserialize<List<Player>>(jsonContent);
                    if (cachedPlayers != null)
                    {
                        _players = cachedPlayers;
                        _logger.LogInformation("ローカルキャッシュから{0}人のプレイヤーデータをロードしました", _players.Count);
                    }
                }
                else
                {
                    _logger.LogWarning("ローカルキャッシュファイルが見つかりません");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ローカルキャッシュのロードに失敗しました");
                throw new VRCApplicationException(
                    "キャッシュ読み込みエラー",
                    "プレイヤーデータキャッシュの読み込みに失敗しました。",
                    isFatal: false,
                    innerException: ex);
            }
        }

        /// <summary>
        /// キャッシュの保存
        /// </summary>
        private void SaveCache()
        {
            try
            {
                string jsonContent = JsonSerializer.Serialize(_players, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_cachePath, jsonContent);
                _logger.LogInformation("ローカルキャッシュを保存しました");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ローカルキャッシュの保存に失敗しました");
                throw new VRCApplicationException(
                    "キャッシュ保存エラー",
                    "プレイヤーデータキャッシュの保存に失敗しました。",
                    isFatal: false,
                    innerException: ex);
            }
        }

        /// <summary>
        /// プレイヤーリスト取得
        /// </summary>
        public List<Player> GetPlayers()
        {
            return _players;
        }

        /// <summary>
        /// プレイヤー追加
        /// </summary>
        public Task Insert(Player player)
        {
            if (player == null)
            {
                return Task.CompletedTask;
            }

            // 既存プレイヤーの確認
            var existingPlayer = _players.Find(p => p.Name == player.Name);
            if (existingPlayer == null)
            {
                // 新規追加
                player.ID = GenerateLocalId();
                _players.Add(player);
            }
            else
            {
                // 既存プレイヤーの情報更新
                existingPlayer.EntryStatus = player.EntryStatus;
                existingPlayer.StaffStatus = player.StaffStatus;
                existingPlayer.ExpStatus = player.ExpStatus;
                existingPlayer.JoinStatus = player.JoinStatus;
            }

            SaveCache();
            return Task.CompletedTask;
        }

        /// <summary>
        /// プレイヤーリスト追加
        /// </summary>
        public Task Insert(List<Player> players)
        {
            if (players == null || players.Count == 0)
            {
                return Task.CompletedTask;
            }

            foreach (var player in players)
            {
                var existingPlayer = _players.Find(p => p.Name == player.Name);
                if (existingPlayer == null)
                {
                    player.ID = GenerateLocalId();
                    _players.Add(player);
                }
            }

            SaveCache();
            return Task.CompletedTask;
        }

        /// <summary>
        /// エントリーステータス更新
        /// </summary>
        public Task UpdateEntryStatus(Player player)
        {
            if (player == null)
            {
                return Task.CompletedTask;
            }

            var targetPlayer = _players.Find(p => p.ID == player.ID || p.Name == player.Name);
            if (targetPlayer != null)
            {
                targetPlayer.EntryStatus = player.EntryStatus;
                SaveCache();
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// スタッフステータス更新
        /// </summary>
        public Task UpdateStaffStatus(Player player)
        {
            if (player == null)
            {
                return Task.CompletedTask;
            }

            var targetPlayer = _players.Find(p => p.ID == player.ID || p.Name == player.Name);
            if (targetPlayer != null)
            {
                targetPlayer.StaffStatus = player.StaffStatus;
                SaveCache();
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// 経験ステータス更新
        /// </summary>
        public Task UpdateExpStatus(Player player)
        {
            if (player == null)
            {
                return Task.CompletedTask;
            }

            var targetPlayer = _players.Find(p => p.ID == player.ID || p.Name == player.Name);
            if (targetPlayer != null)
            {
                targetPlayer.ExpStatus = player.ExpStatus;
                SaveCache();
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Joinステータス更新
        /// </summary>
        public Task UpdateJoinStatus(List<Player> players)
        {
            if (players == null || players.Count == 0)
            {
                return Task.CompletedTask;
            }

            bool updated = false;
            foreach (var player in players)
            {
                var targetPlayer = _players.Find(p => p.ID == player.ID || p.Name == player.Name);
                if (targetPlayer != null)
                {
                    targetPlayer.JoinStatus = player.JoinStatus;
                    updated = true;
                }
            }

            if (updated)
            {
                SaveCache();
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// プレイヤーデータの再同期
        /// オフラインモードでは何もしない
        /// </summary>
        public Task ResyncPlayer()
        {
            // オフラインモードではキャッシュ内のデータのみを使うので処理不要
            return Task.CompletedTask;
        }

        /// <summary>
        /// 更新の購読開始
        /// オフラインモードでは何もしない
        /// </summary>
        public Task SubscribeUpdates()
        {
            // オフラインモードでは購読不要
            return Task.CompletedTask;
        }

        /// <summary>
        /// ローカルIDの生成
        /// </summary>
        private int GenerateLocalId()
        {
            // ローカルIDは負の値とし、Supabaseのものと区別
            int maxId = 0;
            foreach (var player in _players)
            {
                if (player.ID < 0 && player.ID < maxId)
                {
                    maxId = player.ID;
                }
            }
            return maxId - 1; // 既存の最小値-1を返す
        }

        // エラー通知時に抽象化された通知インターフェースを使用
        private void HandleError(string title, string message, Exception ex, bool isFatal = false)
        {
            // すべてのエラーをログに記録
            _logger.LogError(ex, message);
            
            if (isFatal)
            {
                // 致命的エラーの場合はVRCApplicationExceptionをスロー
                throw new VRCApplicationException(title, message, isFatal, ex);
            }
            else
            {
                // 回復可能なエラーの場合:
                // ユーザーに通知して処理を継続可能にする
                _exceptionNotifier.NotifyRecoverableError(title, message, ex);
            }
        }
    }
}
