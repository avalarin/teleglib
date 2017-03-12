using System;
using Teleglib.Renderers;

namespace Teleglib.Middlewares {
    public static class MiddlewareDataExtensions {

        public static MiddlewareData AddResponseRenderer(this MiddlewareData data, string text) {
            return data.AddRenderer(new ResponseMessageRenderer(new MessageData() { Text = text } ));
        }

        public static MiddlewareData AddResponseRenderer(this MiddlewareData data, MessageData messageData) {
            return data.AddRenderer(new ResponseMessageRenderer(messageData));
        }

        public static MiddlewareData AddResponseRenderer(this MiddlewareData data, Action<MessageData.MessageDataBuilder> action) {
            var builder = MessageData.Builder();
            action(builder);
            return data.AddRenderer(new ResponseMessageRenderer(builder.Build()));
        }

    }
}