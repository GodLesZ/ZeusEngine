using System;
using Zeus.CommunicationFramework.EndPoints;
using Zeus.CommunicationFramework.Messengers;

namespace Zeus.CommunicationFramework.Server {

    /// <summary>
    ///     Represents a client from a perspective of a server.
    /// </summary>
    public interface IServerClient : IMessenger {

        /// <summary>
        ///     Unique identifier for this client in server.
        /// </summary>
        long ClientId { get; }

        /// <summary>
        ///     Gets the current communication state.
        /// </summary>
        CommunicationStates CommunicationState { get; }

        /// <summary>
        ///     Gets endpoint of remote application.
        /// </summary>
        ProtocolEndPointBase RemoteEndPoint { get; }

        /// <summary>
        ///     This event is raised when client disconnected from server.
        /// </summary>
        event EventHandler Disconnected;

        /// <summary>
        ///     Disconnects from server.
        /// </summary>
        void Disconnect();

    }

}