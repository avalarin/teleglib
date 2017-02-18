using System;
using Microsoft.Extensions.Logging;

namespace Teleglib.Telegram.Client {
    public interface IClientConfiguration {
        string Token { get; }
        ILoggerFactory LoggerFactory { get; }
        TimeSpan? RequestTimeout { get; }
        TimeSpan? DefaultGetUpdatesTimeout { get; }
    }
}