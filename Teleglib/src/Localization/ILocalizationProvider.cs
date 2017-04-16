using System.Collections.Generic;

namespace Teleglib.Localization {
    public interface ILocalizationProvider {

        string Localize(IUserPreferences userPreferences, ILocalizationKey key, IDictionary<string, object> values);

    }
}