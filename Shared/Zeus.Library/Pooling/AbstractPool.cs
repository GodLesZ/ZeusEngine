using System;
using System.Diagnostics;
using System.Threading;

namespace Zeus.Library.Pooling {

    /// <summary>
    ///     A generic pool which is able to create 3 types of <see cref="ItemStore" />s />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AbstractPool<T> : IPool<T>, IDisposable {

        protected readonly Semaphore Sync;

        /// <summary>
        ///     Gets the delegate to create new items
        /// </summary>
        public Func<IPool<T>, T> Factory { get; protected set; }

        public bool IsDisposed { get; protected set; }

        /// <summary>
        ///     Gets the <see cref="PoolItemAccessMode" /> related item storage
        /// </summary>
        public IPoolItemStore<T> ItemStore { get; protected set; }

        /// <summary>
        ///     Gets the initial pool size
        /// </summary>
        public int PoolSize { get; protected set; }


        protected AbstractPool(int poolSize, Func<IPool<T>, T> factory)
            : this(poolSize, factory, PoolItemAccessMode.Fifo) {
        }

        protected AbstractPool(int poolSize, Func<IPool<T>, T> factory, PoolItemAccessMode accessMode) {
            if (poolSize <= 0) {
                throw new ArgumentOutOfRangeException("poolSize", poolSize, "Argument 'size' must be greater than zero.");
            }
            if (factory == null) {
                throw new ArgumentNullException("factory");
            }

            Sync = new Semaphore(poolSize, poolSize);

            PoolSize = poolSize;
            Factory = factory;
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

            Sync.Release();
        }

        /// <summary>
        ///     Dispose all un-released items in the poll storage and the pool itself
        /// </summary>
        public void Dispose() {
            if (IsDisposed) {
                return;
            }

            IsDisposed = true;
            // Does the pool item support the IDisposable interface?
            if (typeof(IDisposable).IsAssignableFrom(typeof(T))) {
                // if so, try to dispose all unreleased items from the storage
                lock (ItemStore) {
                    while (ItemStore.Count > 0) {
                        var disposable = (IDisposable)ItemStore.Fetch();
                        try {
                            disposable.Dispose();
                        } catch {
                        }
                    }
                }
            }

            // Close the semaphore
            Sync.Close();
        }


        /// <summary>
        ///     Creates  the <see cref="ItemStore" /> based on the given <see cref="PoolItemAccessMode" />
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="capacity"></param>
        /// <returns></returns>
        protected IPoolItemStore<T> CreateItemStore(PoolItemAccessMode mode, int capacity) {
            switch (mode) {
                case PoolItemAccessMode.Fifo:
                    return new QueuePoolItemStore<T>(capacity);
                case PoolItemAccessMode.Lifo:
                    return new StackPoolItemStore<T>(capacity);
                default:
                    Debug.Assert(mode == PoolItemAccessMode.Circular, "Invalid AccessMode in CreateItemStore");
                    return new CircularStore<T>(capacity);
            }
        }

    }

}