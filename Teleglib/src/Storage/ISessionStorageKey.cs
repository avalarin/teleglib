using System;

namespace Teleglib.Storage {
    public interface ISessionStorageKey<T> {
        string Name { get; }
        Type Type { get; }
    }
}