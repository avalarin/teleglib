using System;
using Microsoft.Extensions.DependencyInjection;
using Teleglib.Controllers.Actions;
using Teleglib.Router;

namespace Teleglib.Controllers {
    public static class ControllersServicesExtensions {

        public static IServiceCollection AddControllers(this IServiceCollection services,
            Action<ControllerFactoryConfigurationBuilder> configurator) {

            var builder = new ControllerFactoryConfigurationBuilder();
            configurator(builder);
            services.AddSingleton<IControllerFactoryConfiguration>(builder.Build());
            services.AddSingleton<IActionParametersMapper, ActionParametersMapper>();
            services.AddSingleton<IControllerFactory, ControllerFactory>();
            services.AddRouter();

            return services;
        }

    }
}