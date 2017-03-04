using Teleglib.Features;
using Teleglib.Renderers;

namespace Teleglib.Middlewares {
    public static class MiddlewareDataExtensions {

        public static MiddlewareData AddResponseRenderer(this MiddlewareData data, string text) {
            var chatId = data.Features.RequireOne<UpdateInfoFeature>().GetAnyMessage().Chat.Id;
            return data.AddRenderer(new SendMessageRenderer(chatId.ToString(), text));
        }

    }
}