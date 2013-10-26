using System;

namespace Zeus.CommunicationFramework.Messages {

    /// <summary>
    ///     This message is used to send/receive a raw byte array as message data.
    /// </summary>
    [Serializable]
    public class RawDataMessage : Message {

        /// <summary>
        ///     Message data that is being transmitted.
        /// </summary>
        public byte[] MessageData { get; set; }

        public RawDataMessage() {

        }

        public RawDataMessage(byte[] messageData) {
            MessageData = messageData;
        }

        public RawDataMessage(byte[] messageData, ushort repliedId)
            : this(messageData) {
            RepliedId = repliedId;
        }

        /// <summary>
        ///     Creates a string to represents this object.
        /// </summary>
        /// <returns>A string to represents this object</returns>
        public override string ToString() {
            var messageLength = MessageData == null ? 0 : MessageData.Length;
            if (RepliedId == 0) {
                return string.Format("ZcfRawDataMessage [{0}]: {1} bytes", Id, messageLength);
            }

            return string.Format("ZcfRawDataMessage [{0}] Replied To [{1}]: {2} bytes", Id, RepliedId, messageLength);
        }

    }

}