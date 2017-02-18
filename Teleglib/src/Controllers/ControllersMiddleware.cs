using System.Threading.Tasks;
using Teleglib.Middlewares;

namespace Teleglib.Controllers {
    public class ControllersMiddleware : IMiddleware {
        private readonly IControllerFactory _controllerFactory;

        public ControllersMiddleware(IControllerFactory controllerFactory) {
            _controllerFactory = controllerFactory;
        }

        public async Task<MiddlewareData> InvokeAsync(MiddlewareData data, IMiddlewaresChain chain) {
            var controller = _controllerFactory.GetController(data);

            var newData = await controller.InvokeAsync(data);

            return await chain.NextAsync(newData);
        }
    }
}