using System;
using Zeus.CommunicationFramework.EndPoints;
using Zeus.CommunicationFramework.Messages;
using Zeus.CommunicationFramework.Protocols;

namespace Zeus.CommunicationFramework.Channels {

    /// <summary>
    ///     This class provides base functionality for all communication channel classes.
    /// </summary>
    internal abstract class CommunicationChannelBase : ICommunicationChannel {

        /// <summary>
        ///     Gets the current communication state.
        /// </summary>
        public CommunicationStates CommunicationState { get; protected set; }

        /// <summary>
        ///     Gets the time of the last succesfully received message.
        /// </summary>
        public DateTime LastReceivedMessageTime { get; protected set; }

        /// <summary>
        ///     Gets the time of the last succesfully sent message.
        /// </summary>
        public DateTime LastSentMessageTime { get; protected set; }

        /// <summary>
        ///     Gets endpoint of remote application.
        /// </summary>
        public abstract ProtocolEndPointBase RemoteEndPoint { get; }

        /// <summary>
        ///     Gets/sets wire protocol that the channel uses.
        ///     This property must set before first communication.
        /// </summary>
        public IWireProtocol WireProtocol { get; set; }

        /// <summary>
        ///     This event is raised when a new message is received.
        /// </summary>
        public event EventHandler<MessageEventArgs> MessageReceived;

        /// <summary>
        ///     This event is raised when a new message is sent without any error.
        ///     It does not guaranties that message is properly handled and processed by remote application.
        /// </summary>
        public event EventHandler<MessageEventArgs> MessageSent;

        /// <summary>
        ///     This event is raised when communication channel closed.
        /// </summary>
        public event EventHandler Disconnected;

        /// <summary>
        ///     Constructor.
        /// </summary>
        protected CommunicationChannelBase() {
            CommunicationState = CommunicationStates.Disconnected;
            LastReceivedMessageTime = DateTime.MinValue;
            LastSentMessageTime = DateTime.MinValue;
        }

        /// <summary>
        ///     Disconnects from remote application and closes this channel.
        /// </summary>
        public abstract void Disconnect();

        /// <summary>
        ///     Starts the communication with remote application.
        /// </summary>
        public void Start() {
            StartInternal();
            CommunicationState = CommunicationStates.Connected;
        }

        /// <summary>
        ///     Sends a message to the remote application.
        /// </summary>
        /// <param name="message">Message to be sent</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException if message is null</exception>
        public void SendMessage(IMessage message) {
            if (message == null) {
                throw new ArgumentNullException("message");
            }

            SendMessageInternal(message);
        }

        /// <summary>
        ///     Starts the communication with remote application really.
        /// </summary>
        protected abstract void StartInternal();

        /// <summary>
        ///     Sends a message to the remote application.
        ///     This method is overrided by derived classes to really send to message.
        /// </summary>
        /// <param name="message">Message to be sent</param>
        protected abstract void SendMessageInternal(IMessage message);

        /// <summary>
        ///     Raises Disconnected event.
        /// </summary>
        protected virtual void OnDisconnected() {
            var handler = Disconnected;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        ///     Raises MessageReceived event.
        /// </summary>
        /// <param name="message">Received message</param>
        protected virtual void OnMessageReceived(IMessage message) {
            var handler = MessageReceived;
            if (handler != null) {
                handler(this, new MessageEventArgs(message));
            }
        }

        /// <summary>
        ///     Raises MessageSent event.
        /// </summary>
        /// <param name="message">Received message</param>
        protected virtual void OnMessageSent(IMessage message) {
            var handler = MessageSent;
            if (handler != null) {
                handler(this, new MessageEventArgs(message));
            }
        }

    }

}