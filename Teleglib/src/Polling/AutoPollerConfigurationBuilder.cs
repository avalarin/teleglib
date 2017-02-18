using System;
using Microsoft.Extensions.Logging;

namespace Teleglib.Polling {
    public class AutoPollerConfigurationBuilder {
        private ILoggerFactory _loggerFactory;
        private int? _oneTimeLimit;
        private TimeSpan? _poolingTimeout;
        private string[] _fieldsFilter;

        private AutoPollerConfigurationBuilder() {
            _loggerFactory = null;
            _fieldsFilter = null;
            _oneTimeLimit = null;
            _poolingTimeout = null;
        }

        public AutoPollerConfigurationBuilder SetLoggerFactory(ILoggerFactory loggerFactory) {
            if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));
            _loggerFactory = loggerFactory;
            return this;
        }

        public AutoPollerConfigurationBuilder SetOneTimeLimit(int oneTimeLimit) {
            _oneTimeLimit = oneTimeLimit;
            return this;
        }

        public AutoPollerConfigurationBuilder SetPoolingTimeout(TimeSpan poolingTimeout) {
            _poolingTimeout = poolingTimeout;
            return this;
        }

        public AutoPollerConfigurationBuilder SetFieldsFilter(string[] fieldsFilter) {
            if (fieldsFilter == null) throw new ArgumentNullException(nameof(fieldsFilter));
            _fieldsFilter = fieldsFilter;
            return this;
        }

        public IAutoPollerConfiguration Build() {
            return new Configuration(_loggerFactory, _oneTimeLimit, _poolingTimeout, _fieldsFilter);
        }

        public static AutoPollerConfigurationBuilder Create() {
            return new AutoPollerConfigurationBuilder();
        }

        private class Configuration : IAutoPollerConfiguration {
            public ILoggerFactory LoggerFactory { get; }
            public int? OneTimeLimit { get; }
            public TimeSpan? PoolingTimeout { get; }
            public string[] FieldsFilter { get; }

            public Configuration(ILoggerFactory loggerFactory, int? oneTimeLimit, TimeSpan? poolingTimeout, string[] fieldsFilter) {
                OneTimeLimit = oneTimeLimit;
                PoolingTimeout = poolingTimeout;
                FieldsFilter = fieldsFilter;
                LoggerFactory = loggerFactory;
            }
        }

    }
}