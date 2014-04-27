using System.Collections.Generic;

namespace Zeus.Library.Pooling {

    /// <summary>
    ///     A first-in-first-out item storage implementation, based on a <see cref="T:System.Collections.Generic.Queue`1"/>
    /// </summary>
    public class QueuePoolItemStore<T> : Queue<T>, IPoolItemStore<T> {

        /// <summary>
        ///     Initializes a new first-in-first-out item storage that is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the underlying <see cref="T:System.Collections.Generic.Queue`1"/> can contain.</param>
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