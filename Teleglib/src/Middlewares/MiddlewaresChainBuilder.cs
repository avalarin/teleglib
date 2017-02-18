using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Teleglib.Utils;

namespace Teleglib.Middlewares {
    public class MiddlewaresChainBuilder {

        private readonly List<IMiddlewareInstanceSource> _middlewares;

        public MiddlewaresChainBuilder() {
            _middlewares = new List<IMiddlewareInstanceSource>();
        }

        public MiddlewaresChainBuilder InsertFirst<T>() where T : IMiddleware {
            _middlewares.Insert(0, new MiddlewareInfo<T>());
            return this;
        }

        public MiddlewaresChainBuilder InsertFirst<T>(T instance) where T : IMiddleware {
            _middlewares.Insert(0, new MiddlewareInfo<T>(instance));
            return this;
        }

        public MiddlewaresChainBuilder InsertFirst<T>(Func<T> factory) where T : IMiddleware {
            _middlewares.Insert(0, new MiddlewareInfo<T>(factory));
            return this;
        }

        public MiddlewaresChainBuilder InsertLast<T>() where T : IMiddleware {
            _middlewares.Add(new MiddlewareInfo<T>());
            return this;
        }

        public MiddlewaresChainBuilder InsertLast<T>(T instance) where T : IMiddleware {
            _middlewares.Add(new MiddlewareInfo<T>(instance));
            return this;
        }

        public MiddlewaresChainBuilder InsertLast<T>(Func<T> factory) where T : IMiddleware {
            _middlewares.Add(new MiddlewareInfo<T>(factory));
            return this;
        }

        public MiddlewaresChainBuilder InsertAt<T>(int index) where T : IMiddleware {
            _middlewares.Insert(index, new MiddlewareInfo<T>());
            return this;
        }

        public MiddlewaresChainBuilder InsertAt<T>(int index, T instance) where T : IMiddleware {
            _middlewares.Insert(index, new MiddlewareInfo<T>(instance));
            return this;
        }

        public MiddlewaresChainBuilder InsertAt<T>(int index, Func<T> factory) where T : IMiddleware {
            _middlewares.Insert(index, new MiddlewareInfo<T>(factory));
            return this;
        }

        public MiddlewaresChainBuilder InsertBefore<T, TTarget>() where T : IMiddleware {
            var index = _middlewares.FindIndex(m => m.Type == typeof(TTarget));
            if (index == -1) throw new ArgumentException($"Middleware {typeof(TTarget)} not found");

            _middlewares.Insert(index, new MiddlewareInfo<T>());
            return this;
        }

        public MiddlewaresChainBuilder InsertBefore<T, TTarget>(T instance) where T : IMiddleware {
            var index = _middlewares.FindIndex(m => m.Type == typeof(TTarget));
            if (index == -1) throw new ArgumentException($"Middleware {typeof(TTarget)} not found");

            _middlewares.Insert(index, new MiddlewareInfo<T>(instance));
            return this;
        }

        public MiddlewaresChainBuilder InsertBefore<T, TTarget>(Func<T> factory) where T : IMiddleware {
            var index = _middlewares.FindIndex(m => m.Type == typeof(TTarget));
            if (index == -1) throw new ArgumentException($"Middleware {typeof(TTarget)} not found");

            _middlewares.Insert(index, new MiddlewareInfo<T>(factory));
            return this;
        }

        public MiddlewaresChainBuilder InsertAfter<T, TTarget>() where T : IMiddleware {
            var index = _middlewares.FindIndex(m => m.Type == typeof(TTarget));
            if (index == -1) throw new ArgumentException($"Middleware {typeof(TTarget)} not found");

            _middlewares.Insert(index + 1, new MiddlewareInfo<T>());
            return this;
        }

        public MiddlewaresChainBuilder InsertAfter<T, TTarget>(T instance) where T : IMiddleware {
            var index = _middlewares.FindIndex(m => m.Type == typeof(TTarget));
            if (index == -1) throw new ArgumentException($"Middleware {typeof(TTarget)} not found");

            _middlewares.Insert(index + 1, new MiddlewareInfo<T>(instance));
            return this;
        }

        public MiddlewaresChainBuilder InsertAfter<T, TTarget>(Func<T> factory) where T : IMiddleware {
            var index = _middlewares.FindIndex(m => m.Type == typeof(TTarget));
            if (index == -1) throw new ArgumentException($"Middleware {typeof(TTarget)} not found");

            _middlewares.Insert(index + 1, new MiddlewareInfo<T>(factory));
            return this;
        }

        public MiddlewaresChainBuilder Clear() {
            _middlewares.Clear();
            return this;
        }

        public IMiddlewaresChain Build(IServiceProvider serviceProvider) {
            IMiddlewaresChain chain = new TailElement();
            for (var i = _middlewares.Count - 1; i >= 0; i--) {
                chain = new ChainElement(serviceProvider, _middlewares[i], chain);
            }
            return chain;
        }

        private class ChainElement : IMiddlewaresChain {
            private readonly IServiceProvider _serviceProvider;
            private readonly IMiddlewareInstanceSource _middleware;
            private readonly IMiddlewaresChain _next;

            public ChainElement(IServiceProvider serviceProvider, IMiddlewareInstanceSource middleware, IMiddlewaresChain next) {
                _serviceProvider = serviceProvider;
                _middleware = middleware;
                _next = next;
            }

            public async Task<MiddlewareData> NextAsync(MiddlewareData data) {
                var executor = _middleware.GetInstance(_serviceProvider);
                return await executor.InvokeAsync(data, _next);
            }

        }

        private class TailElement : IMiddlewaresChain {
            public Task<MiddlewareData> NextAsync(MiddlewareData data) {
                return Task.FromResult(data);
            }
        }

        private interface IMiddlewareInstanceSource {
            Type Type { get; }
            IMiddleware GetInstance(IServiceProvider provider);
        }

        private class MiddlewareInfo<T> : IMiddlewareInstanceSource where T : IMiddleware {
            public Type Type { get; }
            public T Instance { get; }
            public bool HasInstance { get; }
            public Func<T> Factory { get; }

            public MiddlewareInfo() {
                Type = typeof(T);
            }

            public MiddlewareInfo(T instance) {
                Instance = instance;
                HasInstance = true;
                Factory = null;
            }

            public MiddlewareInfo(Func<T> factory) {
                Factory = factory;
                Instance = default(T);
            }

            public IMiddleware GetInstance(IServiceProvider provider) {
                if (HasInstance) return Instance;
                if (Factory != null) return Factory.Invoke();
                return (IMiddleware) provider.GetInstance(Type);
            }
        }

    }
}