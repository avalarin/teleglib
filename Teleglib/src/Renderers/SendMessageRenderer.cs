using System.Threading;
using System.Threading.Tasks;
using Teleglib.Middlewares;
using Teleglib.Session;
using Teleglib.Telegram.Client;
using Teleglib.Telegram.Models;

namespace Teleglib.Renderers {
    public class SendMessageRenderer : IClientRenderer {
        private readonly string _chatId;
        private readonly MessageData _messageData;

        public SendMessageRenderer(string chatId, MessageData messageData) {
            _chatId = chatId;
            _messageData = messageData;
        }

        public async Task<MessageContext> Render(MiddlewareData middlewareData, ITelegramClient telegramClient, CancellationToken cancellationToken) {
            var newMessage = await telegramClient.SendMessage(new SendMessageData(_chatId, _messageData), cancellationToken);

            return MessageContext.Builder()
                .AddMessageId(newMessage.Id)
                .Build();
        }
    }
}