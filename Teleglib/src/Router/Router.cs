using System;
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

        public RoutingResult FindRoute(RoutingData routingData) {
            var matched = _routes
                .Select(r => r.Match(routingData))
                .Where(m => m.IsMatched).ToArray();

            if (matched.Length == 0) return RoutingResult.Mismatched;
            var completed = matched.FirstOrDefault(m => m.IsCompleted);
            return new RoutingResult(completed, matched.Where(r => !r.IsCompleted));
        }

        public void RegisterRoute(IRoute route) {
            _logger.LogDebug($"Register route {route}");
            _routes = _routes.Add(route);
        }
    }
}