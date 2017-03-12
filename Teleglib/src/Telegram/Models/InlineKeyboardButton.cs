using Newtonsoft.Json;

namespace Teleglib.Telegram.Models {
    [JsonObject(MemberSerialization.OptIn)]
    public class InlineKeyboardButton {

        [JsonProperty(PropertyName = "text", Required = Required.Always)]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "url", NullValueHandling = NullValueHandling.Ignore)]
        public string Url { get; set; }

        [JsonProperty(PropertyName = "callback_data", NullValueHandling = NullValueHandling.Ignore)]
        public string CallbackData { get; set; }

        public InlineKeyboardButton() {
        }

        public InlineKeyboardButton(string text) {
            Text = text;
        }
    }
}