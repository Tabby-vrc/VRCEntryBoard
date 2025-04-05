using System;
using System.Windows.Forms;

namespace VRCEntryBoard.HMI.Exception
{
    internal class WindowsFormsExceptionNotifier : IExceptionNotifier
    {
        public void NotifyFatalError(string title, string message, System.Exception ex)
        {
            string fullMessage = $"{message}\n\n";
            
            MessageBox.Show(
                fullMessage, 
                title, 
                MessageBoxButtons.OK, 
                MessageBoxIcon.Error
            );
        }

        public bool NotifyRecoverableError(string title, string message, System.Exception ex)
        {
            // ユーザーに選択肢を提示するダイアログ
            var result = MessageBox.Show(
                message, 
                title,
                MessageBoxButtons.OKCancel, 
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button1,
                0,
                "続行する場合はOK、終了する場合はキャンセルを選択してください。");
                
            // OKならtrue（続行）、それ以外ならfalse（終了）
            return result == DialogResult.OK;
        }
    }
}