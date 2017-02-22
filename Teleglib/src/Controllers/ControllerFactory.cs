using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Teleglib.Controllers.Actions;
using Teleglib.Controllers.Attributes;
using Teleglib.Middlewares;
using Teleglib.Router;
using Teleglib.Router.Patterns;
using Teleglib.Utils;

namespace Teleglib.Controllers {
    public class ControllerFactory : IControllerFactory {
        private static readonly Regex ControllerNameRegex = new Regex(@"^(\w+)Controller$", RegexOptions.Compiled);

        private readonly IActionParametersMapper _actionParametersMapper;
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, TypeInfo> _types;

        public ControllerFactory(IControllerFactoryConfiguration configuration, IActionParametersMapper actionParametersMapper,
            IServiceProvider serviceProvider) {
            _actionParametersMapper = actionParametersMapper;
            _serviceProvider = serviceProvider;

            var namespaces = new HashSet<string>(configuration.Namespaces);
            var assembly = configuration.Assembly;

            _types = assembly.GetTypes()
                .Select(t => t.GetTypeInfo())
                .Where(t => t.IsClass && (!namespaces.Any() || namespaces.Contains(t.Namespace)))
                .Select(type => new {type, match = ControllerNameRegex.Match(type.Name)})
                .Where(t => t.match.Success)
                .ToDictionary(t => t.match.Groups[1].Value, t => t.type, StringComparer.OrdinalIgnoreCase);

            if (configuration.GenerateRoutes) {
                var router = _serviceProvider.GetService<IRouter>();
                if (router == null) {
                    throw new InvalidOperationException("Cannot generate routes, IRouter is not registred");
                }
                var controllerType = typeof(IController).GetTypeInfo();
                foreach (var kvp in _types) {
                    var type = kvp.Value;
                    if (controllerType.IsAssignableFrom(type)) continue;
                    var routeAttribute = type.GetCustomAttribute<RouteAttribute>();
                    var routes = ActionsUtils.GetActions(type.UnderlyingSystemType)
                        .Select(method => new {method, attribute = method.GetCustomAttribute<RouteAttribute>()})
                        .Where(t => t.attribute != null)
                        .Select(t => CreateRoute(kvp.Key, t.method, t.attribute, routeAttribute));
                    foreach (var route in routes) {
                        router.RegisterRoute(route);
                    }
                }
            }
        }

        private static IRoute CreateRoute(string controller, MethodInfo method, RouteAttribute actionAttribute, RouteAttribute controllerAttribute) {
            var name = controller + ":" + method.Name;
            if (!actionAttribute.Pattern.StartsWith("/") && controllerAttribute == null) {
                throw new InvalidOperationException($"Cannot register route {name}");
            }

            var defaults = new Dictionary<string, string>();
            defaults["controller"] = controller;
            defaults["action"] = method.Name;

            var pattern = actionAttribute.Pattern;
            if (controllerAttribute != null && !pattern.StartsWith("/")) {
                pattern = controllerAttribute.Pattern + ":" + pattern;
                if (controllerAttribute.Defaults != null) {
                    foreach (var kvp in controllerAttribute.Defaults.GetProperties()) {
                        defaults[kvp.Key] = kvp.Value.ToString();
                    }
                }
                if (actionAttribute.Defaults != null) {
                    foreach (var kvp in actionAttribute.Defaults.GetProperties()) {
                        defaults[kvp.Key] = kvp.Value.ToString();
                    }
                }
            }
            else {
                actionAttribute.Defaults?.GetProperties()
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.ToString())
                    .MergeTo(defaults);
            }

            return new PatternsRoute(name, pattern, defaults, actionAttribute.Details);
        }

        public IController GetController(MiddlewareData input) {
            var fields = input.Features.RequireOne<RouterFeature>().Fields;
            var controller = fields["controller"];

            TypeInfo type;
            if (!_types.TryGetValue(controller, out type)) {
                throw new Exception("Controller not found");
            }

            var instance = _serviceProvider.GetInstance(type.UnderlyingSystemType);

            return instance as IController ??
                   new ActionsController(_actionParametersMapper, instance);
        }

    }
}