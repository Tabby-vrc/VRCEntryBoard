using System.Threading.Tasks;

namespace VRCEntryBoard.Domain.Interfaces
{
    /// <summary>
    /// プレイヤーの優勝回数の提供
    /// </summary>
    internal interface IWinCountProvider
    {
        /// <summary>
        /// 優勝回数を読み込む（読み込み済みの場合は何もしない）
        /// 失敗しても例外は投げず、優勝回数は0として扱う
        /// </summary>
        Task LoadAsync();

        /// <summary>
        /// 優勝回数の取得
        /// </summary>
        /// <param name="playerName">プレイヤー名</param>
        /// <returns>優勝回数（未登録・未読み込みの場合は0）</returns>
        int GetWinCount(string playerName);
    }
}
