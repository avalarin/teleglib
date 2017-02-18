using System;
using System.Text.RegularExpressions;

namespace Teleglib.Router.Patterns {
    public class NamedRegexRoutePatternPart : NamedRoutePatternPart {
        private readonly Regex _regex;

        public NamedRegexRoutePatternPart(string name, Regex regex) : base(name) {
            if (regex == null) throw new ArgumentNullException(nameof(regex));
            _regex = regex;
        }

        public override bool IsMatch(string part) {
            return _regex.IsMatch(part);
        }
    }
}