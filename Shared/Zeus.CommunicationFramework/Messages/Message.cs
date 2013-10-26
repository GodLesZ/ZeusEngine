using System;

namespace Zeus.CommunicationFramework.Messages {

    /// <summary>
    ///     Represents a message that is sent and received by server and client.
    ///     This is the base class for all messages between .NET applications.
    /// </summary>
    [Serializable]
    public class Message : IReplyMessage {

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
            Id = 0;
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