using Zeus.CommunicationFramework.Services.Service;

namespace Zeus.Server.Library.Communication.InterServer {

    /// <summary>
    ///     Service for communication between a inter-server and the world-server.
    ///     All methods will be available to the connected world-server.
    ///     @TODO: This should derive from the world-server interface for available commands.
    /// </summary>
    [Service(Version = "1.0.0.0")]
    public interface IWorldService {

        void WorldServerLogin(string passphrase);

    }

}