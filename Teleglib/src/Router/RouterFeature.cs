using System.Collections.Generic;
using System.Collections.ObjectModel;
using Teleglib.Features;

namespace Teleglib.Router {
    public class RouterFeature : IFeature {
        public IRoute Route { get; }

        public ReadOnlyDictionary<string, string> Fields { get; }

        public RoutingData RoutingData { get; }

        public RouterFeature(IRoute route, RoutingData routingData, Dictionary<string, string> fields) {
            Route = route;
            RoutingData = routingData;
            Fields = new ReadOnlyDictionary<string, string>(fields);
        }
    }
}