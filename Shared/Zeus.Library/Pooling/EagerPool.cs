using System;

namespace Zeus.Library.Pooling {

    public class EagerPool<T> : AbstractPool<T> {

        public EagerPool(int poolSize, Func<IPool<T>, T> factory)
            : this(poolSize, factory, PoolItemAccessMode.Fifo) {
        }

        public EagerPool(int poolSize, Func<IPool<T>, T> factory, PoolItemAccessMode accessMode)
            : base(poolSize, factory, accessMode) {
            PreloadItems();
        }


        public override T Acquire() {
            Sync.WaitOne();

            lock (ItemStore) {
                return ItemStore.Fetch();
            }
        }

        /// <summary>
        ///     Preloads all items
        /// </summary>
        protected void PreloadItems() {
            for (var i = 0; i < PoolSize; i++) {
                var item = Factory(this);
                ItemStore.Store(item);
            }
        }

    }

}