using Teleglib.Router;

namespace Teleglib.Storage {
    public static class SessionStorageKeys {
        public static ISessionStorageKey<RoutingData> PreviousRoutingData = new SessionStorageKey<RoutingData>("ROUTING_DATA");
    }
}