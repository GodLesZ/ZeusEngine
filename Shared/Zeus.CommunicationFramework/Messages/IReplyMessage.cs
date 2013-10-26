namespace Zeus.CommunicationFramework.Messages {

    /// <summary>
    ///     Represents a message that is sent and received by server and client.
    /// </summary>
    public interface IReplyMessage : IMessage {

        /// <summary>
        ///     Unique identified for the message this is replying to.
        /// </summary>
        ushort RepliedId { get; set; }

    }

}