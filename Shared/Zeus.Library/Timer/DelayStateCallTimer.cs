using System;

namespace Zeus.Library.Timer {

    public class DelayStateCallTimer : Timer {

        public TimerStateCallback Callback { get; protected set; }

        public object State { get; protected set; }


        public DelayStateCallTimer(TimeSpan delay, TimeSpan interval, int count, TimerStateCallback callback, object state)
            : base(delay, interval, count) {
            Callback = callback;
            State = state;
        }

        protected override void OnTick() {
            if (Callback != null) {
                Callback(State);
            }
        }

        public override string ToString() {
            return String.Format("DelayStateCall[{0}]", FormatDelegate(Callback));
        }

    }


    public class DelayStateCallTimer<T> : Timer {

        public TimerStateCallback<T> Callback { get; protected set; }

        public T State { get; protected set; }


        public DelayStateCallTimer(TimeSpan delay, TimeSpan interval, int count, TimerStateCallback<T> callback, T state)
            : base(delay, interval, count) {
            Callback = callback;
            State = state;
        }

        protected override void OnTick() {
            if (Callback != null) {
                Callback(State);
            }
        }

        public override string ToString() {
            return String.Format("DelayStateCall[{0}]", FormatDelegate(Callback));
        }

    }

}