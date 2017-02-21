using System;
using Microsoft.Extensions.Configuration;
using Teleglib.Utils;

namespace Teleglib.Polling {
    public class AutoPollerAutoConfiguration : IAutoPollerConfiguration {

        private readonly IConfiguration _configuration;

        public AutoPollerAutoConfiguration(IConfiguration configuration) {
            _configuration = configuration.GetSection("Teleglib");
        }

        public bool Enabled => _configuration.Get<bool?>("AutoPoller:Enabled") ?? false;

        public int? OneTimeLimit => _configuration.Get<int?>("AutoPoller:OneTimeLimit");

        public TimeSpan? PoolingTimeout => _configuration.Get<TimeSpan?>("AutoPoller:PoolingTimeout", str => TimeSpan.Parse(str));

        public string[] FieldsFilter => null;

    }
}