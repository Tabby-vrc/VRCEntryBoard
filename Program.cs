using System;
using System.Windows.Forms;

using VRCEntryBoard.App.Controller;
using VRCEntryBoard.Infra;
using VRCEntryBoard.HMI;

namespace VRCEntryBoard
{
    internal static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            CVRCDataLoderInLogfile cVRCData = new CVRCDataLoderInLogfile();
            CEntryRecorder entryRecorder = new CEntryRecorder();
            entryRecorder.Initialize();
            CEntryViewController cEntryViewController = new CEntryViewController(cVRCData);
            CEntryView cEntryView = new CEntryView(cEntryViewController);
            cEntryView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            cEntryView.Dock = DockStyle.Fill;

            MainForm form1 = new MainForm();
            form1.Controls.Add(cEntryView);
            Application.Run(form1);
        }
    }
}
