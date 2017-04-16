using System.Globalization;

namespace Teleglib.Localization {
    public interface IUserPreferences {
        CultureInfo Culture { get; }
    }
}