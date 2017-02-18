namespace Teleglib.Router {
    public interface IRoute {
        string Name { get; }

        RouteMatch Match(RoutingData routingData);
    }
}