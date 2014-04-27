using System;
using System.Threading;

namespace Zeus.Library.Pooling {

    public class LazyPool<T> : AbstractPool<T> {

        public int LazyLoadedItemCount {
            get;
            protected set;
        }


        public LazyPool(int poolSize, Func<IPool<T>, T> itemFactoryFunc, EPoolItemAccessMode accessMode = EPoolItemAccessMode.Fifo)
            : base(poolSize, itemFactoryFunc, accessMode) {
        }


        public override T Acquire() {
            _sync.WaitOne();

            // Try to get it from the pool of already released items
            lock (ItemStore) {
                if (ItemStore.Count > 0) {
                    return ItemStore.Fetch();
                }
            }

            // Raise amount of lazy-loaded items
            var tmp = LazyLoadedItemCount;
            Interlocked.Increment(ref tmp);
            LazyLoadedItemCount = tmp;
            // Fetch a new item
            return ItemFactoryFunc(this);
        }

    }

}