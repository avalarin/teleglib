using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Teleglib.Middlewares;
using Teleglib.Utils;

namespace Teleglib.Parameters {
    public class ActionParametersMapper : IActionParametersMapper {

        public object[] MapParameters(MiddlewareData data, IEnumerable<ParameterInfo> parameters) {
            var allValues = data.Features.GetAllOfBaseType<IParameterValuesSource>()
                .SelectMany(source => source.GetValues())
                .Union(new [] { new ParameterValue("middlewareData", data) });

            var groupped = parameters.GroupJoin(
                inner: allValues,
                outerKeySelector: p => p.ParameterType,
                innerKeySelector: v => v.Type,
                resultSelector: (parameter, values) => new {parameter, values = values.ToArray()},
                comparer: ParametersEqualityComparer.Instance
            );

            return groupped.Select(g => SelectValue(g.parameter, g.values)).ToArray();
        }

        private static object SelectValue(ParameterInfo parameter, ParameterValue[] values) {
            if (parameter.ParameterType.IsMathesWithGenericDefinition(typeof(Nullable<>))) {
                return values.Select(v => v.Value).ToList();
            }
            if (values.Length == 1) return values[0].Value;
            if (values.Length == 0) return null;

            try {
                return values.SingleOrDefault(v => v.Name.Equals(parameter.Name));
            }
            catch (InvalidOperationException e) {
                throw new InvalidOperationException($"Cannot select value for parameter {parameter.ParameterType.FullName} {parameter.Name}");
            }
        }

        private class ParametersEqualityComparer : IEqualityComparer<Type> {
            public static readonly ParametersEqualityComparer Instance = new ParametersEqualityComparer();

            public bool Equals(Type parameterType, Type valueType) {
                if (parameterType.IsMathesWithGenericDefinition(typeof(Nullable<>))) {
                    var typeOfItem = parameterType.GetTypeInfo().GetGenericArguments()[0];
                    return typeOfItem == valueType;
                }
                return parameterType == valueType;
            }

            public int GetHashCode(Type type) {
                return type.GetHashCode();
            }
        }
    }
}