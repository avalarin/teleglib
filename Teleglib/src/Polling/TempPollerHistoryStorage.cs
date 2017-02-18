using System.Threading.Tasks;

namespace Teleglib.Polling {
    class TempPollerHistoryStorage : IPollerHistoryStorage {
        private readonly long _lastUpdateId;

        public TempPollerHistoryStorage(long lastUpdateId) {
            _lastUpdateId = lastUpdateId;
        }

        public Task SaveLastUpdateId(long updateId) {
            return Task.CompletedTask;
        }

        public Task<long> GetLastUpdateId() {
            return Task.FromResult(_lastUpdateId);
        }
    }
}