using System.Collections.Generic;
using System.Threading.Tasks;
using Teleglib.Renderers;
using Teleglib.Router;

namespace Teleglib.Storage {
    public class InMemorySessionStorage : ISessionStorage {
        private readonly Dictionary<long, RoutingData> _routingDatas = new Dictionary<long, RoutingData>();
        private readonly Dictionary<MessageIdentifier, MessageContext> _contexts = new Dictionary<MessageIdentifier, MessageContext>();

        public Task SaveMessageContextAsync(long chatId, long messageId, MessageContext context) {
            var id = new MessageIdentifier(chatId, messageId);
            _contexts[id] = context;
            return Task.CompletedTask;
        }

        public Task<MessageContext> LoadMessageContextAsync(long chatId, long messageId) {
            var id = new MessageIdentifier(chatId, messageId);
            MessageContext context;
            if (!_contexts.TryGetValue(id, out context)) {
                return Task.FromResult<MessageContext>(null);
            }
            return Task.FromResult(context);
        }

        public Task SaveRoutingDataAsync(long chatId, RoutingData routingData) {
            _routingDatas[chatId] = routingData;
            return Task.CompletedTask;
        }

        public Task ClearRoutingDataAsync(long chatId) {
            _routingDatas[chatId] = null;
            return Task.CompletedTask;
        }

        public Task<RoutingData> GetRoutingDataAsync(long chatId) {
            RoutingData routingData;
            if (!_routingDatas.TryGetValue(chatId, out routingData)) {
                return Task.FromResult<RoutingData>(null);
            }
            return Task.FromResult(routingData);
        }

        public class MessageIdentifier {
            public long ChatId { get; }
            public long MessageId { get; }

            public MessageIdentifier(long chatId, long messageId) {
                ChatId = chatId;
                MessageId = messageId;
            }

            protected bool Equals(MessageIdentifier other) {
                return ChatId == other.ChatId && MessageId == other.MessageId;
            }

            public override bool Equals(object obj) {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((MessageIdentifier) obj);
            }

            public override int GetHashCode() {
                unchecked {
                    return (ChatId.GetHashCode() * 397) ^ MessageId.GetHashCode();
                }
            }
        }
    }
}