using System.Threading;
using System.Threading.Tasks;
using Teleglib.Middlewares;
using Teleglib.Storage;
using Teleglib.Telegram.Client;

namespace Teleglib.Renderers {
    public interface IClientRenderer {
        Task<MessageContext> Render(MiddlewareData middlewareData, ITelegramClient telegramClient, CancellationToken cancellationToken);
    }
}