namespace Teleglib.Router {
    public interface IRouter {
        RouteMatch FindRoute(RoutingData routingData);

        void RegisterRoute(IRoute route);
    }
}