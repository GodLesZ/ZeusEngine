using System.Collections.Generic;

namespace Zeus.Library.Pooling {

    /// <summary>
    ///     A last-in-first-out item storage implementation, based on a <see cref="T:System.Collections.Generic.Stack`1"/>
    /// </summary>
    public class StackPoolItemStore<T> : Stack<T>, IPoolItemStore<T> {

        /// <summary>
        ///     Initializes a new last-in-first-out item storage that is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the underlying <see cref="T:System.Collections.Generic.Stack`1"/> can contain.</param>
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