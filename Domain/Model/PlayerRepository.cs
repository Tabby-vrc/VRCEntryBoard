using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCEntryBoard.Domain.Model
{
    internal class PlayerRepository
    {
        private List<Player> _playerList;

        public PlayerRepository()
        {
            _playerList = new List<Player>();
        }

        /// <summary>
        /// プレイヤー追加
        /// </summary>
        public void AddPlayer(List<Player> addPlayers)
        {
            _playerList.Clear();
            foreach (Player addPlayer in addPlayers)
            {
                AddPlayer(addPlayer);
            }
        }

        /// <summary>
        /// プレイヤー追加
        /// </summary>
        public void AddPlayer(Player addPlayer)
        {
            Player player = _playerList.FirstOrDefault(p => p.Name == addPlayer.Name);
            if (null == player)
            {
                _playerList.Add(addPlayer);
            }
            else
            {
                player.JoinStatus = true;
            }
        }

        /// <summary>
        /// プレイヤーの離席処理
        /// </summary>
        public void LeavePlayer(string playerName)
        {
            var player = _playerList.FirstOrDefault(p => p.Name == playerName);
            if (player == null) return;

            if (player.EntryStatus == emEntryStatus.Entry || player.StaffStatus)
            {
                // Entry済み又はスタッフプレイヤーの離席はリスト除外しない
                player.JoinStatus = false;
            }
            else
            {
                _playerList.Remove(player);
            }
        }

        /// <summary>
        /// ワールド移動時の処理
        /// </summary>
        public void OnWorldChange()
        {
            // 確認済みプレイヤーのみ残してリストリセット.
            _playerList.RemoveAll(player => player.EntryStatus == emEntryStatus.AskMe);
            foreach (var player in _playerList)
            {
                player.JoinStatus = false;
            }
        }
        /// <summary>
        /// プレイヤーリスト取得
        /// </summary>
        /// <returns>プレイヤーリスト</returns>
        public List<Player> GetPlayerList()
        {
            return _playerList;
        }

        /// <summary>
        /// ステータス更新
        /// </summary>
        public void UpdateStatus(string targetPlayerName, emEntryStatus status)
        {
            Player player = _playerList.FirstOrDefault(p => p.Name == targetPlayerName);
            if (null == player)
            {
                // エラー.
                return;
            }

            player.EntryStatus = status;
        }
        /// <summary>
        /// ステータス取得
        /// </summary>
        public emEntryStatus GetEntryStatus(string targetPlayerName)
        {
            Player player = _playerList.FirstOrDefault(p => p.Name == targetPlayerName);
            if (null == player)
            {
                // エラー.
                return emEntryStatus.Unknown;
            }

            return player.EntryStatus;
        }

        /// <summary>
        /// 経験値更新
        /// </summary>
        public void UpdateExpStatus(string targetPlayerName, emExpStatus expStatus)
        {
            Player player = _playerList.FirstOrDefault(p => p.Name == targetPlayerName);
            if(null == player)
            {
                // エラー.
                return;
            }

            player.ExpStatus = expStatus;
        }
        public emExpStatus GetExpStatus(string targetPlayerName)
        {
            Player player = _playerList.FirstOrDefault(p => p.Name == targetPlayerName);
            if (null == player)
            {
                // エラー.
                return emExpStatus.None;
            }

            return player.ExpStatus;
        }

        /// <summary>
        /// スタッフステータス更新
        /// </summary>
        public void UpdateStaff(string targetPlayerName, bool isStaff)
        {
            Player player = _playerList.FirstOrDefault(p => p.Name == targetPlayerName);
            if(null == player)
            {
                // エラー.
                return;
            }

            player.StaffStatus = isStaff;
        }
        public bool GetStaff(string targetPlayerName)
        {
            Player player = _playerList.FirstOrDefault(p => p.Name == targetPlayerName);
            if(null == player)
            {
                // エラー.
                return false;
            }

            return player.StaffStatus;
        }
    }
}
