using Zeus.CommunicationFramework.Messengers;

namespace Zeus.CommunicationFramework.Client {

    /// <summary>
    ///     Represents a client to connect to server.
    /// </summary>
    public interface IClient : IMessenger, IConnectableClient {

        // Does not define any additional member
    }

}