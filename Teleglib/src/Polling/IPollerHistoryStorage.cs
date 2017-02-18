using System.Threading.Tasks;

namespace Teleglib.Polling {
    public interface IPollerHistoryStorage {
        Task SaveLastUpdateId(long updateId);
        Task<long> GetLastUpdateId();
    }
}