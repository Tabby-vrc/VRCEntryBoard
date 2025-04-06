using Microsoft.Extensions.Logging;
using System;

namespace VRCEntryBoard.Infra.Logger
{
    public static class LogManager
    {
        private static readonly SerilogAdapterFactory _factory = new SerilogAdapterFactory();

        public static ILogger<T> GetLogger<T>()
        {
            return new Logger<T>(_factory.CreateLogger(typeof(T).FullName));
        }

        public static ILogger GetLogger(string categoryName)
        {
            return _factory.CreateLogger(categoryName);
        }
    }

    // Microsoft.Extensions.Logging.Logger<T>の簡易実装
    internal class Logger<T> : ILogger<T>
    {
        private readonly ILogger _logger;

        public Logger(ILogger logger)
        {
            _logger = logger;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return _logger.BeginScope(state);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _logger.IsEnabled(logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _logger.Log(logLevel, eventId, state, exception, formatter);
        }
    }
} 