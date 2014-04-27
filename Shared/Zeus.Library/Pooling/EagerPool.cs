using System;

namespace Zeus.Library.Pooling {

    public class EagerPool<T> : AbstractPool<T> {

        public EagerPool(int poolSize, Func<IPool<T>, T> itemFactoryFunc, EPoolItemAccessMode accessMode = EPoolItemAccessMode.Fifo)
            : base(poolSize, itemFactoryFunc, accessMode) {
            PreloadItems();
        }


        public override T Acquire() {
            _sync.WaitOne();

            lock (ItemStore) {
                return ItemStore.Fetch();
            }
        }

        /// <summary>
        ///     Preloads all items
        /// </summary>
        protected void PreloadItems() {
            for (var i = 0; i < PoolSize; i++) {
                var item = ItemFactoryFunc(this);
                ItemStore.Store(item);
            }
        }

    }

}