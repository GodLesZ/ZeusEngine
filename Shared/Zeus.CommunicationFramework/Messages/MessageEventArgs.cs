using System;

namespace Zeus.CommunicationFramework.Messages {

    /// <summary>
    ///     Stores message to be used by an event.
    /// </summary>
    public class MessageEventArgs : EventArgs {

        /// <summary>
        ///     Message object that is associated with this event.
        /// </summary>
        public IMessage Message { get; protected set; }

        public MessageEventArgs(IMessage message) {
            Message = message;
        }

    }

}