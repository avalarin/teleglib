using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

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
                return data.AddResponseRenderer($"Error occured: [{e.GetType().Name}] {e.Message}");
            }
        }
    }
}