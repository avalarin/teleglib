using System.Threading.Tasks;

namespace Teleglib.Middlewares {
    public interface IMiddlewaresChain {
        Task<MiddlewareData> NextAsync(MiddlewareData data);
    }
}