using System.Collections.Generic;

namespace Zeus.Library.Timer {

    public class TimerChangeEntry {

        private static readonly Queue<TimerChangeEntry> _instancePool = new Queue<TimerChangeEntry>();

        public bool IsAdded { get; set; }

        public int NewIndex { get; set; }

        public Timer TimerObj { get; set; }


        public TimerChangeEntry(Timer t, int newIndex, bool isAdded) {
            TimerObj = t;
            NewIndex = newIndex;
            IsAdded = isAdded;
        }


        public void Free() {
            _instancePool.Enqueue(this);
        }


        public static TimerChangeEntry GetInstance(Timer t, int newIndex, bool isAdd) {
            TimerChangeEntry e;

            if (_instancePool.Count > 0) {
                e = _instancePool.Dequeue();

                if (e == null) {
                    e = new TimerChangeEntry(t, newIndex, isAdd);
                } else {
                    e.TimerObj = t;
                    e.NewIndex = newIndex;
                    e.IsAdded = isAdd;
                }
            } else {
                e = new TimerChangeEntry(t, newIndex, isAdd);
            }

            return e;
        }

    }

}