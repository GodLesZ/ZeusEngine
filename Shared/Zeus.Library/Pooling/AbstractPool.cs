using System;
using System.Diagnostics;
using System.Threading;

namespace Zeus.Library.Pooling {

    /// <summary>
    ///     A generic pool which is able to create 3 types of <see cref="ItemStore" />s />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AbstractPool<T> : IPool<T>, IDisposable {

        protected readonly Semaphore _sync;

        /// <summary>
        ///     Gets the delegate to create new items
        /// </summary>
        public Func<IPool<T>, T> ItemFactoryFunc {
            get;
            protected set;
        }

        /// <summary>
        ///     Gets if this pool is disposed or not.
        /// </summary>
        public bool IsDisposed {
            get;
            protected set;
        }

        /// <summary>
        ///     Gets the <see cref="EPoolItemAccessMode" /> related item storage
        /// </summary>
        public IPoolItemStore<T> ItemStore {
            get;
            protected set;
        }

        /// <summary>
        ///     Gets the initial pool size
        /// </summary>
        public int PoolSize {
            get;
            protected set;
        }


        protected AbstractPool(int poolSize, Func<IPool<T>, T> itemFactoryFunc, EPoolItemAccessMode accessMode = EPoolItemAccessMode.Fifo) {
            if (poolSize <= 0) {
                throw new ArgumentOutOfRangeException("poolSize", poolSize, "Size of the pool must be greater than zero.");
            }
            if (itemFactoryFunc == null) {
                throw new ArgumentNullException("itemFactoryFunc");
            }

            _sync = new Semaphore(poolSize, poolSize);

            PoolSize = poolSize;
            ItemFactoryFunc = itemFactoryFunc;
            ItemStore = CreateItemStore(accessMode, poolSize);
        }


        /// <summary>
        ///     Get a item from the pool
        /// </summary>
        /// <returns></returns>
        public virtual T Acquire() {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Release a pooled item back to the store
        /// </summary>
        /// <param name="item"></param>
        public void Release(T item) {
            lock (ItemStore) {
                ItemStore.Store(item);
            }

            _sync.Release();
        }

        /// <summary>
        ///     Dispose all un-released items in the poll storage and the pool itself
        /// </summary>
        public void Dispose() {
            if (IsDisposed) {
                return;
            }

            IsDisposed = true;
            // Do we have items in the store and does the item support the IDisposable interface?
            if (ItemStore.Count > 0 && typeof(IDisposable).IsAssignableFrom(typeof(T))) {
                // If so, try to dispose all unreleased items from the storage
                lock (ItemStore) {
                    do {
                        var disposable = (IDisposable)ItemStore.Fetch();
                        // Avoid expensive exceptions
                        if (disposable == null) {
                            continue;
                        }

                        try {
                            disposable.Dispose();
                        } catch {
                        }
                    } while (ItemStore.Count > 0);
                }
            }

            // Close the semaphore
            _sync.Close();
        }


        /// <summary>
        ///     Creates the <see cref="ItemStore" /> based on the given <see cref="EPoolItemAccessMode" />
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="capacity"></param>
        /// <returns></returns>
        protected IPoolItemStore<T> CreateItemStore(EPoolItemAccessMode mode, int capacity) {
            switch (mode) {
                case EPoolItemAccessMode.Fifo:
                    return new QueuePoolItemStore<T>(capacity);
                case EPoolItemAccessMode.Lifo:
                    return new StackPoolItemStore<T>(capacity);
                default:
                    Debug.Assert(mode == EPoolItemAccessMode.Circular, "Invalid AccessMode in CreateItemStore");
                    return new CircularStore<T>(capacity);
            }
        }

    }

}