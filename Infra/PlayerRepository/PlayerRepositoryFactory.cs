using System;
using Microsoft.Extensions.Logging;

using VRCEntryBoard.HMI.Exception;
using VRCEntryBoard.Infra.Logger;
using VRCEntryBoard.Domain.Interfaces;

namespace VRCEntryBoard.Infra.PlayerRepository
{
    /// <summary>
    /// プレイヤーリポジトリファクトリ
    /// </summary>
    internal class PlayerRepositoryFactory
    {
        private readonly ILogger<PlayerRepositoryFactory> _logger;
        private readonly IExceptionNotifier _exceptionNotifier;

        public PlayerRepositoryFactory(IExceptionNotifier exceptionNotifier)
        {
            _exceptionNotifier = exceptionNotifier ?? 
                throw new ArgumentNullException(nameof(exceptionNotifier));
            _logger = LogManager.GetLogger<PlayerRepositoryFactory>();
        }

        /// <summary>
        /// プレイヤーリポジトリの作成
        /// </summary>
        public IPlayerRepository CreateRepository()
        {
            try
            {
                _logger.LogInformation("オンラインモードでのリポジトリ初期化を試みます");
                return new SupabaseClient(_exceptionNotifier);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "オンラインモードでの初期化に失敗しました。オフラインモードに切り替えます");
                _exceptionNotifier.NotifyRecoverableError(
                    "接続エラー", 
                    "オンラインサービスに接続できませんでした。オフラインモードで動作します。", 
                    ex);
                
                return new OfflinePlayerRepository(_exceptionNotifier);
            }
        }
    }
}
