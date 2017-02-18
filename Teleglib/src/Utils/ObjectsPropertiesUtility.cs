using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Teleglib.Utils {
    public static class ObjectPropertiesUtility {
        private static readonly object LockRoot = new object();
        private static readonly IDictionary<Type, IDictionary<string, IPropertyAccessor>> Cache =
            new Dictionary<Type, IDictionary<string, IPropertyAccessor>>();
        private static readonly MethodInfo LambdaExpressionMethod = typeof(Expression).GetGenericMethod("Lambda",
            BindingFlags.Public | BindingFlags.Static, (type1) => new[] { typeof(Expression), typeof(ParameterExpression[]) });

        public static IEnumerable<KeyValuePair<string, object>> GetProperties(this object data) {
            if (data == null) throw new ArgumentNullException(nameof(data));
            var type = data.GetType();
            IDictionary<string, IPropertyAccessor> cache;
            if (!Cache.TryGetValue(type, out cache)) {
                lock (LockRoot) {
                    if (!Cache.TryGetValue(type, out cache)) {
                        cache = new Dictionary<string, IPropertyAccessor>();
                        foreach (var property in type.GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(property => property.CanRead)) {
                            var accessor = GetPropertyAccessor(type, property);
                            cache[property.Name] = accessor;
                            yield return new KeyValuePair<string, object>(property.Name, accessor.GetValue(data));
                        }
                        Cache[type] = cache;
                        yield break;
                    }
                }
            }
            foreach (var item in cache) {
                yield return new KeyValuePair<string, object>(item.Key, item.Value.GetValue(data));
            }
        }

        private static IPropertyAccessor GetPropertyAccessor(Type type, PropertyInfo property) {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (property == null) throw new ArgumentNullException(nameof(property));
            var genericFastAccessorType = typeof(PropertyAccessor<,>).MakeGenericType(type, property.PropertyType);
            var parameterExpression = Expression.Parameter(type);
            var genericFuncType = typeof(Func<,>).MakeGenericType(type, property.PropertyType);
            var genericLambdaExpressionMethod = LambdaExpressionMethod.MakeGenericMethod(genericFuncType);
            var propertyAccessExpression = Expression.Property(parameterExpression, property);
            var lambdaExpression = (LambdaExpression)genericLambdaExpressionMethod.Invoke(null,
                new object[] { propertyAccessExpression, new[] { parameterExpression } });
            var compiledLambda = lambdaExpression.Compile();
            var accessor = (IPropertyAccessor)Activator.CreateInstance(genericFastAccessorType, compiledLambda);
            return accessor;
        }

        private interface IPropertyAccessor {
            object GetValue(object obj);
        }

        private sealed class PropertyAccessor<T, T2> : IPropertyAccessor {
            private readonly Func<T, T2> _accessor;

            public PropertyAccessor(Func<T, T2> accessor) {
                _accessor = accessor;
            }

            public object GetValue(object obj) {
                return _accessor((T)obj);
            }
        }
    }
}