using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Teleglib.Utils {
    public static class DictionaryExtensions {

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(
            this IEnumerable<KeyValuePair<TKey, TValue>> source) {
            return source.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public static Maybe<TValue> Get<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key) {
            TValue value;
            if (dict.TryGetValue(key, out value)) {
                return Maybe<TValue>.Of(value);
            }
            return Maybe<TValue>.Empty();
        }

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue> valueFactory) {
            TValue value;
            if (dict.TryGetValue(key, out value)) {
                return value;
            }
            value = valueFactory(key);
            dict.Add(key, value);
            return value;
        }

        public static ImmutableDictionary<TKey, TValue> Update<TKey, TValue>(
            this ImmutableDictionary<TKey, TValue> dict, TKey key,
            Func<TKey, TValue, TValue> valueUpdater) {

            return Update<TKey, TValue>(dict, key, (k) => default(TValue), valueUpdater);
        }

        public static ImmutableDictionary<TKey, TValue> Update<TKey, TValue>(
            this ImmutableDictionary<TKey, TValue> dict,
            TKey key, Func<TKey, TValue> valueFactory,
            Func<TKey, TValue, TValue> valueUpdater) {

            TValue value;
            if (!dict.TryGetValue(key, out value)) {
                value = valueFactory(key);
            }

            value = valueUpdater(key, value);

            return dict.SetItem(key, value);
        }

        public static void Merge<TKey, TValue>(this IDictionary<TKey, TValue> target,
            IDictionary<TKey, TValue> source, Func<TKey, TValue, TValue, TValue> remapper = null) {

            foreach (var kvp in source) {
                TValue originalValue;
                if (remapper != null && target.TryGetValue(kvp.Key, out originalValue)) {
                    target[kvp.Key] = remapper(kvp.Key, originalValue, kvp.Value);
                }
                else {
                    target[kvp.Key] = kvp.Value;
                }
            }
        }

        public static void MergeTo<TKey, TValue>(this IDictionary<TKey, TValue> source,
            IDictionary<TKey, TValue> target, Func<TKey, TValue, TValue, TValue> remapper = null) {

            Merge(target, source, remapper);
        }

    }
}