namespace Zeus.Library.Timer {

    public delegate void TimerCallback();

    public delegate void TimerStateCallback(object state);

    public delegate void TimerStateCallback<T>(T state);

}