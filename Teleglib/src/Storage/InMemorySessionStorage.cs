using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Teleglib.Utils;

namespace Teleglib.Storage {
    public class InMemorySessionStorage : ISessionStorage {
        private readonly Dictionary<Key, object> _storage = new Dictionary<Key, object>();

        public Task SetAsync<T>(ISessionStorageKey<T> key, T value) {
            var intKey = new Key(key.GetType(), key.Name);
            _storage[intKey] = value;
            return Task.CompletedTask;
        }

        public Task<T> GetAsync<T>(ISessionStorageKey<T> key) {
            var intKey = new Key(key.GetType(), key.Name);
            var value = (T)_storage.GetOrDefault(intKey);
            return Task.FromResult(value);
        }

        private class Key {
            private readonly Type _keyType;
            private readonly string _keyName;

            public Key(Type keyType, string keyName) {
                _keyType = keyType;
                _keyName = keyName;
            }

            public Type KeyType => _keyType;

            public string KeyName => _keyName;

            protected bool Equals(Key other) {
                return Equals(_keyType, other._keyType) && string.Equals(_keyName, other._keyName);
            }

            public override bool Equals(object obj) {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Key) obj);
            }

            public override int GetHashCode() {
                unchecked {
                    return ((_keyType?.GetHashCode() ?? 0) * 397) ^ (_keyName?.GetHashCode() ?? 0);
                }
            }
        }

    }
}