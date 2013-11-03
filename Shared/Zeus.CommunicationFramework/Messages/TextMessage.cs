using System;

namespace Zeus.CommunicationFramework.Messages {

    /// <summary>
    ///     This message is used to send/receive a text as message data.
    /// </summary>
    [Serializable]
    public class TextMessage : Message {

        /// <summary>
        ///     Message text that is being transmitted.
        /// </summary>
        public string Text { get; set; }

        public TextMessage() {

        }

        public TextMessage(string text) {
            Text = text;
        }

        public TextMessage(string text, ushort repliedId)
            : this(text) {
            RepliedId = repliedId;
        }

        /// <summary>
        ///     Creates a string to represents this object.
        /// </summary>
        /// <returns>A string to represents this object</returns>
        public override string ToString() {
            if (RepliedId == 0) {
                return string.Format("TextMessage [{0}]: {1}", Id, Text);
            }

            return string.Format("TextMessage [{0}] Replied To [{1}]: {2}", Id, RepliedId, Text);
        }

    }

}