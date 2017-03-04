using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Teleglib.Controllers.Results;
using Teleglib.Middlewares;
using Teleglib.Parameters;
using Teleglib.Router;

namespace Teleglib.Controllers.Actions {
    internal class ActionsController: IController {
        private readonly IActionParametersMapper _actionParametersMapper;
        private readonly object _instance;
        private readonly Dictionary<string, MethodInfo> _methods;

        public ActionsController(IActionParametersMapper actionParametersMapper, object instance) {
            _actionParametersMapper = actionParametersMapper;
            _instance = instance;

            var type = instance.GetType();
            _methods = ActionsUtils.GetActions(type)
                .ToDictionary(m => m.Name, m => m, StringComparer.OrdinalIgnoreCase);
        }

        public async Task<MiddlewareData> InvokeAsync(MiddlewareData input) {
            var fields = input.Features.RequireOne<RouterFeature>().Fields;
            var action = fields["action"];

            MethodInfo method;
            if (!_methods.TryGetValue(action, out method)) {
                throw new Exception($"Action '{action}' not found");
            }

            var parameters = method.GetParameters();
            var values = _actionParametersMapper.MapParameters(input, parameters);
            object objResult;

            if (typeof(Task<object>).GetTypeInfo().IsAssignableFrom(method.ReturnType)) {
                var task = (Task<object>)method.Invoke(_instance, values);
                objResult = await task;
            }
            else {
                objResult = await Task.Run(() => method.Invoke(_instance, values));
            }

            var actionResult = objResult as IActionResult;
            if (actionResult != null) {
                return actionResult.Render(input);
            }
            throw new Exception("Cannot process result");
        }

    }
}