using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Teleglib.Controllers;
using Teleglib.Middlewares;
using Teleglib.Router;

namespace Teleglib.Example {
    public class Application : BotApplication {
        private readonly IConfiguration _configuration;

        public Application(string configFile = "config.json") : base(new ServiceCollection()) {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile(configFile, optional: true)
                .Build();
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
            services.AddSingleton<IConfiguration>(_configuration);
        }

        private void ConfigureMiddlewares(MiddlewaresChainBuilder chainBuilder) {
            chainBuilder
                .InsertLast<RouterMiddleware>()
                .InsertLast<ControllersMiddleware>();
        }
    }
}