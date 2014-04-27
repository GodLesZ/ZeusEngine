using System;

namespace Zeus.Library.Pooling {

    public interface IPool<T> {

        Func<IPool<T>, T> ItemFactoryFunc { get; }

        bool IsDisposed { get; }

        IPoolItemStore<T> ItemStore { get; }

        int PoolSize { get; }

        T Acquire();
        void Release(T item);
        void Dispose();

    }

}