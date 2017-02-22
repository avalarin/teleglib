using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Teleglib.Features;
using Teleglib.Middlewares;
using Teleglib.Storage;
using Teleglib.Telegram.Models;

namespace Teleglib.Router {
    public class RouterMiddleware : IMiddleware {

        private static readonly Regex FindCommandRegex = new Regex(@"^\s*/(.+?)( .*|\Z)", RegexOptions.Compiled);

        private readonly IRouter _router;
        private readonly ISessionStorage _sessionStorage;
        private readonly ILogger _logger;

        public RouterMiddleware(IRouter router, ISessionStorage sessionStorage, ILoggerFactory loggerFactory) {
            _router = router;
            _sessionStorage = sessionStorage;
            _logger = loggerFactory.CreateLogger<RouterMiddleware>();
        }

        public async Task<MiddlewareData> InvokeAsync(MiddlewareData data, IMiddlewaresChain chain) {
            var routingData = ParseRoutingData(data);
            var prevRoutingData = await LoadSavedRoutingData(data);
            if (prevRoutingData != null) {
                routingData = JoinRoutingData(prevRoutingData, routingData);
            }

            _logger.LogDebug($"Try find route for {routingData.Path} ({routingData.UserCommand})");

            var match = _router.FindRoute(routingData);
            if (!match.IsMatched) {
                await _sessionStorage.SetAsync(SessionStorageKeys.PreviousRoutingData, null);
                return data.AddResponseRenderer(new SendMessageData() {
                    Text = $"Unknown command {routingData.UserCommand}"
                });
            }

            if (!match.IsCompleted) {
                await _sessionStorage.SetAsync(SessionStorageKeys.PreviousRoutingData, routingData);
                var links = match.UncompletedRoutes.Select(r => r.CompletionText);
                return data.AddResponseRenderer(new SendMessageData() {
                    Text = string.Join("\n", links)
                });
            }
            await _sessionStorage.SetAsync(SessionStorageKeys.PreviousRoutingData, null);

            var route = match.CompletedRoute;
            var feature = new RouterFeature(route.Route, route.Fields);
            var newData = data.UpdateFeatures(f => f.AddExclusive<RouterFeature>(feature));

            return await chain.NextAsync(newData);
        }

        private async Task<RoutingData> LoadSavedRoutingData(MiddlewareData data) {
            return await _sessionStorage.GetAsync(SessionStorageKeys.PreviousRoutingData);
        }

        private static RoutingData ParseRoutingData(MiddlewareData data) {
            var messageText = data.Features.RequireOne<UpdateInfoFeature>().Update.Message.Text;
            var match = FindCommandRegex.Match(messageText);
            if (!match.Success) {
                return new RoutingData("", "", new string[0], messageText);
            }
            var path = match.Groups[1].Value;
            var parts = path.Split(':');
            var content = match.Groups[2].Value;
            return new RoutingData(path, path, parts, content);
        }

        private RoutingData JoinRoutingData(RoutingData prev, RoutingData next) {
            var newParts = new string[prev.PathParts.Length + next.PathParts.Length];
            Array.Copy(prev.PathParts, 0, newParts, 0, prev.PathParts.Length);
            Array.Copy(next.PathParts, 0, newParts, prev.PathParts.Length, next.PathParts.Length);
            return new RoutingData(next.UserCommand, prev.Path + ":" + next.Path, newParts, next.Content);
        }
    }
}