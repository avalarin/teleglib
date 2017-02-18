using System.Threading;
using System.Threading.Tasks;
using Teleglib.Telegram.Client;
using Teleglib.Telegram.Models;

namespace Teleglib.Renderers {
    public class SendMessageRenderer : IClientRenderer {

        private readonly SendMessageData _sendMessageData;

        public SendMessageRenderer(SendMessageData sendMessageData) {
            _sendMessageData = sendMessageData;
        }

        public async Task Render(ITelegramClient client, CancellationToken cancellationToken) {
            await client.SendMessage(_sendMessageData, cancellationToken);
        }
    }
}