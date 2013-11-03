using System;

namespace Zeus.CommunicationFramework.Messages {

    /// <summary>
    ///     Represents a message that is sent and received by server and client.
    ///     This is the base class for all messages between .NET applications.
    /// </summary>
    [Serializable]
    public class Message : IMessage {
        protected static ushort _messageId = 1;

        /// <summary>
        ///     Unique identified for this message.
        ///     Default value: 0
        /// </summary>
        public ushort Id { get; set; }

        /// <summary>
        ///     Unique identified for the message this is replying to.
        /// </summary>
        public ushort RepliedId { get; set; }


        protected Message() {
            // @FIX: Temp. fix, each message needs a unique id, this will kill ushort a way to fast
            // @TODO: A Message sent by a service (RemoteInvokeMessage) need a unique ID
            Id = _messageId++;
        }

        public Message(ushort id) {
            Id = id;
        }

        public Message(ushort id, ushort replyId)
            : this(id) {
            RepliedId = replyId;
        }


        /// <summary>
        ///     Creates a string to represents this object.
        /// </summary>
        /// <returns>A string to represents this object</returns>
        public override string ToString() {
            if (RepliedId == 0) {
                return string.Format("Message [{0}]", Id);
            }

            return string.Format("Message [{0}] Replied To [{1}]", Id, RepliedId);
        }

    }

}