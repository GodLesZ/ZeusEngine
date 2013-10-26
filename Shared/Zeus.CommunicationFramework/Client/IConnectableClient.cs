using System;

namespace Zeus.CommunicationFramework.Client {

    /// <summary>
    ///     Represents a client for ZCF servers.
    /// </summary>
    public interface IConnectableClient : IDisposable {

        /// <summary>
        ///     Gets the current communication state.
        /// </summary>
        CommunicationStates CommunicationState { get; }

        /// <summary>
        ///     Timeout for connecting to a server (as milliseconds).
        ///     Default value: 15 seconds (15000 ms).
        /// </summary>
        int ConnectTimeout { get; set; }

        /// <summary>
        ///     This event is raised when client connected to server.
        /// </summary>
        event EventHandler Connected;

        /// <summary>
        ///     This event is raised when client disconnected from server.
        /// </summary>
        event EventHandler Disconnected;

        /// <summary>
        ///     Connects to server.
        /// </summary>
        void Connect();

        /// <summary>
        ///     Disconnects from server.
        ///     Does nothing if already disconnected.
        /// </summary>
        void Disconnect();

    }

}