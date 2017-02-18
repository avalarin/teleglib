using System;
using System.Collections.Generic;

namespace Teleglib.Router {
    public class RouteMatch {

        public static readonly RouteMatch Unsuccess = new RouteMatch(false, false, null, null);

        public bool IsMatched { get; }

        public bool IsCompleted { get; }

        public IRoute Route { get; }

        public Dictionary<string, string> Fields { get; }

        private RouteMatch(bool isMatched, bool isCompleted, IRoute route, Dictionary<string, string> fields) {
            IsMatched = isMatched;
            IsCompleted = isCompleted;
            Route = route;
            Fields = fields;
        }

        public static RouteMatch Create(IRoute route, Dictionary<string, string> fields, bool completed) {
            if (route == null) throw new ArgumentNullException(nameof(route));
            if (fields == null) throw new ArgumentNullException(nameof(fields));
            return new RouteMatch(true, completed, route, fields);
        }

    }
}