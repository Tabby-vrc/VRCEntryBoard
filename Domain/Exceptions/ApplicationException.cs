using System;

namespace VRCEntryBoard.Domain.Exceptions
{
    /// <summary>
    /// アプリケーション固有の例外の基底クラス
    /// </summary>
    public class VRCApplicationException : Exception
    {
        /// <summary>
        /// エラーのタイトル
        /// </summary>
        public string ErrorTitle { get; }

        /// <summary>
        /// 詳細なエラーメッセージ
        /// </summary>
        public string DetailedMessage { get; }

        /// <summary>
        /// アプリケーションを終了すべきかどうか
        /// </summary>
        public bool IsFatal { get; }

        /// <summary>
        /// 新しいアプリケーション例外を初期化します
        /// </summary>
        /// <param name="errorTitle">エラーのタイトル</param>
        /// <param name="detailedMessage">詳細なエラーメッセージ</param>
        /// <param name="isFatal">アプリケーションを終了すべきかどうか</param>
        /// <param name="innerException">内部例外</param>
        public VRCApplicationException(
            string errorTitle,
            string detailedMessage,
            bool isFatal = false,
            Exception innerException = null)
            : base(detailedMessage, innerException)
        {
            ErrorTitle = errorTitle;
            DetailedMessage = detailedMessage;
            IsFatal = isFatal;
        }
    }
} 