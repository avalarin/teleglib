using Teleglib.Middlewares;

namespace Teleglib.Controllers {
    public interface IControllerFactory {
        IController GetController(MiddlewareData input);
    }
}