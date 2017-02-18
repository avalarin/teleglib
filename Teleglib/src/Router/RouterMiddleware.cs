using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Teleglib.Features;
using Teleglib.Middlewares;

namespace Teleglib.Router {
    public class RouterMiddleware : IMiddleware {

        private static readonly Regex FindCommandRegex = new Regex(@"^\s*/(.+?)( .*|\Z)", RegexOptions.Compiled);

        private readonly IRouter _router;

        public RouterMiddleware(IRouter router) {
            _router = router;
        }

        public async Task<MiddlewareData> InvokeAsync(MiddlewareData data, IMiddlewaresChain chain) {
            var routeData = ParseRoutingData(data);

            var route = _router.FindRoute(routeData);
            if (!route.IsMatched) {
                throw new Exception("Route not found");
            }

            var feature = new RouterFeature(route.Route, route.Fields);
            var newData = data.UpdateFeatures(f => f.AddExclusive<RouterFeature>(feature));

            return await chain.NextAsync(newData);
        }

        private static RoutingData ParseRoutingData(MiddlewareData data) {
            var messageText = data.Features.RequireOne<UpdateInfoFeature>().Update.Message.Text;
            RoutingData routingData;
            var match = FindCommandRegex.Match(messageText);
            if (!match.Success) {
                return new RoutingData("", new string[0], messageText);
            }
            var path = match.Groups[1].Value;
            var parts = path.Split(':');
            var content = match.Groups[2].Value;
            return new RoutingData(path, parts, content);
        }
    }
}