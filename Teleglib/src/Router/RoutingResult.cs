using System;
using System.Collections.Generic;
using System.Linq;

namespace Teleglib.Router {
    public class RoutingResult {

        public static RoutingResult Mismatched = new RoutingResult();

        public bool IsMatched => IsCompleted || (UncompletedRoutes?.Any() ?? false);

        public bool IsCompleted => CompletedRoute != null;

        public RouteMatch CompletedRoute { get; }

        public IEnumerable<RouteMatch> UncompletedRoutes { get; }

        private RoutingResult() {
        }

        public RoutingResult(RouteMatch completedRoute, IEnumerable<RouteMatch> uncompletedRoutes) {
            if (uncompletedRoutes == null) throw new ArgumentNullException(nameof(uncompletedRoutes));

            CompletedRoute = completedRoute;
            UncompletedRoutes = uncompletedRoutes;
        }

    }
}