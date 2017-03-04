using System.Collections.Generic;
using Teleglib.Features;
using Teleglib.Renderers;
using Teleglib.Router;

namespace Teleglib.Storage {
    public class ContextFeature : IFeature {

        public MessageContext Context { get; }

        public ContextFeature SetRoutingData(RoutingData routingData) {
            var newContext = MessageContext.Builder()
                .SetRoutingData(routingData)
                .Build()
                .Join(Context);
            return new ContextFeature(newContext);
        }

        public ContextFeature Join(IEnumerable<MessageContext> contexts) {
            return new ContextFeature(Context.Join(contexts));
        }

        public ContextFeature(MessageContext context) {
            Context = context;
        }

    }
}