using System.Collections.Generic;

namespace Zeus.Library.Pooling {

    public class StackPoolItemStore<T> : Stack<T>, IPoolItemStore<T> {

        public StackPoolItemStore(int capacity)
            : base(capacity) {
        }


        public T Fetch() {
            return Pop();
        }

        public void Store(T item) {
            Push(item);
        }

    }

}