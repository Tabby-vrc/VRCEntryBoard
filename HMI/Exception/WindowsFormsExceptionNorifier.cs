using System;
using System.Windows.Forms;

namespace VRCEntryBoard.HMI.Exception
{
    public class WindowsFormsExceptionNotifier : IExceptionNotifier
    {
        public void NotifyFatalError(string title, string message, System.Exception ex)
        {
            string fullMessage = $"{message}\n\n";
            
            if (ex != null)
            {
                fullMessage += $"エラーの詳細: {ex.Message}\n\n";
            }

            MessageBox.Show(
                fullMessage, 
                title, 
                MessageBoxButtons.OK, 
                MessageBoxIcon.Error
            );
        }

        public void NotifyRecoverableError(string title, string message, System.Exception ex)
        {
            string fullMessage = $"{message}\n\n";
            
            if (ex != null)
            {
                fullMessage += $"エラーの詳細: {ex.Message}\n\n";
            }
            MessageBox.Show(
                fullMessage, 
                title, 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Warning
            );
        }
    }
}