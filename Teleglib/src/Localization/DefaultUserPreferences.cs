using System.Globalization;

namespace Teleglib.Localization {
    public class DefaultUserPreferences : IUserPreferences {
        public static IUserPreferences Instance = new DefaultUserPreferences();

        public CultureInfo Culture { get; } = CultureInfo.CurrentUICulture;
    }
}