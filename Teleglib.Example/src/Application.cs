using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Teleglib.Bot;
using Teleglib.Controllers;
using Teleglib.Router;

namespace Teleglib.Example {
    public class Application : BotApplication {
        public Application(IServiceCollection services) : base(services) {
        }

        protected override IServiceProvider CreateServiceProvider(IServiceCollection serviceCollection) {
            return serviceCollection.BuildServiceProvider();
        }

        protected override void ConfigureServices(IServiceCollection services) {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddConsole(
                includeScopes: true,
                minLevel: LogLevel.Trace
            );
            services.AddSingleton<ILoggerFactory>(loggerFactory);

            services.AddControllers(b =>
                b.SetAssembly(typeof(Application).GetTypeInfo().Assembly)
                 .AddNamespace("Teleglib.Example.Controllers")
                 .GenerateRoutes()
            );
        }

        protected override void Configure(ApplicationBuilder builder) {
            builder
                .ConfigureTelegramClient(c => c.SetToken(""))
                .EnableAutoPolling()
                .ConfigureMiddlewares(m => m
                    .InsertLast<RouterMiddleware>()
                    .InsertLast<ControllersMiddleware>()
                );
        }
    }
}