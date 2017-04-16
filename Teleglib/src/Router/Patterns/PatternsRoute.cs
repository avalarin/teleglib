 using System;
 using System.Collections.Generic;
using System.Linq;
 using Teleglib.Utils;

namespace Teleglib.Router.Patterns {
    public class PatternsRoute : IRoute {
        public string Name { get; }
        private string Pattern { get; }
        private string Details { get; }
        private IRoutePatternPart[] Parts { get; }
        private Dictionary<string, string> Defaults { get; }

        public PatternsRoute(string name, string pattern, Dictionary<string, string> defaults, string details) {
            Name = name;
            Defaults = defaults;
            Details = details;
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

            if (completed) return RouteMatch.CreateCompleted(this, fields);

            var nextPart = Parts[routingData.PathParts.Length];
            var nextExactPath = nextPart as ExactRoutePatternPart;
            if (nextExactPath == null) throw new InvalidOperationException("Cannot create completion link for route part " + nextPart.GetType());

            var completionPath = "/" + Parts.Take(routingData.PathParts.Length + 1)
                                     .Cast<ExactRoutePatternPart>()
                                     .Select(p => p.ExactValue)
                                     .JoinToString(":");
            var completionData = new RouteCompletionData(Details, completionPath);
            return RouteMatch.CreateUncompleted(this, fields, completionData);
        }

        public override string ToString() {
            return Name + " (" + Pattern + ")";
        }
    }
}