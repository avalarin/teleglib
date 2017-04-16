using Teleglib.Features;

namespace Teleglib.Localization {
    public class UserPreferencesFeature : IFeature {
        public IUserPreferences UserPreferences { get; }

        public UserPreferencesFeature(IUserPreferences userPreferences) {
            UserPreferences = userPreferences;
        }
    }
}