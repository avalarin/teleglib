using System.Threading;
using System.Threading.Tasks;
using Teleglib.Telegram.Client;

namespace Teleglib.Renderers {
    public interface IClientRenderer {
        Task Render(ITelegramClient client, CancellationToken cancellationToken);
    }
}