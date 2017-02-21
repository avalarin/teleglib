using System.Collections.Generic;
using System.Linq;
using Teleglib.Router.Patterns;
using Teleglib.Utils;

namespace Teleglib.Router {
    public class RouterConfigurationBuilder {
        private readonly List<PatternsRoute> _routes;

        public RouterConfigurationBuilder() {
            _routes = new List<PatternsRoute>();
        }

        public RouterConfigurationBuilder MapRoute(string name, string pattern, object defaults = null) {
            var defaultsDict = defaults?.GetProperties().ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.ToString())
                ?? new Dictionary<string, string>();

            _routes.Add(new PatternsRoute(name, pattern, defaultsDict));
            return this;
        }

        public IRouterConfiguration Build() {
            return new Configuration(_routes.AsReadOnly());
        }

        private class Configuration : IRouterConfiguration {
            public IEnumerable<IRoute> Routes { get; }

            public Configuration(IEnumerable<PatternsRoute> routes) {
                Routes = routes;
            }
        }
    }
}