using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Teleglib.Features;
using Teleglib.Localization;
using Teleglib.Middlewares;
using Teleglib.Renderers;
using Teleglib.Session;
using Teleglib.Telegram.Models;
using Teleglib.Utils;

namespace Teleglib.Router {
    public class RouterMiddleware : IMiddleware {

        private static readonly Regex FindCommandRegex = new Regex(@"^\s*/(.+?)( .*|\Z)", RegexOptions.Compiled);

        private readonly IRouter _router;
        private readonly ISessionStorage _sessionStorage;
        private readonly LocalizationManager _localizationManager;
        private readonly ILogger _logger;

        public RouterMiddleware(
            IRouter router,
            ISessionStorage sessionStorage,
            ILoggerFactory loggerFactory,
            LocalizationManager localizationManager
        ) {
            _router = router;
            _sessionStorage = sessionStorage;
            _localizationManager = localizationManager;
            _logger = loggerFactory.CreateLogger<RouterMiddleware>();
        }

        public async Task<MiddlewareData> InvokeAsync(MiddlewareData data, IMiddlewaresChain chain) {
            var userPreferences = data.Features.GetOne<UserPreferencesFeature>()
                .Map(f => f.UserPreferences)
                .OrElse(DefaultUserPreferences.Instance);

            var routingData = ParseRoutingData(data);
            var prevRoutingData = await LoadSavedRoutingData(data);
            if (prevRoutingData != null) {
                routingData = JoinRoutingData(prevRoutingData, routingData);
            }

            var dataWithContext = data.UpdateFeatures(f => f.ReplaceExclusive<SessionFeature>(
                    e => e.SetRoutingData(prevRoutingData)
                )
            );

            _logger.LogDebug($"Try find route for {routingData.Path} ({routingData.UserCommand})");

            var match = _router.FindRoute(routingData);
            var chatId = dataWithContext.Features.RequireOne<UpdateInfoFeature>().GetAnyMessage().Chat.Id;
            if (!match.IsMatched) {
                await _sessionStorage.ClearRoutingDataAsync(chatId);
                var message =_localizationManager.Localize(
                    userPreferences: userPreferences,
                    key: LocalizationKeys.UnknownCommand,
                    parameters: new {text = routingData.UserCommand}
                );
                return dataWithContext.AddResponseRenderer(message);
            }

            if (!match.IsCompleted) {
                await _sessionStorage.SaveRoutingDataAsync(chatId, routingData);

                var responseMessage = MessageData.Builder()
                    .SetText(_localizationManager.Localize(userPreferences, LocalizationKeys.SelectOneForCompletion))
                    .SetInlineKeyboardMarkup(bldr => BuildResponseMarkup(bldr, match.UncompletedRoutes))
                    .Build();

                return dataWithContext.AddResponseRenderer(responseMessage);
            }
            await _sessionStorage.ClearRoutingDataAsync(chatId);

            var route = match.CompletedRoute;
            var feature = new RouterFeature(route.Route, routingData, route.Fields);
            var dataWithRoute = dataWithContext.UpdateFeatures(f => f.AddExclusive<RouterFeature>(feature));

            return await chain.NextAsync(dataWithRoute);
        }

        private async Task<RoutingData> LoadSavedRoutingData(MiddlewareData data) {
            var message = data.Features.RequireOne<UpdateInfoFeature>().GetAnyMessage();
            var chatId = message.Chat.Id;
            var messageId = message.Id;

            var context = await _sessionStorage.LoadMessageContextAsync(chatId, messageId);
            if (context != null) {
                return context.RoutingData;
            }

            return await _sessionStorage.GetRoutingDataAsync(chatId);
        }

        private static RoutingData ParseRoutingData(MiddlewareData data) {
            var updateFeature = data.Features.RequireOne<UpdateInfoFeature>();
            string messageText;
            if (updateFeature.Update.CallbackQuery != null) {
                var callback = updateFeature.Update.CallbackQuery;
                messageText = callback.Data;
            }
            else {
                var message = updateFeature.GetAnyMessage();
                messageText = message.Text;
            }
            var match = FindCommandRegex.Match(messageText);
            if (!match.Success) {
                return new RoutingData("", "", new string[0], messageText);
            }
            var path = match.Groups[1].Value;
            var parts = path.Split(':');
            var content = match.Groups[2].Value;
            return new RoutingData(path, path, parts, content);
        }

        private static RoutingData JoinRoutingData(RoutingData prev, RoutingData next) {
            var newParts = new string[prev.PathParts.Length + next.PathParts.Length];
            Array.Copy(prev.PathParts, 0, newParts, 0, prev.PathParts.Length);
            Array.Copy(next.PathParts, 0, newParts, prev.PathParts.Length, next.PathParts.Length);
            return new RoutingData(next.UserCommand, prev.Path + ":" + next.Path, newParts, next.Content, prev);
        }

        void BuildResponseMarkup(InlineKeyboardMarkup.InlineKeyboardMarkupBuilder bldr, IEnumerable<RouteMatch> routes) {
            routes.Select(ur => new InlineKeyboardButton(ur.CompletionData.HintText + " " + ur.CompletionData.Path) {
                    CallbackData = ur.CompletionData.Path
                })
                .ForEach(btn => bldr.AddRow(rbldr => rbldr.AddItem(btn)));
        }
    }
}