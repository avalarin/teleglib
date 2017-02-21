using System;
using Microsoft.Extensions.Logging;

namespace Teleglib.Telegram.Client {
    public class ClientConfigurationBuilder {
        private string _token;
        private TimeSpan? _requestTimeout;
        private TimeSpan? _defaultGetUpdatesTimeout;

        private ClientConfigurationBuilder() {
            _token = null;
            _requestTimeout = null;
            _defaultGetUpdatesTimeout = null;
        }

        public ClientConfigurationBuilder SetToken(string token) {
            if (token == null) throw new ArgumentNullException(nameof(token));
            _token = token;
            return this;
        }

        public ClientConfigurationBuilder SetRequestTimeout(TimeSpan requestTimeout) {
            _requestTimeout = requestTimeout;
            return this;
        }

        public ClientConfigurationBuilder SetGetUpdatesTimeout(TimeSpan getUpdatesTimeout) {
            _defaultGetUpdatesTimeout = getUpdatesTimeout;
            return this;
        }

        public IClientConfiguration Build() {
            return new Configuration(_token, _requestTimeout, _defaultGetUpdatesTimeout);
        }

        public static ClientConfigurationBuilder Create() {
            return new ClientConfigurationBuilder();
        }

        private class Configuration : IClientConfiguration {
            public string Token { get; }
            public TimeSpan? RequestTimeout { get; }
            public TimeSpan? DefaultGetUpdatesTimeout { get; }

            public Configuration(string token, TimeSpan? requestTimeout, TimeSpan? defaultGetUpdatesTimeout) {
                Token = token;
                RequestTimeout = requestTimeout;
                DefaultGetUpdatesTimeout = defaultGetUpdatesTimeout;
            }
        }

    }
}