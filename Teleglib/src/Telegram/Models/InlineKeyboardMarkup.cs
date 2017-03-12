using System.Collections.Generic;
using Newtonsoft.Json;
using Teleglib.Utils;

namespace Teleglib.Telegram.Models {
    [JsonObject(MemberSerialization.OptIn)]
    public class InlineKeyboardMarkup  {

        [JsonProperty(PropertyName = "inline_keyboard")]
        public IEnumerable<IEnumerable<InlineKeyboardButton>> InlineKeyboard { get; }

        public InlineKeyboardMarkup(IEnumerable<IEnumerable<InlineKeyboardButton>> inlineKeyboard) {
            InlineKeyboard = inlineKeyboard;
        }

        public static InlineKeyboardMarkupBuilder Builder() {
            return new InlineKeyboardMarkupBuilder();
        }

        public class InlineKeyboardMarkupBuilder : GridBuilder<InlineKeyboardButton, InlineKeyboardMarkup> {
            protected override InlineKeyboardMarkup MapResult(IEnumerable<IEnumerable<InlineKeyboardButton>> result) {
                return new InlineKeyboardMarkup(result);
            }
        }

    }
}