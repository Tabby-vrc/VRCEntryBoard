using System.Collections.Generic;

namespace VRCEntryBoard.Domain.Model
{
    internal sealed class VRCEntryBoardConfig
    {
        public string Url { get; set; }
        public string Key { get; set; }
        public List<string> MonitoredWorldNames { get; set; }
        /// <summary>優勝回数ランキングのURL（未指定・空文字の場合は優勝回数の機能を使わない）</summary>
        public string WinCountRankingUrl { get; set; }
    }
}
