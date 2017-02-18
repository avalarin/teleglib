using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using Teleglib.Telegram.Exceptions;
using Teleglib.Telegram.Models;

namespace Teleglib.Telegram.Client {
    public class TelegramClient : ITelegramClient {
        private static readonly Regex ValidateTokenRegex = new Regex(@"^\d*:[\w\d-_]{35}$", RegexOptions.Compiled);
        private static readonly TimeSpan DefaultRequestTimeout = TimeSpan.FromSeconds(10);
        private static readonly TimeSpan DefaultGetUpdatesTimeout = TimeSpan.Zero;

        private readonly string _baseUrl = "https://api.telegram.org/bot";
        private readonly Encoding _encoding = Encoding.UTF8;

        private readonly ILogger _logger;
        private readonly Uri _uriWithToken;
        private readonly string _token;
        private readonly TimeSpan _requestTimeout;
        private readonly TimeSpan _defaultGetUpdatesTimeout;

        private readonly JsonSerializer _serializer;

        public TelegramClient(IClientConfiguration configuration) {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (string.IsNullOrWhiteSpace(configuration.Token)) throw new ArgumentException("Token required");
            if (!ValidateTokenRegex.IsMatch(configuration.Token)) throw new ArgumentException("Invalid token");

            _token = configuration.Token;
            _uriWithToken = new Uri(_baseUrl + _token + "/");

            _requestTimeout = configuration.RequestTimeout ?? DefaultRequestTimeout;
            _defaultGetUpdatesTimeout = configuration.DefaultGetUpdatesTimeout ?? DefaultGetUpdatesTimeout;
            _logger = configuration.LoggerFactory?.CreateLogger(GetType()) ?? NullLogger.Instance;

            _serializer = new JsonSerializer();
        }

        public async Task<UserInfo> GetMe(CancellationToken cancellationToken = default(CancellationToken)) {
            return await SendRequestAsync<UserInfo>("getMe", cancellationToken: cancellationToken);
        }

        public async Task<ChatInfo> GetChat(string chatId,
            CancellationToken cancellationToken = default(CancellationToken)) {

            return await SendRequestAsync<ChatInfo>("getChat", new { chat_id = chatId }, cancellationToken);
        }

        public async Task<MessageInfo> SendMessage(SendMessageData data,
            CancellationToken cancellationToken = default(CancellationToken)) {

            return await SendRequestAsync<MessageInfo>("sendMessage", data, cancellationToken);
        }

        public async Task<IEnumerable<UpdateInfo>> GetUpdates(
            long offset = 0,
            int limit = 100,
            TimeSpan? timeout = null,
            string[] filter = null,
            CancellationToken cancellationToken = default(CancellationToken)) {

            var secondsTimeout = (int)(timeout ?? _defaultGetUpdatesTimeout).Seconds;
            return await SendRequestAsync<IEnumerable<UpdateInfo>>("getUpdates",
                new { offset, limit, allowed_updates = filter, timeout = secondsTimeout},
                cancellationToken, (timeout ?? _defaultGetUpdatesTimeout).Add(TimeSpan.FromSeconds(5)));
        }

        private async Task<T> SendRequestAsync<T>(string method,
            object payload = null,
            CancellationToken cancellationToken = default(CancellationToken),
            TimeSpan? requestTimeout = null) {

            var uri = new Uri(_uriWithToken, method);
            var handler = new HttpClientHandler();

            using (var client = new HttpClient(handler, true)) {
                client.Timeout = requestTimeout ?? _requestTimeout;

                try {
                    HttpResponseMessage response;

                    if (payload == null) {
                        _logger.LogDebug($"Trying to send request GET {uri}");
                        response = await client.GetAsync(uri, cancellationToken)
                                               .ConfigureAwait(false);

                        return await ParseResponse<T>(response);
                    }

                    string strPayload;
                    using (var strWriter = new StringWriter()) {
                        _serializer.Serialize(strWriter, payload);
                        strPayload = strWriter.ToString();
                    }

                    _logger.LogDebug($"Trying to send request GET {uri}");
                    _logger.LogDebug($"with {strPayload}");

                    HttpContent content = new StringContent(strPayload, _encoding, "application/json");

                    response = await client.PostAsync(uri, content, cancellationToken)
                        .ConfigureAwait(false);
                    return await ParseResponse<T>(response);
                }
                catch (Exception e) {
                    _logger.LogError(0, e, "Error occurred while request sending");
                    throw;
                }
            }
        }

        private async Task<T> ParseResponse<T>(HttpResponseMessage response) {
            string strResponse;
            using (var stream = await response.Content.ReadAsStreamAsync())
            using (var textReader = new StreamReader(stream, _encoding)) {
                strResponse = await textReader.ReadToEndAsync();
            }

            _logger.LogDebug($"Response received {strResponse}");

            using (var jsonReader = new JsonTextReader(new StringReader(strResponse))) {
                var tResponse = _serializer.Deserialize<TelegramResponse<T>>(jsonReader);

                if (!tResponse.IsOk) {
                    throw new TelegramApiException(tResponse.Description ?? "Error occured (not ok)", 500);
                }

                return tResponse.Result;
            }
        }

        [JsonObject(MemberSerialization.OptIn)]
        private class TelegramResponse<T> {

            [JsonProperty(PropertyName = "result", Required = Required.DisallowNull)]
            public T Result { get; set; }

            [JsonProperty(PropertyName = "ok", Required = Required.Always)]
            public bool IsOk { get; set; }

            [JsonProperty(PropertyName = "description")]
            public string Description { get; set; }

        }
    }
}