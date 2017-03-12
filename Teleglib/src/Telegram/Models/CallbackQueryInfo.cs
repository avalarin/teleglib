using Newtonsoft.Json;

namespace Teleglib.Telegram.Models {
    [JsonObject(MemberSerialization.OptIn)]
    public class CallbackQueryInfo {

        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "from")]
        public UserInfo From { get; set; }

        [JsonProperty(PropertyName = "message")]
        public MessageInfo Message { get; set; }

        [JsonProperty(PropertyName = "data")]
        public string Data { get; set; }

    }
}