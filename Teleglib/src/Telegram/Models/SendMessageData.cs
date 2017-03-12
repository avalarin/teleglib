using Newtonsoft.Json;
using Teleglib.Renderers;
using Teleglib.Telegram.Converters;

namespace Teleglib.Telegram.Models {
    public class SendMessageData {
        public SendMessageData() {
        }

        public SendMessageData(string chatId, MessageData messageData) {
            ChatId = chatId;
            Text = messageData.Text;
            ReplyMarkup = messageData.ReplyMarkup;
        }

        [JsonProperty(PropertyName = "chat_id")]
        public string ChatId { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "parse_mode", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(MessageParseModeJsonConverter))]
        public MessageParseMode? ParseMode { get; set; }

        [JsonProperty(PropertyName = "disable_web_page_preview", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DisableWebPagePreview { get; set; }

        [JsonProperty(PropertyName = "disable_notification", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DisableNotification { get; set; }

        [JsonProperty(PropertyName = "reply_to_message_id", NullValueHandling = NullValueHandling.Ignore)]
        public long? ReplyToMessageId { get; set; }

        [JsonProperty(PropertyName = "reply_markup", NullValueHandling = NullValueHandling.Ignore)]
        public InlineKeyboardMarkup ReplyMarkup { get; set; }

    }
}