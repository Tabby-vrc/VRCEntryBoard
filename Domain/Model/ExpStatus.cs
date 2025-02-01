using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCEntryBoard.Domain.Model
{
    /// <summary>経験値ステータス</summary>
    [System.Flags]
    public enum emExpStatus
    {
        /// <summary>集計外</summary>
        None        = 0,
        /// <summary>初見</summary>
        NewUser     = 1 << 0,
        /// <summary>初心者</summary>
        Beginner    = 1 << 1,
    }

    internal static class ExtensionsExpStatus
    {
        /// <summary>
        /// ビット判定用拡張メソッド
        /// </summary>
        /// <param name="expStatus">変換元</param>
        /// <returns>判定結果</returns>
        public static bool HasFlag(this emExpStatus expStatus, emExpStatus checkStatus)
        {
            // フラグが立っていないこと用チェック.
            if (checkStatus == emExpStatus.None)
            {
                return (expStatus == emExpStatus.None);
            }
            return (expStatus & checkStatus) == checkStatus;
        }
    }
}
