using System;

namespace Teleglib.Parameters {
    public class ParameterValue {
        public string Name { get; }

        public object Value { get; }

        public Type Type { get; }

        public ParameterValue(string key, object value) {
            Name = key ?? throw new ArgumentNullException(nameof(key));
            Value = value ?? throw new ArgumentNullException(nameof(value), $"Cannot create ParameterValue(\"{key}\", null)");
            Type = value.GetType();
        }
    }
}