using System;
using Microsoft.Extensions.Logging;
using Teleglib.Utils;

namespace Teleglib.Polling {
    public interface IAutoPollerConfiguration {
        ILoggerFactory LoggerFactory { get; }
        int? OneTimeLimit { get; }
        TimeSpan? PoolingTimeout { get; }
        string[] FieldsFilter { get; }
    }
}