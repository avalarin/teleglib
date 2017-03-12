using Teleglib.Middlewares;
using Teleglib.Renderers;

namespace Teleglib.Controllers.Results {
    public class ResponseResult : IActionResult {

        public MessageData MessageData { get; }

        public ResponseResult(string text) {
            MessageData = new MessageData(text);
        }

        public ResponseResult(MessageData messageData) {
            MessageData = messageData;
        }

        public MiddlewareData Render(MiddlewareData input) {
            return input.AddResponseRenderer(MessageData);
        }

    }
}