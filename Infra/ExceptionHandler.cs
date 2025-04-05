using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;
using VRCEntryBoard.Infra.Logger;

namespace VRCEntryBoard.Infra
{
    /// <summary>
    /// アプリケーション全体の例外処理を提供するクラス
    /// </summary>
    public static class ExceptionHandler
    {
        private static readonly ILogger _logger = LogManager.GetLogger("GlobalExceptionHandler");

        /// <summary>
        /// 致命的な例外を処理し、ユーザーに通知する
        /// </summary>
        /// <param name="ex">例外オブジェクト</param>
        /// <param name="title">エラータイトル</param>
        /// <param name="message">エラーメッセージ</param>
        /// <returns>アプリケーションを終了すべきかどうか</returns>
        public static bool HandleFatalException(Exception ex, string title, string message)
        {
            _logger.LogCritical(ex, "致命的なエラーが発生しました: {Message}", message);
            ShowExceptionMessageBox(title, message, ex);
            return true; // アプリケーションを終了すべき
        }

        /// <summary>
        /// 回復可能な例外を処理し、ユーザーに通知する
        /// </summary>
        /// <param name="ex">例外オブジェクト</param>
        /// <param name="title">エラータイトル</param>
        /// <param name="message">エラーメッセージ</param>
        /// <returns>ユーザーが続行を選択したかどうか</returns>
        public static bool HandleRecoverableException(Exception ex, string title, string message)
        {
            _logger.LogError(ex, "回復可能なエラーが発生しました: {Message}", message);
            
            string fullMessage = $"{message}\n\n";
            
            fullMessage += "アプリケーションを続行しますか？\n\n";
            
            DialogResult result = MessageBox.Show(
                fullMessage, 
                title, 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Warning);
                
            return result == DialogResult.Yes;
        }

        /// <summary>
        /// 例外情報をユーザーに表示するMessageBoxを表示
        /// </summary>
        /// <param name="title">エラータイトル</param>
        /// <param name="message">エラーメッセージ</param>
        /// <param name="ex">例外オブジェクト（nullの場合もあり）</param>
        public static void ShowExceptionMessageBox(string title, string message, Exception ex)
        {
            string fullMessage = $"{message}\n\n";
            
            MessageBox.Show(
                fullMessage, 
                title, 
                MessageBoxButtons.OK, 
                MessageBoxIcon.Error);
        }
    }
} 