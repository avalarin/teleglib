using Newtonsoft.Json;

namespace Teleglib.Telegram.Models {
    [JsonObject(MemberSerialization.OptIn)]
    public class UpdateInfo {

        [JsonProperty(PropertyName = "update_id", Required = Required.Always)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "message")]
        public MessageInfo Message { get; set; }

        [JsonProperty(PropertyName = "edited_message")]
        public MessageInfo EditedMessage { get; set; }

        [JsonProperty(PropertyName = "channel_post")]
        public MessageInfo ChannelPost { get; set; }

        [JsonProperty(PropertyName = "edited_channel_post")]
        public MessageInfo EditedChannelPost { get; set; }

    }
}