using System;
using Teleglib.Telegram.Models;

namespace Teleglib.Renderers {
    public class MessageData {
        public MessageData() {
        }

        public MessageData(string text) {
            Text = text;
        }

        public string Text { get; set; }

        public InlineKeyboardMarkup ReplyMarkup { get; set; }

        public static MessageDataBuilder Builder() {
            return new MessageDataBuilder();
        }

        public class MessageDataBuilder {

            private string _text;
            private InlineKeyboardMarkup _replyMarkup;

            public MessageDataBuilder SetText(string text) {
                _text = text;
                return this;
            }

            public MessageDataBuilder SetInlineKeyboardMarkup(Action<InlineKeyboardMarkup.InlineKeyboardMarkupBuilder> action) {
                var builder = InlineKeyboardMarkup.Builder();
                action(builder);
                _replyMarkup = builder.Build();
                return this;
            }

            public MessageData Build() {
                return new MessageData() {
                    Text = _text,
                    ReplyMarkup = _replyMarkup
                };
            }

        }

    }
}