using System;

namespace Teleglib.Parameters {
    public class ParameterValue {
        public string Name { get; }

        public object Value { get; }

        public Type Type { get; }

        public ParameterValue(string key, object value) {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));
            Name = key;
            Value = value;
            Type = value.GetType();
        }
    }
}