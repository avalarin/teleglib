using System;
using System.Collections.Generic;
using System.Linq;

namespace Teleglib.Localization {
    public class EnglishLocalizationProvider : ILocalizationProvider {
        public string Localize(IUserPreferences userPreferences, ILocalizationKey key, IDictionary<string, object> values) {
            if (key.Key.Equals(LocalizationKeys.SelectOneForCompletion.Key, StringComparison.OrdinalIgnoreCase)) {
                return "Select one: ";
            }
            if (key.Key.Equals(LocalizationKeys.UnknownCommand.Key, StringComparison.OrdinalIgnoreCase)) {
                return $"Unknown command: {values.Values.First()}";
            }
            throw new ArgumentException($"Unknown localization key {key.Key}");
        }
    }
}