using Newtonsoft.Json;
using Teleglib.Renderers;

namespace Teleglib.Telegram.Models {
    public class UpdateMessageData : SendMessageData {
        public UpdateMessageData() {
        }

        public UpdateMessageData(string chatId, long messageId, MessageData messageData) : base(chatId, messageData) {
            MessageId = messageId;
        }

        [JsonProperty(PropertyName = "message_id")]
        public long MessageId { get; set; }

    }
}