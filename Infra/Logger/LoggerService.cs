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
                    // 例外出力はメッセージのみ表示し、スタックトレースは含めない
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}",
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
                        // ファイル名のみを出力し、完全パスは含めない
                        Console.WriteLine($"古いログファイルを削除しました: {logFiles[i].Name}");
                    }
                    catch (Exception ex)
                    {
                        // エラーメッセージからパス情報を除去
                        Console.WriteLine($"ファイル削除エラー: {ex.GetType().Name}");
                    }
                }
            }
            catch (Exception ex)
            {
                // ログファイルのクリーンアップ中の例外は無視（アプリケーション動作に影響させない）
                // エラーメッセージからパス情報を除去
                Console.WriteLine($"ログクリーンアップエラー: {ex.GetType().Name}");
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
            
            // パス情報を含む可能性のある例外スタックトレースを除去
            // 例外が存在する場合は、例外メッセージのみをログに含める
            string logMessage;
            if (exception != null)
            {
                // 例外のメッセージのみを使用し、スタックトレースは含めない
                logMessage = $"[{_categoryName}] {message} - 例外: {exception.GetType().Name}: {exception.Message}";
            }
            else
            {
                logMessage = $"[{_categoryName}] {message}";
            }

            switch (logLevel)
            {
                case LogLevel.Trace:
                    _logger.Verbose(logMessage);
                    break;
                case LogLevel.Debug:
                    _logger.Debug(logMessage);
                    break;
                case LogLevel.Information:
                    _logger.Information(logMessage);
                    break;
                case LogLevel.Warning:
                    _logger.Warning(logMessage);
                    break;
                case LogLevel.Error:
                    _logger.Error(logMessage);
                    break;
                case LogLevel.Critical:
                    _logger.Fatal(logMessage);
                    break;
                default:
                    _logger.Information(logMessage);
                    break;
            }
        }
    }
} 