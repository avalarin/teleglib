using System;
using System.Linq;
using System.Reflection;

namespace Teleglib.Utils {
    public static class TypeExtensions {

        public static bool IsMathesWithGenericDefinition(this Type type, Type genericDifinition) {
            var typeInfo = type.GetTypeInfo();
            return typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == genericDifinition;
        }

        public static MethodInfo GetGenericMethod(this Type type, string name, BindingFlags flags, Func<Type, Type[]> typesProvider) {
            return GetGenericMethod(type, name, flags, 1, types => typesProvider(types[0]));
        }

        public static MethodInfo GetGenericMethod(this Type type, string name, BindingFlags flags, Func<Type, Type, Type[]> typesProvider) {
            return GetGenericMethod(type, name, flags, 2, types => typesProvider(types[0], types[1]));
        }

        public static MethodInfo GetGenericMethod(this Type type, string name, BindingFlags flags, Func<Type, Type, Type, Type[]> typesProvider) {
            return GetGenericMethod(type, name, flags, 3, types => typesProvider(types[0], types[1], types[2]));
        }

        public static MethodInfo GetGenericMethod(this Type type, string name, BindingFlags flags, int genericArgsCount, Func<Type[], Type[]> typesProvider) {
            var nameComparer = ((flags & BindingFlags.IgnoreCase) != 0) ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
            var methods = type.GetTypeInfo().GetMethods(flags).Where(m => nameComparer.Equals(name, m.Name));
            MethodInfo matched = null;
            foreach (var method in methods) {
                var genericArgs = method.GetGenericArguments();
                if (genericArgs.Length != genericArgsCount) continue;
                var types = typesProvider(genericArgs);
                if (!method.GetParameters().Select(p => p.ParameterType).SequenceEqual(types)) continue;
                if (matched != null) {
                    throw new AmbiguousMatchException("More than one matching method found.");
                }
                matched = method;
            }
            return matched;
        }
    }
}
