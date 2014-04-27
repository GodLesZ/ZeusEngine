namespace Zeus.Library.Pooling {

    /// <summary>
    ///     A interface to describe an abstract item storage.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPoolItemStore<T> {

        /// <summary>
        ///     Gets the current amount of items in this storage
        /// </summary>
        int Count { get; }

        /// <summary>
        ///     Fetches a new item from the storage using the underlying logic.
        /// </summary>
        /// <returns></returns>
        T Fetch();
        /// <summary>
        ///     Stores an previously fetched item back to the storage using the underlying logic.
        /// </summary>
        void Store(T item);

    }

}