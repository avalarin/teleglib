using System.Collections.Generic;

namespace Teleglib.Router {
    public interface IRouterConfiguration {
        IEnumerable<IRoute> Routes { get; }
    }
}