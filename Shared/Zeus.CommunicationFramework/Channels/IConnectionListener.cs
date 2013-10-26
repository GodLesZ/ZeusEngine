using System;

namespace Zeus.CommunicationFramework.Channels {

    /// <summary>
    ///     Represents a communication listener.
    ///     A connection listener is used to accept incoming client connection requests.
    /// </summary>
    public interface IConnectionListener {

        /// <summary>
        ///     This event is raised when a new communication channel connected.
        /// </summary>
        event EventHandler<CommunicationChannelEventArgs> CommunicationChannelConnected;

        /// <summary>
        ///     Gets if the connection is still active.
        /// </summary>
        bool IsActive { get; }


        /// <summary>
        ///     Starts listening incoming connections.
        /// </summary>
        void Start();

        /// <summary>
        ///     Stops listening incoming connections.
        /// </summary>
        void Stop();

    }

}