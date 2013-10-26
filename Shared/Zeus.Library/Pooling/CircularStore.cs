using System;
using System.Collections.Generic;

namespace Zeus.Library.Pooling {

    public class CircularStore<T> : IPoolItemStore<T> {

        protected int freeSlotCount;
        protected int position = -1;
        protected List<CircularStoreSlot<T>> slots;

        public int Count {
            get { return freeSlotCount; }
        }


        public CircularStore(int capacity) {
            slots = new List<CircularStoreSlot<T>>(capacity);
        }


        public T Fetch() {
            if (Count == 0) {
                throw new InvalidOperationException("The buffer is empty.");
            }

            var startPosition = position;
            do {
                Advance();
                var circularStoreSlot = slots[position];
                if (circularStoreSlot.IsInUse) {
                    continue;
                }

                circularStoreSlot.IsInUse = true;
                --freeSlotCount;
                return circularStoreSlot.Item;
            } while (startPosition != position);
            throw new InvalidOperationException("No free slots.");
        }

        public void Store(T item) {
            var slot = slots.Find(s => Equals(s.Item, item));
            if (slot == null) {
                slot = new CircularStoreSlot<T>(item);
                slots.Add(slot);
            }

            slot.IsInUse = false;
            ++freeSlotCount;
        }

        private void Advance() {
            position = (position + 1) % slots.Count;
        }

    }

}