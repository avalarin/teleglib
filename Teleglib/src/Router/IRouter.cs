namespace Teleglib.Router {
    public interface IRouter {
        RoutingResult FindRoute(RoutingData routingData);

        void RegisterRoute(IRoute route);
    }
}