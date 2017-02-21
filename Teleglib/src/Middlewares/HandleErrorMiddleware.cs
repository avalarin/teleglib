using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Teleglib.Features;
using Teleglib.Renderers;
using Teleglib.Telegram.Models;

namespace Teleglib.Middlewares {
    public class HandleErrorMiddleware : IMiddleware {
        private readonly ILogger _logger;

        public HandleErrorMiddleware(ILoggerFactory loggerFactory) {
            _logger = loggerFactory.CreateLogger(GetType());
        }

        public async Task<MiddlewareData> InvokeAsync(MiddlewareData data, IMiddlewaresChain chain) {
            try {
                return await chain.NextAsync(data);
            }
            catch (Exception e) {
                _logger.LogError(0, e, "Unexpected error occurred");
                return data.AddResponseRenderer(new SendMessageData() {
                    Text = $"Error occured: [{e.GetType().Name}] {e.Message}"
                });
            }
        }
    }
}