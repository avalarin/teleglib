using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Teleglib.Telegram.Client;

namespace Teleglib.Polling {
    public class AutoPoller {
        private static readonly TimeSpan DefaultPollingTimeout = TimeSpan.FromSeconds(3);
        private const int DefaultOneTimeLimit = 100;

        private readonly object _lock = new object();

        private readonly ILogger _logger;
        private readonly ITelegramClient _client;
        private readonly IPollerHistoryStorage _historyStorage;

        private readonly int _oneTimeLimit;
        private readonly TimeSpan _poolingTimeout;
        private readonly string[] _fieldsFilter;

        private long _offset;
        private Task _task;
        private CancellationTokenSource _cancellationTokenSource;

        public AutoPoller(ITelegramClient client, IPollerHistoryStorage historyStorage, IAutoPollerConfiguration configuration, ILoggerFactory loggerFactory) {
            _client = client;
            _historyStorage = historyStorage;


            _logger = loggerFactory.CreateLogger(GetType());
            _oneTimeLimit = configuration.OneTimeLimit ?? DefaultOneTimeLimit;
            _poolingTimeout = configuration.PoolingTimeout ?? DefaultPollingTimeout;
            _fieldsFilter = configuration.FieldsFilter;
        }

        public event EventHandler<UpdateEventArgs> UpdateReceived;

        public void Start() {
            if (_task != null) throw new InvalidOperationException("Already enabled");
            lock (_lock) {
                if (_task != null) throw new InvalidOperationException("Already enabled");
                _cancellationTokenSource = new CancellationTokenSource();
                _task = Pooling(_cancellationTokenSource.Token);
                _logger.LogInformation("Auto polling task has been started");
            }
        }

        public void Stop(TimeSpan? timeout = null) {
            _logger.LogInformation("Auto polling task is stopping...");
            _cancellationTokenSource.Cancel();
            _task.Wait();
        }

        private async Task Pooling(CancellationToken cancellationToken) {
            await Task.Yield();

            _offset = (await _historyStorage.GetLastUpdateId()) + 1;

            while (!cancellationToken.IsCancellationRequested) {
                try {
                    var results = await _client.GetUpdates(_offset, _oneTimeLimit, _poolingTimeout, _fieldsFilter,
                        cancellationToken);
                    foreach (var update in results) {
                        _logger.LogDebug($"Process update #{update.Id}: ${JsonConvert.SerializeObject(update)}");
                        OnUpdateReceived(new UpdateEventArgs(update));

                        _offset = update.Id + 1;
                        await _historyStorage.SaveLastUpdateId(update.Id);
                    }
                }
                catch (Exception e) {
                    _logger.LogError(0, e, "Error occurred while polling");
                }
            }
        }

        protected virtual void OnUpdateReceived(UpdateEventArgs e) {
            UpdateReceived?.Invoke(this, e);
        }
    }
}