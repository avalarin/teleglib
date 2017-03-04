using Newtonsoft.Json;

namespace Teleglib.Telegram.Models {
    public class UpdateMessageData : SendMessageData {

        [JsonProperty(PropertyName = "message_id")]
        public long MessageId { get; set; }

    }
}