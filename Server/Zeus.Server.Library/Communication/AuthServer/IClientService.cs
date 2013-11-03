using System.Collections.Generic;
using Zeus.CommunicationFramework.Services.Client;
using Zeus.CommunicationFramework.Services.Service;
using Zeus.Library.Shared;
using Zeus.Library.Shared.AuthServer;

namespace Zeus.Server.Library.Communication.AuthServer {

    /// <summary>
    ///     Service for communication between a client and the auth-server.
    ///     All methods will be available to the connected client.
    /// </summary>
    [Service(Version = "1.0.0.0")]
    public interface IClientService {

        IAccountInfo ClientLogin(string username, string password);

        IEnumerable<IServerDescription> GetServerDescriptions();

        IEnumerable<ICharacterInfo> SelectServer(IServerDescription server);

    }

}