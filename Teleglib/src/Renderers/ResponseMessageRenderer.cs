using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Teleglib.Features;
using Teleglib.Middlewares;
using Teleglib.Session;
using Teleglib.Telegram.Client;
using Teleglib.Telegram.Models;

namespace Teleglib.Renderers {
    public class ResponseMessageRenderer : IClientRenderer {
        private readonly MessageData _messageData;

        public ResponseMessageRenderer(MessageData messageData) {
            _messageData = messageData;
        }

        public async Task<MessageContext> Render(MiddlewareData middlewareData, ITelegramClient telegramClient, CancellationToken cancellationToken) {
            var context = middlewareData.Features.RequireOne<SessionFeature>().Context;
            var message = middlewareData.Features.RequireOne<UpdateInfoFeature>().GetAnyMessage();

            if (message == null) {
                throw new InvalidOperationException("Cannot send response");
            }

            if (context.SentMessageIds.Any()) {
                await telegramClient.UpdateMessage(new UpdateMessageData(
                    message.Chat.Id.ToString(),
                    context.SentMessageIds.First(),
                    _messageData
                ), cancellationToken);
                return MessageContext.Empty();
            }

            var chatId = message.Chat.Id.ToString();
            var newMessage = await telegramClient.SendMessage(new SendMessageData(
                chatId, _messageData
            ), cancellationToken);

            return MessageContext.Builder()
                .AddMessageId(newMessage.Id)
                .Build();
        }
    }
}