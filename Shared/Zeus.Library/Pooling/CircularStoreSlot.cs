namespace Zeus.Library.Pooling {

    public class CircularStoreSlot<T> {

        public bool IsInUse { get; set; }

        public T Item { get; private set; }


        public CircularStoreSlot(T item) {
            Item = item;
        }

    }

}