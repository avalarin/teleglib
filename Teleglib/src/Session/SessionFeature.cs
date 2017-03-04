using System.Collections.Generic;
using Teleglib.Features;
using Teleglib.Router;

namespace Teleglib.Session {
    public class SessionFeature : IFeature {

        public MessageContext Context { get; }

        public SessionFeature SetRoutingData(RoutingData routingData) {
            var newContext = MessageContext.Builder()
                .SetRoutingData(routingData)
                .Build()
                .Join(Context);
            return new SessionFeature(newContext);
        }

        public SessionFeature Join(IEnumerable<MessageContext> contexts) {
            return new SessionFeature(Context.Join(contexts));
        }

        public SessionFeature(MessageContext context) {
            Context = context;
        }

    }
}