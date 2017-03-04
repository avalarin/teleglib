using System.Threading.Tasks;
using Teleglib.Features;
using Teleglib.Middlewares;
using Teleglib.Renderers;

namespace Teleglib.Storage {
    public class ContextMiddleware : IMiddleware {
        private readonly ISessionStorage _sessionStorage;

        public ContextMiddleware(ISessionStorage sessionStorage) {
            _sessionStorage = sessionStorage;
        }

        public async Task<MiddlewareData> InvokeAsync(MiddlewareData data, IMiddlewaresChain chain) {
            var message = data.Features.RequireOne<UpdateInfoFeature>().GetAnyMessage();
            var context = await _sessionStorage.LoadMessageContextAsync(message.Chat.Id, message.Id) ??
                                MessageContext.Builder().Build();
            var feature = new ContextFeature(context);

            var output = await chain.NextAsync(data.UpdateFeatures(f => f.AddExclusive<ContextFeature>(feature)));
            var outputContext = output.Features.RequireOne<ContextFeature>().Context;

            await _sessionStorage.SaveMessageContextAsync(message.Chat.Id, message.Id, outputContext);

            return output;
        }
    }
}