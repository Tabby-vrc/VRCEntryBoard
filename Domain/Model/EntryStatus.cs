using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCEntryBoard.Domain.Model
{
    /// <summary>エントリーステータス</summary>
    public enum emEntryStatus
    {
        /// <summary>不明</summary>
        Unknown,
        /// <summary>未確認</summary>
        AskMe,
        /// <summary>参加</summary>
        Entry,
        /// <summary>見学</summary>
        Visiter
    }

    internal static class ExtensionsEntryStatus
    {
        /// <summary>
        /// View用拡張メソッド
        /// </summary>
        /// <param name="entryStatus">変換元</param>
        /// <returns>表示用文言</returns>
        public static string ToStringEx(this emEntryStatus entryStatus)
        {
            switch (entryStatus)
            {
                case emEntryStatus.AskMe:
                    return "未確認";
                case emEntryStatus.Entry:
                    return "参加";
                case emEntryStatus.Visiter:
                    return "見学";
                default:
                    return "";
            }
        }
    }
}
