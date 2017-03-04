using System.Threading.Tasks;
using Teleglib.Router;

namespace Teleglib.Session {
    public interface ISessionStorage {
        Task SaveMessageContextAsync(long chatId, long messageId, MessageContext context);
        Task<MessageContext> LoadMessageContextAsync(long chatId, long messageId);

        Task SaveRoutingDataAsync(long chatId, RoutingData routingData);
        Task ClearRoutingDataAsync(long chatId);
        Task<RoutingData> GetRoutingDataAsync(long chatId);
    }
}