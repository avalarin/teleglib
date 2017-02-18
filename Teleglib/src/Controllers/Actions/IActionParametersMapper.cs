using System.Reflection;
using Teleglib.Middlewares;

namespace Teleglib.Controllers.Actions {
    public interface IActionParametersMapper {
        object[] MapParameters(MiddlewareData data, ParameterInfo[] parameters);
    }
}