using System;

namespace Teleglib.Router.Patterns {
    public class NamedRoutePatternPart : IRoutePatternPart {
        public string Name { get; }

        public NamedRoutePatternPart(string name) {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty");
            Name = name;
        }

        public virtual bool IsMatch(string part) {
            return true;
        }
    }
}