using System;

namespace Zeus.CommunicationFramework.Messages {

    /// <summary>
    ///     This message is used to send/receive ping messages.
    ///     Ping messages is used to keep connection alive between server and client.
    /// </summary>
    [Serializable]
    public sealed class PingMessage : Message {

        public PingMessage() {
            
        }

        public PingMessage(ushort repliedId)
            : this() {
            RepliedId = repliedId;
        }

        /// <summary>
        ///     Creates a string to represents this object.
        /// </summary>
        /// <returns>A string to represents this object</returns>
        public override string ToString() {
            if (RepliedId == 0) {
                return string.Format("PingMessage [{0}]", Id);
            }

            return string.Format("PingMessage [{0}] Replied To [{1}]", Id, RepliedId);
        }

    }

}