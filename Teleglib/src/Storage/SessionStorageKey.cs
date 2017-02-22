using System;

namespace Teleglib.Storage {
    public class SessionStorageKey<T> : ISessionStorageKey<T> {
        public string Name { get; }
        public Type Type { get; }

        public SessionStorageKey(string name) {
            Name = name;
            Type = typeof(T);
        }
    }
}