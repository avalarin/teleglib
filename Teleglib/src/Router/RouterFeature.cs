using System.Collections.Generic;
using System.Collections.ObjectModel;
using Teleglib.Features;

namespace Teleglib.Router {
    public class RouterFeature : IFeature {
        public IRoute Route { get; }

        public ReadOnlyDictionary<string, string> Fields { get; }

        public RouterFeature(IRoute route, Dictionary<string, string> fields) {
            Route = route;
            Fields = new ReadOnlyDictionary<string, string>(fields);
        }
    }
}