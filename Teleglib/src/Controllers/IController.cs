using System.Threading.Tasks;
using Teleglib.Middlewares;

namespace Teleglib.Controllers {
    public interface IController {
        Task<MiddlewareData> InvokeAsync(MiddlewareData input);
    }
}