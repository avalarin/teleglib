using Teleglib.Middlewares;

namespace Teleglib.Controllers.Actions {
    public interface IActionResult {
        MiddlewareData Render(MiddlewareData input);
    }
}