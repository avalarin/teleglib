using System;
using System.Collections.Generic;
using System.Linq;
using Teleglib.Utils;

namespace Teleglib.Features {
    public class FeaturesSet<TBase> : IFeaturesAccessor<TBase> {

        private readonly Dictionary<Type, List<TBase>> _features;

        public FeaturesSet() {
            _features = new Dictionary<Type, List<TBase>>();
        }

        public FeaturesSet(int capacity) {
            _features = new Dictionary<Type, List<TBase>>(capacity);
        }

        public void AddExclusive<T>(TBase feature) where T : TBase {
            var list = _features.GetOrAdd(typeof(TBase), t => new List<TBase>());
            if (list.Count > 0) throw new InvalidOperationException($"Set has one or more features of type {typeof(T)}");
            list.Add(feature);
        }

        public void Add<T>(TBase feature) where T : TBase {
            var list = _features.GetOrAdd(typeof(TBase), t => new List<TBase>());
            list.Add(feature);
        }

        public bool Has<T>() where T : TBase {
            return _features.Get(typeof(T))
                .Filter(l => l.Count > 0)
                .IsPresent;
        }

        public Maybe<T> GetOne<T>() where T : TBase {
            return _features.Get(typeof(T))
                .Filter(list => list.Count > 0)
                .Map(list => {
                    if (list.Count > 1) {
                        throw new InvalidOperationException($"Set has more than 1 features of type {typeof(T)}");
                    }
                    return (T)list[0];
                });
        }

        public T RequireOne<T>() where T : TBase {
            return GetOne<T>()
                .OrElseThrow(() => new InvalidOperationException($"Send don't have features of type {typeof(T)}"));
        }

        public IEnumerable<T> GetAll<T>() where T : TBase {
            return _features.Get(typeof(T))
                .Map(l => (IEnumerable<T>)l.AsReadOnly())
                .OrElseGet(Enumerable.Empty<T>);
        }

    }
}