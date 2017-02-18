using Teleglib.Middlewares;
using Teleglib.Polling;
using Teleglib.Telegram.Client;

namespace Teleglib.Bot {
    public interface IApplicationConfiguration {

        IClientConfiguration ClientConfiguration { get; }

        IAutoPollerConfiguration AutoPollerConfiguration { get; }

        IMiddlewaresChain Middlewares { get; }

        bool AutoPollingEnabled { get; }

    }
}