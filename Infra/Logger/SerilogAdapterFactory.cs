using Microsoft.Extensions.Logging;
using System;

namespace VRCEntryBoard.Infra.Logger
{
    public class SerilogAdapterFactory : ILoggerFactory
    {
        public void AddProvider(ILoggerProvider provider)
        {
            // このシンプルな実装では使用しない
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new LoggerService(categoryName);
        }

        public void Dispose()
        {
            // 特に破棄するリソースはなし
        }
    }
} 