using System.Collections.Generic;
using Teleglib.Utils;

namespace Teleglib.Localization {
    public class LocalizationManager {
        private readonly ILocalizationProvider _localizationProvider;

        public LocalizationManager(ILocalizationProvider localizationProvider) {
            _localizationProvider = localizationProvider;
        }

        public string Localize(IUserPreferences userPreferences, ILocalizationKey key) {
            return _localizationProvider.Localize(userPreferences, key, new Dictionary<string, object>());
        }

        public string Localize(IUserPreferences userPreferences, ILocalizationKey key, object parameters) {
            return _localizationProvider.Localize(userPreferences, key, parameters.GetProperties().ToDictionary());
        }
    }
}