using System;
using Microsoft.Extensions.DependencyInjection;

namespace Teleglib.Router {
    public static class RouterServicesExtensions {
        public static IServiceCollection AddRouter(this IServiceCollection services) {
            return AddRouter(services, b => {});
        }

        public static IServiceCollection AddRouter(this IServiceCollection services,
            Action<RouterConfigurationBuilder> configurator) {

            var builder = new RouterConfigurationBuilder();
            configurator(builder);
            services.AddSingleton<IRouterConfiguration>(builder.Build());
            services.AddSingleton<IRouter, Router>();

            return services;
        }

    }
}