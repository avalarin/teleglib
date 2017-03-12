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
        private readonly MessageData _messageData;

        public UpdateMessageRenderer(string chatId, long messageId, MessageData messageData) {
            _chatId = chatId;
            _messageId = messageId;
            _messageData = messageData;
        }

        public async Task<MessageContext> Render(MiddlewareData middlewareData, ITelegramClient telegramClient, CancellationToken cancellationToken) {
            var newMessage = await telegramClient.UpdateMessage(new UpdateMessageData(
                    chatId: _chatId,
                    messageId: _messageId,
                    messageData: _messageData
            ), cancellationToken);

            return MessageContext.Builder()
                .AddMessageId(newMessage.Id)
                .Build();
        }
    }
}