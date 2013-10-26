using System;

namespace Zeus.Library.Timer {

    public class DelayCallTimer : Timer {

        public TimerCallback Callback { get; protected set; }


        public DelayCallTimer(TimeSpan delay, TimeSpan interval, int count, TimerCallback callback)
            : base(delay, interval, count) {
            Callback = callback;
        }

        protected override void OnTick() {
            if (Callback != null) {
                Callback();
            }
        }

        public override string ToString() {
            return String.Format("DelayCallTimer[{0}]", FormatDelegate(Callback));
        }

    }

}