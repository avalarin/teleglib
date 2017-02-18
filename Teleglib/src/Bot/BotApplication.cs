using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Teleglib.Controllers;
using Teleglib.Features;
using Teleglib.Middlewares;
using Teleglib.Polling;
using Teleglib.Router;
using Teleglib.Telegram.Client;
using Teleglib.Telegram.Models;
using Teleglib.Utils;

namespace Teleglib.Bot {
    public abstract class BotApplication {
        private readonly IServiceCollection _services;
        private IServiceProvider _serviceProvider;
        private ITelegramClient _client;
        private AutoPoller _autoPoller;
        private IMiddlewaresChain _middlewaresChain;

        private Task _updatesProcessingTask;
        private CancellationTokenSource _cancellationTokenSource;
        private ILogger _logger;
        private readonly ConcurrentQueue<UpdateInfo> _updatesQueue;

        public BotApplication(IServiceCollection services) {
            _services = services;
            _updatesQueue = new ConcurrentQueue<UpdateInfo>();
        }

        public void Start() {
            _services.AddSingleton<IPollerHistoryStorage>(new TempPollerHistoryStorage(0));
            
            ConfigureServices(_services);

            _serviceProvider = CreateServiceProvider(_services);

            // check all required services
            _serviceProvider.GetRequiredService<IRouter>();
            _serviceProvider.GetRequiredService<IControllerFactory>();

            var loggerFactory = _serviceProvider.GetRequiredService<ILoggerFactory>();
            _logger = loggerFactory.CreateLogger(GetType());

            var builder = new ApplicationBuilder(_serviceProvider)
                .ConfigureMiddlewares(b => b.InsertFirst<HandleErrorMiddleware>())
                .ConfigureAutoPolling(c => c.SetLoggerFactory(loggerFactory))
                .ConfigureTelegramClient(c => c.SetLoggerFactory(loggerFactory));
            Configure(builder);

            var configuration = builder.Build();
            _client = new TelegramClient(configuration.ClientConfiguration);

            if (configuration.AutoPollingEnabled) {
                var pollerHistoryStorage = _serviceProvider.GetRequiredService<IPollerHistoryStorage>();
                _autoPoller = new AutoPoller(_client, pollerHistoryStorage, configuration.AutoPollerConfiguration);
                _autoPoller.UpdateReceived += (sender, args) => EnqueueUpdate(args.Update);
                _autoPoller.Start();

                _logger.LogInformation("Auto polling has been enabled...");
            }

            _middlewaresChain = configuration.Middlewares;

            _cancellationTokenSource = new CancellationTokenSource();
            _updatesProcessingTask = ProcessUpdates(_cancellationTokenSource.Token);

            _logger.LogInformation("Application started...");
        }

        private void EnqueueUpdate(UpdateInfo updateInfo) {
            _logger.LogTrace($"Update #{updateInfo.Id} enqueued");
            _updatesQueue.Enqueue(updateInfo);
        }

        private async Task ProcessUpdates(CancellationToken token) {
            await Task.Yield();

            while (!token.IsCancellationRequested) {
                UpdateInfo updateInfo;
                if (!_updatesQueue.TryDequeue(out updateInfo)) {
                    await Task.Delay(TimeSpan.FromSeconds(1), token);
                    continue;
                }

                try {
                    var inputData = new MiddlewareData()
                        .UpdateFeatures(f => f.AddExclusive<UpdateInfoFeature>(new UpdateInfoFeature(updateInfo)));

                    var outputData = await _middlewaresChain.NextAsync(inputData);

                    foreach (var renderer in outputData.Renderers) {
                        await renderer.Render(_client, CancellationToken.None);
                    }

                }
                catch (Exception e) {
                    _logger.LogError(0, e, "Unhandled exception occured");
                }
            }
        }


        protected abstract IServiceProvider CreateServiceProvider(IServiceCollection serviceCollection);

        protected abstract void ConfigureServices(IServiceCollection services);

        protected abstract void Configure(ApplicationBuilder builder);

    }
}