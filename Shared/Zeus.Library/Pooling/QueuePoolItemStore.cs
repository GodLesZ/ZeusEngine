using System.Collections.Generic;

namespace Zeus.Library.Pooling {

    public class QueuePoolItemStore<T> : Queue<T>, IPoolItemStore<T> {

        public QueuePoolItemStore(int capacity)
            : base(capacity) {
        }


        public T Fetch() {
            return Dequeue();
        }

        public void Store(T item) {
            Enqueue(item);
        }

    }

}