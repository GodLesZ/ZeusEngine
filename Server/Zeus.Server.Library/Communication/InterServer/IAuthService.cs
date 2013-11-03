using System.Collections.Generic;
using Zeus.CommunicationFramework.Services.Service;
using Zeus.Library.Shared;

namespace Zeus.Server.Library.Communication.InterServer {

    /// <summary>
    ///     Service for communication between a inter-server and the auth-server.
    ///     All methods will be available to the connected auth-server.
    /// </summary>
    [Service(Version = "1.0.0.0")]
    public interface IAuthService {

        bool AuthServerLogin(string passphrase);

        List<IServerDescription> GetServerDescriptions();

        List<ICharacterInfo> GetCharacterInfos(IAccountInfo account, IServerDescription server);
         
    }

}