using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Teleglib.Utils;

namespace Teleglib.Features {
    public class ImmutableFeaturesSet<TBase> : IFeaturesAccessor<TBase> {

        private readonly ImmutableDictionary<Type, ImmutableList<TBase>> _features;

        public ImmutableFeaturesSet() {
            _features = ImmutableDictionary.Create<Type, ImmutableList<TBase>>();
        }

        private ImmutableFeaturesSet(ImmutableDictionary<Type, ImmutableList<TBase>> features) {
            _features = features;
        }

        public ImmutableFeaturesSet<TBase> AddExclusive<T>(TBase feature) where T : TBase {
            var newDict = _features.Update(typeof(T),
                valueFactory: t => ImmutableList<TBase>.Empty,
                valueUpdater: (t, list) => {
                    if (list.Count > 0) throw new InvalidOperationException($"Set has one or more features of type {typeof(T)}");
                    return list.Add(feature);
                });

            return new ImmutableFeaturesSet<TBase>(newDict);
        }

        public ImmutableFeaturesSet<TBase> Add<T>(TBase feature) where T : TBase {
            var newDict = _features.Update(typeof(T),
                valueFactory: t => ImmutableList<TBase>.Empty,
                valueUpdater: (t, list) => list.Add(feature));

            return new ImmutableFeaturesSet<TBase>(newDict);
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
                .OrElseThrow(() => new InvalidOperationException($"Set has no features of type {typeof(T)}"));
        }

        public IEnumerable<T> GetAll<T>() where T : TBase {
            return _features.Get(typeof(T))
                .Map(list => (IEnumerable<TBase>)list)
                .OrElseGet(Enumerable.Empty<TBase>)
                .Select(item => (T)item);
        }

        public IEnumerable<T> GetAllOfBaseType<T>() {
            return _features.Values.SelectMany(l => l).OfType<T>();
        }
    }
}