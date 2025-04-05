using Microsoft.Extensions.DependencyInjection;
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
using VRCEntryBoard.Domain.Interfaces;

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
                // DIコンテナの設定
                var services = new ServiceCollection();
                ConfigureServices(services);
                var serviceProvider = services.BuildServiceProvider();

                // メインフォームの取得と表示
                var mainForm = serviceProvider.GetRequiredService<MainForm>();
                Application.Run(mainForm);
            }
            catch (Exception ex)
            {
                HandleFatalException(ex);
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // HMI層
            services.AddSingleton<IExceptionNotifier, WindowsFormsExceptionNotifier>();
            services.AddTransient<CEntryView>();
            services.AddSingleton<MainForm>();

            // インフラ層
            services.AddSingleton<IVRCDataLoder, CVRCDataLoderInLogfile>();
            services.AddSingleton<IEntryRecorder, CEntryRecorder>();
            services.AddSingleton<IPlayerRepository, SupabaseClient>();

            // アプリケーション層
            services.AddSingleton<VRCDataManagementService>();
            services.AddSingleton<CEntryViewController>();

            // 構成処理
            services.AddTransient<Action<MainForm>>(provider => form =>
            {
                var entryView = provider.GetRequiredService<CEntryView>();
                entryView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                entryView.Dock = DockStyle.Fill;
                form.Controls.Add(entryView);
            });
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
                // ExceptionHandlerを使用して例外通知とログ記録を一元化
                Infra.ExceptionHandler.HandleFatalException(
                    ex,
                    "予期せぬエラー",
                    "アプリケーションで予期せぬエラーが発生しました。アプリケーションを終了します。");
            }
            finally
            {
                Application.Exit();
            }
        }
    }
}
