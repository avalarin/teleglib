using Newtonsoft.Json;

namespace Teleglib.Telegram.Models {
    [JsonObject(MemberSerialization.OptIn)]
    public class MessageInfo {

        [JsonProperty(PropertyName = "message_id", Required = Required.Always)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "user")]
        public UserInfo User { get; set; }

        [JsonProperty(PropertyName = "date", Required = Required.Always)]
        public long Date { get; set; }

        [JsonProperty(PropertyName = "chat", Required = Required.Always)]
        public ChatInfo Chat { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

    }
}