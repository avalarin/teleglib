using System;
using System.Collections.Generic;

namespace Teleglib.Utils {
    public static class EnumerableExtensions {
        public static void ForEach<T>(this IEnumerable<T> src, Action<T> supplier) {
            foreach (var item in src) {
                supplier(item);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> src, Action<int, T> supplier) {
            var i = 0;
            foreach (var item in src) {
                supplier(i++, item);
            }
        }

        public static string JoinToString<T>(this IEnumerable<T> src, string separator) {
            return string.Join(separator, src);
        }
    }
}