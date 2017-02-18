using System;

namespace Teleglib.Utils {
    public static class NullableExtensions {

        public static TResult? Map<T, TResult>(this T? value, Func<T, TResult> mapper) where T : struct where TResult : struct {
            if (mapper == null) throw new ArgumentNullException(nameof(mapper));
            return value.HasValue ? new TResult?(mapper(value.Value)) : null;
        }

        public static TResult? Map<T, TResult>(this T? value, Func<T, TResult?> mapper) where T : struct where TResult : struct {
            if (mapper == null) throw new ArgumentNullException(nameof(mapper));
            return value.HasValue ? mapper(value.Value) : null;
        }

        public static T? Filter<T>(this T? value, Predicate<T> predicate) where T : struct  {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            if (!value.HasValue || !predicate(value.Value)) return null;
            return value;
        }

        public static bool IfPresent<T>(this T? value, Action<T> consumer) where T : struct {
            if (consumer == null) throw new ArgumentNullException(nameof(consumer));
            if (value.HasValue) consumer(value.Value);
            return value.HasValue;
        }

        public static T OrElse<T>(this T? value, T elseValue) where T : struct {
            return value ?? elseValue;
        }

        public static T OrElseGet<T>(this T? value, Func<T> supplier) where T : struct {
            if (supplier == null) throw new ArgumentNullException(nameof(supplier));
            return value ?? supplier();
        }

        public static T OrElseThrow<T, TX>(this T? value, Func<TX> exceptionSupplier) where T : struct where TX : Exception {
            if (exceptionSupplier == null) throw new ArgumentNullException(nameof(exceptionSupplier));
            if (value.HasValue) return value.Value;
            throw exceptionSupplier();
        }

    }
}