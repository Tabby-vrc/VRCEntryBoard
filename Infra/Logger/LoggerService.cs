using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Linq;

namespace VRCEntryBoard.Infra.Logger
{
    public class LoggerService : Microsoft.Extensions.Logging.ILogger
    {
        private readonly Serilog.ILogger _logger;
        private readonly string _categoryName;

        public LoggerService(string categoryName)
        {
            _categoryName = categoryName;
            _logger = GetLogger();
        }

        private static Serilog.ILogger GetLogger()
        {
            string logFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            
            // ログディレクトリが存在しない場合は作成
            if (!Directory.Exists(logFolder))
            {
                Directory.CreateDirectory(logFolder);
            }

            // 古いログファイルを削除（3日分より古いもの）
            CleanupOldLogFiles(logFolder, keepFiles: 3);

            return new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(
                    Path.Combine(logFolder, "vrc-entryboard-.log"),
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    shared: true,  // 複数プロセスからの書き込みを許可
                    flushToDiskInterval: TimeSpan.FromSeconds(10)) // 定期的にディスクに書き込み
                .CreateLogger();
        }

        /// <summary>
        /// 古いログファイルを削除する
        /// </summary>
        /// <param name="logFolder">ログフォルダのパス</param>
        /// <param name="keepFiles">保持するファイル数</param>
        private static void CleanupOldLogFiles(string logFolder, int keepFiles)
        {
            try
            {
                // ログフォルダ内のログファイルを日付の降順でソート
                var logFiles = Directory.GetFiles(logFolder, "vrc-entryboard-*.log")
                    .Select(f => new FileInfo(f))
                    .OrderByDescending(f => f.LastWriteTime)
                    .ToList();
                
                // 指定したファイル数より多いファイルを削除
                for (int i = keepFiles; i < logFiles.Count; i++)
                {
                    try
                    {
                        File.Delete(logFiles[i].FullName);
                        Console.WriteLine($"古いログファイルを削除しました: {logFiles[i].Name}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"ファイル削除エラー: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                // ログファイルのクリーンアップ中の例外は無視（アプリケーション動作に影響させない）
                Console.WriteLine($"ログクリーンアップエラー: {ex.Message}");
            }
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var message = formatter(state, exception);
            var logMessage = $"[{_categoryName}] {message}";

            switch (logLevel)
            {
                case LogLevel.Trace:
                    _logger.Verbose(exception, logMessage);
                    break;
                case LogLevel.Debug:
                    _logger.Debug(exception, logMessage);
                    break;
                case LogLevel.Information:
                    _logger.Information(exception, logMessage);
                    break;
                case LogLevel.Warning:
                    _logger.Warning(exception, logMessage);
                    break;
                case LogLevel.Error:
                    _logger.Error(exception, logMessage);
                    break;
                case LogLevel.Critical:
                    _logger.Fatal(exception, logMessage);
                    break;
                default:
                    _logger.Information(exception, logMessage);
                    break;
            }
        }
    }
} 