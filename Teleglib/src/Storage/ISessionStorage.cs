using System.Threading.Tasks;

namespace Teleglib.Storage {
    public interface ISessionStorage {
        Task SetAsync<T>(ISessionStorageKey<T> key, T value);
        Task<T> GetAsync<T>(ISessionStorageKey<T> key);
    }
}