using System;
using System.Collections.Generic;

namespace Teleglib.Middlewares {
    public class MiddlewaresChainBuilder {

        private readonly List<MiddlewareInfo> _middlewares;

        public MiddlewaresChainBuilder() {
            _middlewares = new List<MiddlewareInfo>();
        }

        public MiddlewaresChainBuilder InsertFirst<T>() where T : IMiddleware {
            _middlewares.Insert(0, new MiddlewareInfo(typeof(T)));
            return this;
        }

        public MiddlewaresChainBuilder InsertFirst<T>(T instance) where T : IMiddleware {
            _middlewares.Insert(0, new MiddlewareInfo(instance));
            return this;
        }

        public MiddlewaresChainBuilder InsertLast<T>() where T : IMiddleware {
            _middlewares.Add(new MiddlewareInfo(typeof(T)));
            return this;
        }

        public MiddlewaresChainBuilder InsertLast<T>(T instance) where T : IMiddleware {
            _middlewares.Add(new MiddlewareInfo(instance));
            return this;
        }

        public MiddlewaresChainBuilder InsertAt<T>(int index) where T : IMiddleware {
            _middlewares.Insert(index, new MiddlewareInfo(typeof(T)));
            return this;
        }

        public MiddlewaresChainBuilder InsertAt<T>(int index, T instance) where T : IMiddleware {
            _middlewares.Insert(index, new MiddlewareInfo(instance));
            return this;
        }

        public MiddlewaresChainBuilder InsertBefore<T, TTarget>() where T : IMiddleware {
            var index = _middlewares.FindIndex(m => m.Type == typeof(TTarget));
            if (index == -1) throw new ArgumentException($"Middleware {typeof(TTarget)} not found");

            _middlewares.Insert(index, new MiddlewareInfo(typeof(T)));
            return this;
        }

        public MiddlewaresChainBuilder InsertBefore<T, TTarget>(T instance) where T : IMiddleware {
            var index = _middlewares.FindIndex(m => m.Type == typeof(TTarget));
            if (index == -1) throw new ArgumentException($"Middleware {typeof(TTarget)} not found");

            _middlewares.Insert(index, new MiddlewareInfo(instance));
            return this;
        }

        public MiddlewaresChainBuilder InsertAfter<T, TTarget>() where T : IMiddleware {
            var index = _middlewares.FindIndex(m => m.Type == typeof(TTarget));
            if (index == -1) throw new ArgumentException($"Middleware {typeof(TTarget)} not found");

            _middlewares.Insert(index + 1, new MiddlewareInfo(typeof(T)));
            return this;
        }

        public MiddlewaresChainBuilder InsertAfter<T, TTarget>(T instance) where T : IMiddleware {
            var index = _middlewares.FindIndex(m => m.Type == typeof(TTarget));
            if (index == -1) throw new ArgumentException($"Middleware {typeof(TTarget)} not found");

            _middlewares.Insert(index + 1, new MiddlewareInfo(instance));
            return this;
        }

        public MiddlewaresChainBuilder Clear() {
            _middlewares.Clear();
            return this;
        }

        public IMiddlewaresChain Build(IMiddlewaresChainFactory chainFactory) {
            var chain = chainFactory.CreateTailElement();
            for (var i = _middlewares.Count - 1; i >= 0; i--) {
                var info = _middlewares[i];
                if (info.HasInstance) {
                    chain = chainFactory.CreateElement(info.Instance, chain);
                }
                else {
                    chain = chainFactory.CreateElement(info.Type, chain);
                }
            }
            return chain;
        }

        private class MiddlewareInfo {
            public Type Type { get; }
            public IMiddleware Instance { get; }
            public bool HasInstance { get; }

            public MiddlewareInfo(Type type) {
                Type = type;
            }

            public MiddlewareInfo(IMiddleware instance) {
                Instance = instance;
                HasInstance = true;
            }
        }

    }
}