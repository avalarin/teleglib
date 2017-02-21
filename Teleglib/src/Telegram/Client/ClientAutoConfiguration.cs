using System;
using Microsoft.Extensions.Configuration;
using Teleglib.Utils;

namespace Teleglib.Telegram.Client {
    public class ClientAutoConfiguration : IClientConfiguration {
        private readonly IConfiguration _configuration;

        public ClientAutoConfiguration(IConfiguration configuration) {
            _configuration = configuration.GetSection("Teleglib:Client");
        }

        public string Token => _configuration["Token"];

        public TimeSpan? RequestTimeout => _configuration.Get<TimeSpan?>("RequestTimeout", str => TimeSpan.Parse(str));

        public TimeSpan? DefaultGetUpdatesTimeout => _configuration.Get<TimeSpan?>("DefaultGetUpdatesTimeout", str => TimeSpan.Parse(str));
    }
}