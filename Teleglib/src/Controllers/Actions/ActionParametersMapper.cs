using System.Linq;
using System.Reflection;
using Teleglib.Middlewares;

namespace Teleglib.Controllers.Actions {
    public class ActionParametersMapper : IActionParametersMapper {
        
        public object[] MapParameters(MiddlewareData data, ParameterInfo[] parameters) {
            return parameters.Select(p => FindValue(p, data)).ToArray();
        }

        private object FindValue(ParameterInfo parameterInfo, MiddlewareData data) {
            if (parameterInfo.ParameterType == typeof(MiddlewareData)) {
                return data;
            }
            return null;
        }
    }
}