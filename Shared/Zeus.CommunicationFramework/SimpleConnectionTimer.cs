using System;
using System.Threading;

namespace Zeus.CommunicationFramework {

    /// <summary>
    ///     This class is a timer that performs some tasks periodically.
    /// </summary>
    public class SimpleConnectionTimer {

        private readonly System.Threading.Timer _taskTimer;
        private volatile bool _performingTasks;
        private volatile bool _running;

        /// <summary>
        ///     Task period of timer (as milliseconds).
        /// </summary>
        public int Period { get; set; }

        /// <summary>
        ///     Indicates whether timer raises Elapsed event on Start method of Timer for once.
        ///     Default: False.
        /// </summary>
        public bool RunOnStart { get; set; }

        /// <summary>
        ///     This event is raised periodically according to Period of Timer.
        /// </summary>
        public event EventHandler Elapsed;


        public SimpleConnectionTimer(int period)
            : this(period, false) {

        }

        public SimpleConnectionTimer(int period, bool runOnStart) {
            Period = period;
            RunOnStart = runOnStart;
            _taskTimer = new System.Threading.Timer(TimerCallBack, null, Timeout.Infinite, Timeout.Infinite);
        }


        /// <summary>
        ///     Starts the timer.
        /// </summary>
        public void Start() {
            _running = true;
            _taskTimer.Change(RunOnStart ? 0 : Period, Timeout.Infinite);
        }

        /// <summary>
        ///     Stops the timer.
        /// </summary>
        public void Stop() {
            lock (_taskTimer) {
                _running = false;
                _taskTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        /// <summary>
        ///     Waits the service to stop.
        /// </summary>
        public void WaitToStop() {
            lock (_taskTimer) {
                while (_performingTasks) {
                    Monitor.Wait(_taskTimer);
                }
            }
        }


        /// <summary>
        ///     This method is called by _taskTimer.
        /// </summary>
        /// <param name="state">Not used argument</param>
        private void TimerCallBack(object state) {
            lock (_taskTimer) {
                if (!_running || _performingTasks) {
                    return;
                }

                _taskTimer.Change(Timeout.Infinite, Timeout.Infinite);
                _performingTasks = true;
            }

            try {
                if (Elapsed != null) {
                    Elapsed(this, new EventArgs());
                }
            } catch {

            } finally {
                lock (_taskTimer) {
                    _performingTasks = false;
                    if (_running) {
                        _taskTimer.Change(Period, Timeout.Infinite);
                    }

                    Monitor.Pulse(_taskTimer);
                }
            }
        }

    }

}