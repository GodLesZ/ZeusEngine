using System;
using System.Collections.Generic;
using System.IO;

namespace Zeus.Library.Timer {

    public class Timer {

        private TimerPriority _priority;
        private bool _running;

        public static int BreakCount { get; set; }

        public int Count { get; private set; }

        public TimeSpan Delay { get; set; }

        public int Index { get; set; }

        public TimeSpan Interval { get; set; }

        public DateTime Next { get; set; }

        public TimerPriority Priority {
            get { return _priority; }
            set {
                if (_priority == value) {
                    return;
                }

                _priority = value;
                if (_running) {
                    TimerThread.PriorityChange(this, (int)_priority);
                }
            }
        }

        public static Queue<Timer> Queue { get; private set; }

        public bool Queued { get; set; }

        public bool Running {
            get { return _running; }
            set {
                if (value) {
                    Start();
                } else {
                    Stop();
                }
            }
        }

        public static long Ticks {
            get { return DateTime.Now.Ticks; }
        }

        public List<Timer> TimerList { get; set; }

        static Timer() {
            Queue = new Queue<Timer>();
            BreakCount = 20000;
        }


        public Timer(TimeSpan delay, TimeSpan interval, int count) {
            Delay = delay;
            Interval = interval;
            Count = count;
        }

        public Timer(TimeSpan delay)
            : this(delay, TimeSpan.Zero, 1) {
        }

        public Timer(TimeSpan delay, TimeSpan interval)
            : this(delay, interval, 0) {
        }


        public static void Slice() {
            lock (Queue) {
                for (var index = 0; index < BreakCount && Queue.Count != 0; index++) {
                    var t = Queue.Dequeue();
                    t.OnTick();
                    t.Queued = false;
                }
            }
        }

        public static void DumpInfo(TextWriter tw) {
            TimerThread.DumpInfo(tw);
        }


        public static TimerPriority ComputePriority(TimeSpan ts) {
            if (ts >= TimeSpan.FromMinutes(1.0)) {
                return TimerPriority.FiveSeconds;
            }

            if (ts >= TimeSpan.FromSeconds(10.0)) {
                return TimerPriority.OneSecond;
            }

            if (ts >= TimeSpan.FromSeconds(5.0)) {
                return TimerPriority.TwoFiftyMs;
            }

            if (ts >= TimeSpan.FromSeconds(2.5)) {
                return TimerPriority.FiftyMs;
            }

            if (ts >= TimeSpan.FromSeconds(1.0)) {
                return TimerPriority.TwentyFiveMs;
            }

            if (ts >= TimeSpan.FromSeconds(0.5)) {
                return TimerPriority.TenMs;
            }

            return TimerPriority.EveryTick;
        }

        public static bool IsValid(Timer t) {
            return (t != null && t.Running);
        }


        public static string FormatDelegate(Delegate callback) {
            if (callback == null) {
                return "null";
            }

            return String.Format("{0}.{1}", callback.Method.DeclaringType.FullName, callback.Method.Name);
        }


        public static Timer DelayCall(TimeSpan delay, TimerCallback callback) {
            return DelayCall(delay, TimeSpan.Zero, 1, callback);
        }

        public static Timer DelayCall(TimeSpan delay, TimeSpan interval, TimerCallback callback) {
            return DelayCall(delay, interval, 0, callback);
        }

        public static Timer DelayCall(TimeSpan delay, TimeSpan interval, int count, TimerCallback callback) {
            Timer t = new DelayCallTimer(delay, interval, count, callback);

            t.Priority = ComputePriority(count == 1 ? delay : interval);
            t.Start();

            return t;
        }

        public static Timer DelayCall(TimeSpan delay, TimerStateCallback callback, object state) {
            return DelayCall(delay, TimeSpan.Zero, 1, callback, state);
        }

        public static Timer DelayCall(TimeSpan delay, TimeSpan interval, TimerStateCallback callback, object state) {
            return DelayCall(delay, interval, 0, callback, state);
        }

        public static Timer DelayCall(TimeSpan delay, TimeSpan interval, int count, TimerStateCallback callback, object state) {
            Timer t = new DelayStateCallTimer(delay, interval, count, callback, state);

            t.Priority = ComputePriority(count == 1 ? delay : interval);
            t.Start();

            return t;
        }

        public static Timer DelayCall<T>(TimeSpan delay, TimerStateCallback<T> callback, T state) {
            return DelayCall(delay, TimeSpan.Zero, 1, callback, state);
        }

        public static Timer DelayCall<T>(TimeSpan delay, TimeSpan interval, TimerStateCallback<T> callback, T state) {
            return DelayCall(delay, interval, 0, callback, state);
        }

        public static Timer DelayCall<T>(TimeSpan delay, TimeSpan interval, int count, TimerStateCallback<T> callback, T state) {
            Timer t = new DelayStateCallTimer<T>(delay, interval, count, callback, state);

            t.Priority = ComputePriority(count == 1 ? delay : interval);
            t.Start();

            return t;
        }

        public void Start() {
            if (_running) {
                return;
            }

            _running = true;
            TimerThread.AddTimer(this);
        }

        public void Stop() {
            if (!_running) {
                return;
            }

            _running = false;
            TimerThread.RemoveTimer(this);
        }

        protected virtual void OnTick() {
        }


        public override string ToString() {
            return GetType().FullName;
        }

    }

}