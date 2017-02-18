using System;

namespace Teleglib.Router.Patterns {
    public class ExactRoutePatternPart : IRoutePatternPart {
        private readonly string _exactValue;

        public ExactRoutePatternPart(string exactValue) {
            if (exactValue == null) throw new ArgumentNullException(nameof(exactValue));
            if (string.IsNullOrWhiteSpace(exactValue)) throw new ArgumentException("Value cannot be empty");
            _exactValue = exactValue;
        }

        public bool IsMatch(string part) {
            return _exactValue.Equals(part, StringComparison.OrdinalIgnoreCase);
        }
    }
}