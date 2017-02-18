using System.Collections.Immutable;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Teleglib.Router {
    public class Router : IRouter {
        private readonly ILogger _logger;
        private ImmutableList<IRoute> _routes;

        public Router(IRouterConfiguration configuration, ILoggerFactory loggerFactory) {
            _routes = configuration.Routes.ToImmutableList();
            _logger = loggerFactory.CreateLogger(GetType());
        }

        public RouteMatch FindRoute(RoutingData routingData) {
            return _routes
                .Select(r => r.Match(routingData))
                .Where(m => m.IsMatched)
                .OrderByDescending(m => m.IsCompleted)
                .FirstOrDefault() ?? RouteMatch.Unsuccess;
        }

        public void RegisterRoute(IRoute route) {
            _logger.LogDebug($"Register route {route}");
            _routes = _routes.Add(route);
        }
    }
}