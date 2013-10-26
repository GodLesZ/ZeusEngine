namespace Zeus.Library.Pooling {

    public interface IPoolItemStore<T> {

        int Count { get; }

        T Fetch();
        void Store(T item);

    }

}