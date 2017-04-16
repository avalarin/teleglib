namespace Teleglib.Localization {
    public class LocalizationKeys {

        public static ILocalizationKey SelectOneForCompletion = new SimpleLocalizationKey("routing.select_one_for_completion");
        public static ILocalizationKey UnknownCommand = new SimpleLocalizationKey("routing.unknown_command");

        private class SimpleLocalizationKey : ILocalizationKey {
            public string Key { get; }

            public SimpleLocalizationKey(string key) {
                Key = key;
            }
        }

    }
}