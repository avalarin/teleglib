using System;

namespace Teleglib.Controllers.Attributes {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class RouteAttribute : Attribute {

        public string Pattern { get; }

        public object Defaults { get; set; }

        public string Details { get; set; }

        public RouteAttribute(string pattern) {
            Pattern = pattern;
        }

    }
}