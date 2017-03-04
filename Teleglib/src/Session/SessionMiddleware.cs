using System.Threading.Tasks;
using Teleglib.Features;
using Teleglib.Middlewares;

namespace Teleglib.Session {
    public class SessionMiddleware : IMiddleware {
        private readonly ISessionStorage _sessionStorage;

        public SessionMiddleware(ISessionStorage sessionStorage) {
            _sessionStorage = sessionStorage;
        }

        public async Task<MiddlewareData> InvokeAsync(MiddlewareData data, IMiddlewaresChain chain) {
            var message = data.Features.RequireOne<UpdateInfoFeature>().GetAnyMessage();
            var context = await _sessionStorage.LoadMessageContextAsync(message.Chat.Id, message.Id) ??
                                MessageContext.Builder().Build();
            var feature = new SessionFeature(context);

            var output = await chain.NextAsync(data.UpdateFeatures(f => f.AddExclusive<SessionFeature>(feature)));
            var outputContext = output.Features.RequireOne<SessionFeature>().Context;

            await _sessionStorage.SaveMessageContextAsync(message.Chat.Id, message.Id, outputContext);

            return output;
        }
    }
}