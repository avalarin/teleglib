using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
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

namespace Teleglib {

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

        protected BotApplication(IServiceCollection services) {
            _services = services;
            _updatesQueue = new ConcurrentQueue<UpdateInfo>();
        }

        public void Start() {
            try {
                _services.AddSingleton<IPollerHistoryStorage>(new TempPollerHistoryStorage(0));
                _services.AddSingleton<ITelegramClient, TelegramClient>();
                _services.AddSingleton<IClientConfiguration, ClientAutoConfiguration>();
                _services.AddSingleton<IAutoPollerConfiguration, AutoPollerAutoConfiguration>();
                _services.AddSingleton<IMiddlewaresChainFactory, MiddlewaresChainFactory>();

                var middlewaresChainBuilder = new MiddlewaresChainBuilder();
                middlewaresChainBuilder.InsertFirst<HandleErrorMiddleware>();
                _services.AddSingleton(middlewaresChainBuilder);

                _services.AddControllers(b =>
                    b.SetAssembly(GetType().GetTypeInfo().Assembly)
                        .GenerateRoutes()
                );

                ConfigureServices(_services);

                _serviceProvider = CreateServiceProvider(_services);

                // check all required services
                _serviceProvider.GetRequiredService<IRouter>();
                _serviceProvider.GetRequiredService<IControllerFactory>();
                _client = _serviceProvider.GetRequiredService<ITelegramClient>();

                var loggerFactory = _serviceProvider.GetRequiredService<ILoggerFactory>();
                _logger = loggerFactory.CreateLogger(GetType());

                var autoPollerConfig = _serviceProvider.GetService<IAutoPollerConfiguration>();
                if ((autoPollerConfig as AutoPollerAutoConfiguration)?.Enabled ?? true) {
                    _autoPoller = _serviceProvider.GetInstance<AutoPoller>();
                    _autoPoller.UpdateReceived += (sender, args) => EnqueueUpdate(args.Update);
                    _autoPoller.Start();
                    _logger.LogInformation("Auto polling has been enabled...");
                }

                CallConfigure();

                var middlewaresChainFactory = _serviceProvider.GetRequiredService<IMiddlewaresChainFactory>();
                _middlewaresChain = middlewaresChainBuilder.Build(middlewaresChainFactory);
                _cancellationTokenSource = new CancellationTokenSource();
                _updatesProcessingTask = ProcessUpdates(_cancellationTokenSource.Token);

                _logger.LogInformation("Application started...");
            }
            catch (Exception e) {
                Console.WriteLine("Cannot start application: " + e.ToString());
                throw;
            }
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

                    await _middlewaresChain.NextAsync(inputData);
                }
                catch (Exception e) {
                    _logger.LogError(0, e, "Unhandled exception occured");
                }
            }
        }

        private void CallConfigure() {
            var configureMethods = GetType().GetTypeInfo()
                .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(m => m.Name.StartsWith("Configure") && !m.Name.Equals("ConfigureServices"));

            foreach (var configureMethod in configureMethods) {
                var parameters = configureMethod.GetParameters()
                    .Select(t => _serviceProvider.GetService(t.ParameterType))
                    .ToArray();
                configureMethod.Invoke(this, parameters);
            }
        }

        protected abstract IServiceProvider CreateServiceProvider(IServiceCollection serviceCollection);

        protected abstract void ConfigureServices(IServiceCollection services);

    }
}