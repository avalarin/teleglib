using System.Collections.Generic;
using System.Linq;
using Teleglib.Router;

namespace Teleglib.Storage {
    public class MessageContext {

        public RoutingData RoutingData { get; }

        public IEnumerable<long> SentMessageIds { get; }

        private MessageContext(IEnumerable<long> sentMessageIds, RoutingData routingData) {
            SentMessageIds = sentMessageIds;
            RoutingData = routingData;
        }

        public MessageContext Join(MessageContext second) {
            return new MessageContext(this.SentMessageIds.Concat(second.SentMessageIds), RoutingData);
        }

        public MessageContext Join(IEnumerable<MessageContext> contexts) {
            return contexts.Aggregate(this, (prev, next) => prev.Join(next));
        }

        public static MessageContextBuilder Builder() {
            return new MessageContextBuilder();
        }

        public static MessageContext Empty() {
            return new MessageContext(Enumerable.Empty<long>(), null);
        }

        public class MessageContextBuilder {
            private RoutingData _routingData;
            private readonly List<long> _messageIds = new List<long>();

            public MessageContextBuilder AddMessageId(long messageId) {
                _messageIds.Add(messageId);
                return this;
            }

            public MessageContextBuilder SetRoutingData(RoutingData routingData) {
                _routingData = routingData;
                return this;
            }

            public MessageContext Build() {
                return new MessageContext(_messageIds.AsReadOnly(), _routingData);
            }

        }

    }
}