using System;
using System.Collections.Generic;

namespace Teleglib.Router {
    public class RouteMatch {

        public static readonly RouteMatch Unsuccess = new RouteMatch();

        public bool IsMatched { get; private set; }

        public bool IsCompleted { get; private set; }

        public IRoute Route { get; private set; }

        public Dictionary<string, string> Fields { get; private set; }

        public RouteCompletionData CompletionData { get; private set; }

        private RouteMatch() {
        }

        public static RouteMatch CreateCompleted(IRoute route, Dictionary<string, string> fields) {
            if (route == null) throw new ArgumentNullException(nameof(route));
            if (fields == null) throw new ArgumentNullException(nameof(fields));
            return new RouteMatch() {
                IsMatched = true,
                IsCompleted = true,
                Route = route,
                Fields = fields,
                CompletionData = null
            };
        }

        public static RouteMatch CreateUncompleted(IRoute route, Dictionary<string, string> fields, RouteCompletionData completionText) {
            if (route == null) throw new ArgumentNullException(nameof(route));
            if (fields == null) throw new ArgumentNullException(nameof(fields));
            return new RouteMatch() {
                IsMatched = true,
                IsCompleted = false,
                Route = route,
                Fields = fields,
                CompletionData = completionText
            };
        }

    }
}