using System.Threading;
using System.Threading.Tasks;
using Teleglib.Middlewares;
using Teleglib.Session;
using Teleglib.Telegram.Client;
using Teleglib.Telegram.Models;

namespace Teleglib.Renderers {
    public class UpdateMessageRenderer : IClientRenderer {
        private readonly string _chatId;
        private readonly long _messageId;
        private readonly string _text;

        public UpdateMessageRenderer(string chatId, long messageId, string text) {
            _chatId = chatId;
            _messageId = messageId;
            _text = text;
        }

        public async Task<MessageContext> Render(MiddlewareData middlewareData, ITelegramClient telegramClient, CancellationToken cancellationToken) {
            var newMessage = await telegramClient.UpdateMessage(new UpdateMessageData() {
                ChatId = _chatId,
                MessageId = _messageId,
                Text = _text
            }, cancellationToken);

            return MessageContext.Builder()
                .AddMessageId(newMessage.Id)
                .Build();
        }
    }
}