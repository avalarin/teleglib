 using System.Collections.Generic;
using System.Linq;
using Teleglib.Utils;

namespace Teleglib.Router.Patterns {
    public class PatternsRoute : IRoute {

        public string Name { get; }
        private string Pattern { get; }
        private IRoutePatternPart[] Parts { get; }
        private Dictionary<string, string> Defaults { get; }

        public PatternsRoute(string name, string pattern, Dictionary<string, string> defaults) {
            Name = name;
            Defaults = defaults;
            Parts = RoutePatternParser.Parse(pattern).ToArray();
            Pattern = pattern;
        }

        public RouteMatch Match(RoutingData routingData) {
            if (routingData.PathParts.Length > Parts.Length) return RouteMatch.Unsuccess;

            var matched = Parts.Zip(routingData.PathParts, (patternPart, pathPart) => new {patternPart, pathPart})
                               .Where(t => t.pathPart != null && t.patternPart.IsMatch(t.pathPart))
                               .ToArray();

            if (matched.Length != routingData.PathParts.Length) {
                return RouteMatch.Unsuccess;
            }

            var fields = new Dictionary<string, string>();
            if (Defaults != null) fields.Merge(Defaults);
            matched
                .Select(t => new KeyValuePair<string, string>((t.patternPart as NamedRoutePatternPart)?.Name, t.pathPart))
                .Where(kvp => kvp.Key != null)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
                .MergeTo(fields);

            var completed = routingData.PathParts.Length == Parts.Length;

            return RouteMatch.Create(this, fields, completed);
        }

        public override string ToString() {
            return Name + " (" + Pattern + ")";
        }
    }
}