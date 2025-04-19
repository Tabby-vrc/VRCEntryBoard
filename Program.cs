using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using VRCEntryBoard.Infra;
using VRCEntryBoard.Infra.PlayerRepository;
using VRCEntryBoard.Infra.VRChat;
using VRCEntryBoard.App.Controller;
using VRCEntryBoard.App.Services;
using VRCEntryBoard.HMI;
using VRCEntryBoard.HMI.Exception;
using VRCEntryBoard.Domain.Interfaces;
using VRCEntryBoard.Domain.Exceptions;
using VRCEntryBoard.Infra.Logger;

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
            // アプリケーション開始ログの出力
            var logger = LogManager.GetLogger("Application");
            logger.LogInformation("==================================================");
            logger.LogInformation("アプリケーション起動開始 - VRCEntryBoard v{0}", GetAppVersion());
            logger.LogInformation("==================================================");

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
                logger.LogInformation("メインフォーム表示開始");
                Application.Run(mainForm);
                
                // アプリケーション正常終了のログ
                logger.LogInformation("==================================================");
                logger.LogInformation("アプリケーション正常終了");
                logger.LogInformation("==================================================");
            }
            catch (Exception ex)
            {
                // エラー終了のログは例外ハンドラで記録されるため、ここでは不要
                HandleException(ex);
            }
        }

        /// <summary>
        /// アプリケーションのバージョンを取得
        /// </summary>
        private static string GetAppVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
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
            services.AddSingleton<PlayerRepositoryFactory>();
            services.AddSingleton<IPlayerRepository>(provider => {
                var factory = provider.GetRequiredService<PlayerRepositoryFactory>();
                return factory.CreateRepository();
            });
            services.AddSingleton<SupabaseClient>();
            services.AddSingleton<IRegulationRepository, SupabaseRegulationRepository>();

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
            HandleException(e.Exception);
        }

        /// <summary>
        /// 非UIスレッドでの未処理例外をハンドル
        /// </summary>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException(e.ExceptionObject as Exception);
        }

        /// <summary>
        /// 非同期タスクでの未処理例外をハンドル
        /// </summary>
        private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            HandleException(e.Exception);
            e.SetObserved(); // 例外を処理済みとしてマーク
        }

        /// <summary>
        /// 例外の共通処理
        /// </summary>
        private static void HandleException(Exception ex)
        {
            try
            {
                var logger = LogManager.GetLogger("Application");
                logger.LogError("==================================================");
                logger.LogError("アプリケーションでエラーが発生しました");
                
                // アプリケーション例外とその他の例外を区別
                if (ex is VRCApplicationException appEx)
                {
                    // アプリケーション例外の場合は、設定されたタイトルとメッセージを使用
                    logger.LogError($"エラー種別: {appEx.ErrorTitle}");
                    Infra.ExceptionHandler.HandleFatalException(
                        ex,
                        appEx.ErrorTitle,
                        appEx.DetailedMessage);
                    
                    // 致命的でない場合は、アプリケーションを終了しない
                    if (!appEx.IsFatal)
                    {
                        return;
                    }
                }
                else
                {
                    // 予期しない例外の場合は、デフォルトのメッセージを使用
                    logger.LogError("エラー種別: 予期せぬエラー");
                    Infra.ExceptionHandler.HandleFatalException(
                        ex,
                        "予期せぬエラー",
                        "アプリケーションで予期せぬエラーが発生しました。アプリケーションを終了します。");
                }
                
                logger.LogError("アプリケーションを終了します");
                logger.LogError("==================================================");
            }
            finally
            {
                // VRCApplicationExceptionでIsFatalがfalseの場合はここに到達しない
                Application.Exit();
            }
        }
    }
}
