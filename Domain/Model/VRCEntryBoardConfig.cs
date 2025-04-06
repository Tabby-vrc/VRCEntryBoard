using System.Collections.Generic;

namespace VRCEntryBoard.Domain.Model
{
    internal sealed class VRCEntryBoardConfig
    {
        public string Url { get; set; }
        public string Key { get; set; }
        public List<string> MonitoredWorldNames { get; set; }
    }
}
