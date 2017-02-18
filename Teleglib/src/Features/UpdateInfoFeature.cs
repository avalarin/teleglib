using Teleglib.Telegram.Models;

namespace Teleglib.Features {
    public class UpdateInfoFeature : IFeature {
        public UpdateInfo Update { get; }

        public UpdateInfoFeature(UpdateInfo update) {
            Update = update;
        }
    }
}