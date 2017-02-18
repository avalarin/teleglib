using System;
using Teleglib.Middlewares;
using Teleglib.Polling;
using Teleglib.Telegram.Client;

namespace Teleglib.Bot {
    public class ApplicationBuilder {
        private readonly IServiceProvider _serviceProvider;

        private readonly ClientConfigurationBuilder _clientConfigurationBuilder;
        private readonly MiddlewaresChainBuilder _middlewaresChainBuilder;
        private readonly AutoPollerConfigurationBuilder _autoPollerConfigurationBuilder;
        private bool _autoPollerEnabled;

        public ApplicationBuilder(IServiceProvider serviceProvider) {
            _serviceProvider = serviceProvider;
            _clientConfigurationBuilder = ClientConfigurationBuilder.Create();
            _middlewaresChainBuilder = new MiddlewaresChainBuilder();
            _autoPollerConfigurationBuilder = AutoPollerConfigurationBuilder.Create();
            _autoPollerEnabled = false;
        }

        public IServiceProvider ServiceProvider => _serviceProvider;

        public ApplicationBuilder ConfigureTelegramClient(
            Action<ClientConfigurationBuilder> configurator) {

            configurator(_clientConfigurationBuilder);
            return this;
        }

        public ApplicationBuilder ConfigureAutoPolling(
            Action<AutoPollerConfigurationBuilder> configurator) {

            configurator(_autoPollerConfigurationBuilder);
            return this;
        }

        public ApplicationBuilder EnableAutoPolling() {
            _autoPollerEnabled = true;
            return this;
        }

        public ApplicationBuilder ConfigureMiddlewares(
            Action<MiddlewaresChainBuilder> configurator) {

            configurator(_middlewaresChainBuilder);
            return this;
        }

        public IApplicationConfiguration Build() {
            return new Configuration(
                _clientConfigurationBuilder.Build(),
                _middlewaresChainBuilder.Build(_serviceProvider),
                _autoPollerConfigurationBuilder.Build(),
                _autoPollerEnabled
            );
        }

        private class Configuration : IApplicationConfiguration {

            public IClientConfiguration ClientConfiguration { get; }
            public IAutoPollerConfiguration AutoPollerConfiguration { get; }
            public IMiddlewaresChain Middlewares { get; }
            public bool AutoPollingEnabled { get; }

            public Configuration(IClientConfiguration clientConfiguration, IMiddlewaresChain middlewares,
                                 IAutoPollerConfiguration autoPollerConfiguration, bool autoPollerEnabled) {

                ClientConfiguration = clientConfiguration;
                AutoPollerConfiguration = autoPollerConfiguration;
                Middlewares = middlewares;
                AutoPollingEnabled = autoPollerEnabled;
            }
        }

    }
}