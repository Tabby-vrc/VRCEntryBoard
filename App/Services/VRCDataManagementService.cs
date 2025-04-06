using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;

using VRCEntryBoard.Infra.VRChat;
using VRCEntryBoard.Domain.Interfaces;
using VRCEntryBoard.Domain.Model;
using VRCEntryBoard.Domain.Exceptions;

namespace VRCEntryBoard.App.Services
{
    /// <summary>
    /// イベント管理サービス
    /// 外部ソースイベントと永続化リポジトリの仲介を行う
    /// </summary>
    internal class VRCDataManagementService
    {
        private readonly IVRCDataLoder _vrcDataLoder;
        private readonly IPlayerRepository _playerRepository;
        private readonly List<string> _monitoredWorldNames;
        private bool _isMonitoring = false;
        private readonly string _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VRCEntryBoard-config.json");

        public VRCDataManagementService(
            IVRCDataLoder vrcDataLoder,
            IPlayerRepository playerRepository)
        {
            _vrcDataLoder = vrcDataLoder;
            _playerRepository = playerRepository;
            _monitoredWorldNames = LoadConfig().MonitoredWorldNames;
        }

        /// <summary>
        /// プレイヤーリストの更新
        /// </summary>
        public async Task UpdatePlayerList()
        {
            // 外部ソースから最新のイベントを取得
            var latestEvents = _vrcDataLoder.GetVRCData();
            // 並行して現在のプレイヤーリストの同期
            var resyncPlayerTask = _playerRepository.ResyncPlayer();

            // モニター対象のワールドのイベントのみにふるい分け
            // 同じプレイヤーイベントが複数溜まっていることがあるので、
            // イベント後勝ちで精査する
            var joinEvent = new List<VRChatLogFileModel>();
            var leaveEvent = new List<VRChatLogFileModel>();
            foreach (var @event in latestEvents)
            {
                switch (@event.EventType)
                {
                    case EventType.PlayerJoin:
                        if(!_isMonitoring) continue;
                        if(!joinEvent.Any(e => e.PlayerName == @event.PlayerName))
                            joinEvent.Add(@event);
                        leaveEvent.RemoveAll(e => e.PlayerName == @event.PlayerName);
                        break;
                    case EventType.PlayerLeft:
                        if(!_isMonitoring) continue;
                        if(!leaveEvent.Any(e => e.PlayerName == @event.PlayerName))
                            leaveEvent.Add(@event);
                        joinEvent.RemoveAll(e => e.PlayerName == @event.PlayerName);
                        break;
                    case EventType.WorldMove:
                        _isMonitoring = true;
                        if(_monitoredWorldNames.Contains(@event.WorldName))
                            _isMonitoring = true;
                        else
                            _isMonitoring = false;
                        break;
                }
            }

            // 永続化モデルへ変換
            // プレイヤーリストを再同期
            await resyncPlayerTask;
            var players = _playerRepository.GetPlayers();
            
            // 新規プレイヤーは追加
            var newJoinEvents = joinEvent.Where(e => players.Find(p => p.Name == e.PlayerName) == null).ToList();
            var newJoinPlayers = newJoinEvents.Select(e => new Player(e.PlayerName)).ToList();
            _playerRepository.Insert(newJoinPlayers);

            // 既存プレイヤーはJoinステータスを更新
            var updateJoinPlayers = players.Where(p => joinEvent.Any(e => e.PlayerName == p.Name)).ToList();
            updateJoinPlayers.ForEach(p => p.JoinStatus = true);
            var updateLeavePlayers = players.Where(p => leaveEvent.Any(e => e.PlayerName == p.Name)).ToList();
            updateLeavePlayers.ForEach(p => p.JoinStatus = false);
            _playerRepository.UpdateJoinStatus(new List<Player>(updateJoinPlayers.Concat(updateLeavePlayers)));
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
                        "設定ファイルの読み込みに失敗しました。",
                        isFatal: true);
                }

                // 設定の検証
                if (config.MonitoredWorldNames == null || !config.MonitoredWorldNames.Any())
                {
                    throw new VRCApplicationException(
                        "設定ファイルエラー",
                        "設定が不完全です: 監視対象のワールド名が設定されていません",
                        isFatal: true);
                }

                return config;
            }
            catch (JsonException ex)
            {
                throw new VRCApplicationException(
                    "設定ファイルエラー",
                    "設定ファイルのJSONフォーマットが不正です",
                    isFatal: true,
                    innerException: ex);
            }
            catch (Exception ex) when (ex is not VRCApplicationException)
            {
                throw new VRCApplicationException(
                    "設定ファイルエラー",
                    "設定ファイルの読み込みに失敗しました。",
                    isFatal: true,
                    innerException: ex);
            }
        }
    }
} 