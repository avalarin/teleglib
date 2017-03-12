using System;
using System.Collections.Generic;

namespace Teleglib.Utils {
    public abstract class GridBuilder<T, TResult> {

        private readonly List<IEnumerable<T>> _list = new List<IEnumerable<T>>();

        public GridBuilder<T, TResult> AddRow(Action<RowBuilder> action) {
            var rowBuilder = new RowBuilder();
            action(rowBuilder);
            _list.Add(rowBuilder.Build());
            return this;
        }

        public TResult Build() {
            return MapResult(_list.AsReadOnly());
        }

        public class RowBuilder {

            private readonly List<T> _items = new List<T>();

            public RowBuilder AddItem(T item) {
                _items.Add(item);
                return this;
            }

            public IEnumerable<T> Build() {
                return _items;
            }

        }

        protected abstract TResult MapResult(IEnumerable<IEnumerable<T>> result);

    }
}