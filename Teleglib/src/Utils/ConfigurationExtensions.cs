using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace Teleglib.Utils {
    public static class ConfigurationExtensions {

        public static T Get<T>(this IConfiguration configuration, string key, Func<string, T> factory = null) {
            var str = configuration[key];
            if (str == null) return default(T);
            if (factory != null) factory?.Invoke(str);

            var valueType = typeof(T);
            if (valueType.GetTypeInfo().IsGenericType && valueType.GetGenericTypeDefinition() == typeof(Nullable<>)) {
                valueType = Nullable.GetUnderlyingType(valueType);
            }

            return (T)Convert.ChangeType(str, valueType);
        }

    }
}