using System;

namespace Teleglib.Polling {
    public interface IAutoPollerConfiguration {
        int? OneTimeLimit { get; }
        TimeSpan? PoolingTimeout { get; }
        string[] FieldsFilter { get; }
    }
}