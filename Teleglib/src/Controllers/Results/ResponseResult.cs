using Teleglib.Middlewares;
using Teleglib.Telegram.Models;

namespace Teleglib.Controllers.Results {
    public class ResponseResult : IActionResult {

        public string Text { get; set; }

        public MiddlewareData Render(MiddlewareData input) {
            return input.AddResponseRenderer(Text);
        }

    }
}