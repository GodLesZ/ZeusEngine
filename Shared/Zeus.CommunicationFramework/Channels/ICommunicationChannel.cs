using System;
using Zeus.CommunicationFramework.EndPoints;
using Zeus.CommunicationFramework.Messengers;

namespace Zeus.CommunicationFramework.Channels {

    /// <summary>
    ///     Represents a communication channel.
    ///     A communication channel is used to communicate (send/receive messages) with a remote application.
    /// </summary>
    public interface ICommunicationChannel : IMessenger {

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
        ///     Starts the communication with remote application.
        /// </summary>
        void Start();

        /// <summary>
        ///     Closes messenger.
        /// </summary>
        void Disconnect();

    }

}