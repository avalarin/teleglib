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
        private string Text { get;  }

        public ResponseMessageRenderer(string text) {
            Text = text;
        }

        public async Task<MessageContext> Render(MiddlewareData middlewareData, ITelegramClient telegramClient, CancellationToken cancellationToken) {
            var update = middlewareData.Features.RequireOne<UpdateInfoFeature>().Update;
            var context = middlewareData.Features.RequireOne<SessionFeature>().Context;

            if (update.EditedMessage != null && context.SentMessageIds.Any()) {
                await telegramClient.UpdateMessage(new UpdateMessageData() {
                    ChatId = update.EditedMessage.Chat.Id.ToString(),
                    MessageId = context.SentMessageIds.First(),
                    Text = Text
                }, cancellationToken);
                return MessageContext.Empty();
            }

            var message = update.Message ?? update.EditedMessage;

            if (message == null) {
                throw new InvalidOperationException("Cannot send response");
            }

            var chatId = message.Chat.Id;
            var newMessage = await telegramClient.SendMessage(new SendMessageData() {
                ChatId = message.Chat.Id.ToString(),
                Text = Text
            }, cancellationToken);

            return MessageContext.Builder()
                .AddMessageId(newMessage.Id)
                .Build();
        }
    }
}