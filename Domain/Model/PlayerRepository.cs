using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCEntryBoard.Domain.Model
{
    internal class PlayerRepository
    {
        private readonly List<Player> _playerList;

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
        private void AddPlayer(Player addPlayer)
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
            {
                
            }
        }
    }
}
