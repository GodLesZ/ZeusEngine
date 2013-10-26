using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Zeus.Library.Timer {

    public class TimerThread : IResetableTask {

        private static readonly Queue mChangeQueue = Queue.Synchronized(new Queue());
        private static readonly AutoResetEvent mSignal = new AutoResetEvent(false);

        private static readonly DateTime[] mNextPriorities = new DateTime[8];

        private static readonly TimeSpan[] mPriorityDelays = new TimeSpan[8] {
            TimeSpan.Zero,
            TimeSpan.FromMilliseconds(10.0),
            TimeSpan.FromMilliseconds(25.0),
            TimeSpan.FromMilliseconds(50.0),
            TimeSpan.FromMilliseconds(250.0),
            TimeSpan.FromSeconds(1.0),
            TimeSpan.FromSeconds(5.0),
            TimeSpan.FromMinutes(1.0)
        };

        private static readonly List<Timer>[] mTimers = new List<Timer>[8] {
            new List<Timer>(),
            new List<Timer>(),
            new List<Timer>(),
            new List<Timer>(),
            new List<Timer>(),
            new List<Timer>(),
            new List<Timer>(),
            new List<Timer>()
        };


        public static void DumpInfo(TextWriter tw) {
            for (var i = 0; i < 8; ++i) {
                tw.WriteLine("Priority: {0}", (TimerPriority)i);
                tw.WriteLine();

                var hash = new Dictionary<string, List<Timer>>();

                for (var j = 0; j < mTimers[i].Count; ++j) {
                    var t = mTimers[i][j];

                    var key = t.ToString();

                    List<Timer> list;
                    hash.TryGetValue(key, out list);

                    if (list == null) {
                        hash[key] = list = new List<Timer>();
                    }

                    list.Add(t);
                }

                foreach (var kv in hash) {
                    var key = kv.Key;
                    var list = kv.Value;

                    tw.WriteLine("Type: {0}; Count: {1}; Percent: {2}%", key, list.Count, (int)(100 * (list.Count / (double)mTimers[i].Count)));
                }

                tw.WriteLine();
                tw.WriteLine();
            }
        }

        public static void Change(Timer t, int newIndex, bool isAdd) {
            mChangeQueue.Enqueue(TimerChangeEntry.GetInstance(t, newIndex, isAdd));
            mSignal.Set();
        }

        public static void AddTimer(Timer t) {
            Change(t, (int)t.Priority, true);
        }

        public static void PriorityChange(Timer t, int newPrio) {
            Change(t, newPrio, false);
        }

        public static void RemoveTimer(Timer t) {
            Change(t, -1, false);
        }


        private static void ProcessChangeQueue() {
            while (mChangeQueue.Count > 0) {
                var tce = (TimerChangeEntry)mChangeQueue.Dequeue();
                var timer = tce.TimerObj;
                var newIndex = tce.NewIndex;

                if (timer.TimerList != null) {
                    timer.TimerList.Remove(timer);
                }

                if (tce.IsAdded) {
                    timer.Next = DateTime.Now + timer.Delay;
                    timer.Index = 0;
                }

                if (newIndex >= 0) {
                    timer.TimerList = mTimers[newIndex];
                    timer.TimerList.Add(timer);
                } else {
                    timer.TimerList = null;
                }

                tce.Free();
            }
        }

        public static void Set() {
            mSignal.Set();
        }

        public bool IsActive() {
            return true;
        }

        public void TimerMain(IResetableTask resetInterface) {
            DateTime now;
            int i, j;
            bool loaded;

            while (resetInterface.IsActive()) {
                ProcessChangeQueue();

                loaded = false;

                for (i = 0; i < mTimers.Length; i++) {
                    now = DateTime.Now;
                    if (now < mNextPriorities[i]) {
                        break;
                    }

                    mNextPriorities[i] = now + mPriorityDelays[i];

                    for (j = 0; j < mTimers[i].Count; j++) {
                        var t = mTimers[i][j];

                        if (!t.Queued && now > t.Next) {
                            t.Queued = true;

                            lock (Timer.Queue) {
                                Timer.Queue.Enqueue(t);
                            }

                            loaded = true;

                            if (t.Count != 0 && (++t.Index >= t.Count)) {
                                t.Stop();
                            } else {
                                t.Next = now + t.Interval;
                            }
                        }
                    }
                }

                if (loaded) {
                    resetInterface.Set();
                }

                mSignal.WaitOne(10, false);
            } // while resetInterface.IsActive()

        } // TimerMain()

        void IResetableTask.Set() {
            Set();
        }

    }

}