using System;

 namespace Teleglib.Utils {
     public class Maybe<T> {

         private readonly bool _isPresent;
         private readonly T _value;

         private Maybe(bool isPresent, T value) {
             _isPresent = isPresent;
             _value = value;
         }

         public bool IsPresent => _isPresent;

         public T Value {
             get {
                 if (!_isPresent) throw new NullReferenceException("There is no value");
                 return _value;
             }
         }

         public Maybe<TResult> Map<TResult>(Func<T, TResult> mapper) {
             if (mapper == null) throw new ArgumentNullException(nameof(mapper));
             if (!IsPresent) return Maybe<TResult>.Empty();
             return Maybe<TResult>.OfNullable(mapper(_value));
         }

         public Maybe<TResult> Map<TResult>(Func<T, Maybe<TResult>> mapper) {
             if (mapper == null) throw new ArgumentNullException(nameof(mapper));
             if (!IsPresent) return Maybe<TResult>.Empty();
             return  mapper(_value);
         }

         public Maybe<T> Filter(Predicate<T> predicate) {
             if (predicate == null) throw new ArgumentNullException(nameof(predicate));
             if (!IsPresent || !predicate(_value)) return Empty();
             return this;
         }
         public bool IfPresent(Action<T> consumer) {
             if (consumer == null) throw new ArgumentNullException(nameof(consumer));
             if (IsPresent) consumer(_value);
             return IsPresent;
         }

         public T OrElse(T elseValue) {
             return IsPresent ? _value : elseValue;
         }

         public T OrElseGet(Func<T> supplier) {
             if (supplier == null) throw new ArgumentNullException(nameof(supplier));
             return IsPresent ? _value : supplier();
         }

         public T OrElseThrow<TX>(Func<TX> exceptionSupplier) where TX : Exception {
             if (exceptionSupplier == null) throw new ArgumentNullException(nameof(exceptionSupplier));
             if (IsPresent) return _value;
             throw exceptionSupplier();
         }

         public static Maybe<T> Empty() {
             return new Maybe<T>(false, default(T));
         }

         public static Maybe<T> Of(T value) {
             if (value == null) throw new NullReferenceException("There is no value");
             return new Maybe<T>(true, value);
         }

         public static Maybe<T> OfNullable(T value) {
             return new Maybe<T>(value != null, value);
         }

     }
 }