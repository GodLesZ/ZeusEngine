using System;
using Zeus.CommunicationFramework.Channels;
using Zeus.CommunicationFramework.Protocols;
using Zeus.CommunicationFramework.Threading;

namespace Zeus.CommunicationFramework.Server {

    /// <summary>
    ///     Represents a ZCF server that is used to accept and manage client connections.
    /// </summary>
    public interface IServer {

        /// <summary>
        ///     Gets the underlying connection listener.
        /// </summary>
        IConnectionListener Connection { get; }

        /// <summary>
        ///     A collection of clients that are connected to the server.
        /// </summary>
        ThreadSafeSortedList<long, IServerClient> Clients { get; }

        /// <summary>
        ///     Gets/sets wire protocol factory to create IWireProtocol objects.
        /// </summary>
        IWireProtocolFactory WireProtocolFactory { get; set; }

        /// <summary>
        ///     This event is raised when a new client connected to the server.
        /// </summary>
        event EventHandler<ServerClientEventArgs> ClientConnected;

        /// <summary>
        ///     This event is raised when a client disconnected from the server.
        /// </summary>
        event EventHandler<ServerClientEventArgs> ClientDisconnected;

        /// <summary>
        ///     Starts the server.
        /// </summary>
        void Start();

        /// <summary>
        ///     Stops the server.
        /// </summary>
        void Stop();

    }

}