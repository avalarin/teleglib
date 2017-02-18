using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Teleglib.Controllers.Actions {
    public static class ActionsUtils {
        public static IEnumerable<MethodInfo> GetActions(Type type) {
            return type.GetTypeInfo()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.DeclaringType == type);
        }
    }
}