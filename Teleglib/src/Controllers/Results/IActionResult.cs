using Teleglib.Middlewares;

namespace Teleglib.Controllers.Results {
    public interface IActionResult {
        MiddlewareData Render(MiddlewareData input);
    }
}