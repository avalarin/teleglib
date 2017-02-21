using Teleglib.Features;
using Teleglib.Renderers;
using Teleglib.Telegram.Models;

namespace Teleglib.Middlewares {
    public static class MiddlewareDataExtensions {

        public static MiddlewareData AddResponseRenderer(this MiddlewareData data, SendMessageData messageData) {
            var incomeMessage = data.Features.RequireOne<UpdateInfoFeature>().Update.Message;
            var chatId = incomeMessage.Chat.Id.ToString();
            return data.AddRenderer(new SendMessageRenderer(messageData));
        }

    }
}