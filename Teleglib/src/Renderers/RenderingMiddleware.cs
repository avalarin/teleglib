using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Teleglib.Middlewares;
using Teleglib.Session;
using Teleglib.Telegram.Client;

namespace Teleglib.Renderers {
    public class RenderingMiddleware : IMiddleware {
        private readonly ITelegramClient _telegramClient;

        public RenderingMiddleware(ITelegramClient telegramClient) {
            _telegramClient = telegramClient;
        }

        public async Task<MiddlewareData> InvokeAsync(MiddlewareData data, IMiddlewaresChain chain) {
            var outputData = await chain.NextAsync(data);

            var tasks = outputData.Renderers
                .Select(r => r.Render(outputData, _telegramClient, CancellationToken.None))
                .ToArray();

            await Task.WhenAll(tasks);
            var joinedContext = tasks.Select(t => t.Result);

            var newData = outputData.UpdateFeatures(f =>
                f.ReplaceExclusive<SessionFeature>(e =>
                    e.Join(joinedContext)
                )
            );

            return newData;
        }

    }
}