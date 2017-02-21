using System;

namespace Teleglib.Telegram.Client {
    public interface IClientConfiguration {
        string Token { get; }
        TimeSpan? RequestTimeout { get; }
        TimeSpan? DefaultGetUpdatesTimeout { get; }
    }
}