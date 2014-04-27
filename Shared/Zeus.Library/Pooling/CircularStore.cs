using System;
using System.Collections.Generic;

namespace Zeus.Library.Pooling {

    /// <summary>
    ///     A circulated item storage implementation, based on a <see cref="T:System.Collections.Generic.List`1"/>
    /// </summary>
    public class CircularStore<T> : IPoolItemStore<T> {

        protected int _freeSlotCount;
        protected int _position = -1;
        protected readonly List<CircularStoreSlot<T>> _slots;

        public int Count {
            get { return _freeSlotCount; }
        }


        /// <summary>
        ///     Initializes a new circulated item storage that is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the underlying <see cref="T:System.Collections.Generic.List`1"/> can contain.</param>
        public CircularStore(int capacity) {
            _slots = new List<CircularStoreSlot<T>>(capacity);
        }


        public T Fetch() {
            if (Count == 0) {
                throw new InvalidOperationException("The buffer is empty");
            }

            var startPosition = _position;
            // Iterate all items 
            do {
                // Advance current position by one and circulate between 0 and max amount
                _position = (_position + 1) % _slots.Count;
                var circularStoreSlot = _slots[_position];
                if (circularStoreSlot.IsInUse) {
                    continue;
                }

                circularStoreSlot.IsInUse = true;
                _freeSlotCount++;
                return circularStoreSlot.Item;
            } while (startPosition != _position);
            
            throw new InvalidOperationException("No free slots");
        }

        public void Store(T item) {
            var slot = _slots.Find(s => Equals(s.Item, item));
            if (slot == null) {
                slot = new CircularStoreSlot<T>(item);
                _slots.Add(slot);
            }

            slot.IsInUse = false;
            _freeSlotCount++;
        }

    }

}