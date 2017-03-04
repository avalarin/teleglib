using System.Threading;
using System.Threading.Tasks;
using Teleglib.Middlewares;
using Teleglib.Session;
using Teleglib.Telegram.Client;
using Teleglib.Telegram.Models;

namespace Teleglib.Renderers {
    public class SendMessageRenderer : IClientRenderer {
        private readonly string _chatId;
        private readonly string _text;

        public SendMessageRenderer(string chatId, string text) {
            _chatId = chatId;
            _text = text;
        }

        public async Task<MessageContext> Render(MiddlewareData middlewareData, ITelegramClient telegramClient, CancellationToken cancellationToken) {
            var newMessage = await telegramClient.SendMessage(new SendMessageData() {
                ChatId = _chatId,
                Text = _text
            }, cancellationToken);

            return MessageContext.Builder()
                .AddMessageId(newMessage.Id)
                .Build();
        }
    }
}