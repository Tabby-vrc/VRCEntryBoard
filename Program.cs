using System;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;

using VRCEntryBoard.Infra;
using VRCEntryBoard.Infra.SupabaseAdapter;
using VRCEntryBoard.Infra.VRChat;
using VRCEntryBoard.App.Controller;
using VRCEntryBoard.App.Services;
using VRCEntryBoard.HMI;
using VRCEntryBoard.HMI.Exception;
using VRCEntryBoard.Domain.Model;

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
            // メインフォームの取得と表示
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // グローバル例外ハンドラーの設定
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            try
            {
                // HMI層
                WindowsFormsExceptionNotifier exceptionNotifier = new WindowsFormsExceptionNotifier();

                // インフラ層
                CVRCDataLoderInLogfile VRCData = new CVRCDataLoderInLogfile();
                CEntryRecorder entryRecorder = new CEntryRecorder();
                entryRecorder.Initialize();
                SupabaseClient supabaseClient = new SupabaseClient(exceptionNotifier);

                // アプリケーション層
                VRCDataManagementService vrcDataManagementService = new VRCDataManagementService(VRCData, supabaseClient);
                CEntryViewController cEntryViewController = new CEntryViewController(vrcDataManagementService, supabaseClient);
                CEntryView cEntryView = new CEntryView(cEntryViewController);
                cEntryView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                cEntryView.Dock = DockStyle.Fill;

                MainForm form1 = new MainForm();
                form1.Controls.Add(cEntryView);
                Application.Run(form1);
            }
            catch (Exception ex)
            {
                HandleFatalException(ex);
            }
        }

        /// <summary>
        /// UIスレッドでの未処理例外をハンドル
        /// </summary>
        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            HandleFatalException(e.Exception);
        }

        /// <summary>
        /// 非UIスレッドでの未処理例外をハンドル
        /// </summary>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleFatalException(e.ExceptionObject as Exception);
        }

        /// <summary>
        /// 非同期タスクでの未処理例外をハンドル
        /// </summary>
        private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            HandleFatalException(e.Exception);
            e.SetObserved(); // 例外を処理済みとしてマーク
        }

        /// <summary>
        /// 致命的な例外の共通処理
        /// </summary>
        private static void HandleFatalException(Exception ex)
        {
            try
            {
                // ExceptionHandler.ShowExceptionMessageBox(
                //     "予期せぬエラー",
                //     "アプリケーションで予期せぬエラーが発生しました。アプリケーションを終了します。",
                //     ex);
            }
            finally
            {
                Application.Exit();
            }
        }
    }
}
