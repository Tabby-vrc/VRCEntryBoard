using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VRCEntryBoard.Domain.Model;

namespace VRCEntryBoard.Domain.Interfaces
{
    internal interface IVRCDataLoder
    {
        /// <summary>
        /// データ更新
        /// </summary>
        void UpdatePlayerList();
        
        /// <summary>
        /// プレイヤーリスト取得
        /// </summary>
        /// <returns>リスト</returns>
        List<Player> GetPlayerList();
    }
}
