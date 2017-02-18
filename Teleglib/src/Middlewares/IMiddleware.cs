using System.Threading.Tasks;

namespace Teleglib.Middlewares {
    public interface IMiddleware {
        Task<MiddlewareData> InvokeAsync(MiddlewareData data, IMiddlewaresChain chain);
    }
}