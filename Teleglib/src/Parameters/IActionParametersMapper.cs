using System.Collections.Generic;
using System.Reflection;
using Teleglib.Middlewares;

namespace Teleglib.Parameters {
    public interface IActionParametersMapper {
        object[] MapParameters(MiddlewareData data, IEnumerable<ParameterInfo> parameters);
    }
}