namespace Zeus.CommunicationFramework.Messages {

    /// <summary>
    ///     Represents a message that is sent and received by server and client.
    /// </summary>
    public interface IMessage {

        /// <summary>
        ///     Unique identified for this message.
        /// </summary>
        ushort Id { get; }

    }

}