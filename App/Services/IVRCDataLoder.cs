using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VRCEntryBoard.Infra.VRChat;
using VRCEntryBoard.Domain.Model;

namespace VRCEntryBoard.App.Services
{
    internal interface IVRCDataLoder
    {
        /// <summary>
        /// データ更新
        /// </summary>
        List<VRChatLogFileModel> GetVRCData();
    }
}
