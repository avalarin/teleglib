using Teleglib.Features;
using Teleglib.Middlewares;
using Teleglib.Renderers;
using Teleglib.Telegram.Models;

namespace Teleglib.Controllers.Results {
    public class ResponseResult : IActionResult {

        public string Text { get; set; }

        public MiddlewareData Render(MiddlewareData input) {
            var message = input.Features.RequireOne<UpdateInfoFeature>().Update.Message;
            var chatId = message.Chat.Id.ToString();

            var sendMessageData = new SendMessageData() {
                ChatId = chatId, Text = Text
            };

            var renderer = new SendMessageRenderer(sendMessageData);

            return input.AddRenderer(renderer);
        }

    }
}