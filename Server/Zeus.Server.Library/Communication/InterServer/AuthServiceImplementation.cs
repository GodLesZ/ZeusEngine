using System.Collections.Generic;
using Zeus.CommunicationFramework.Services.Service;
using Zeus.Library.Models;
using Zeus.Library.Shared;
using Zeus.Server.Library.Tools;

namespace Zeus.Server.Library.Communication.InterServer {

    /// <summary>
    ///     Implements the service for auth-server to inter-server communication.
    /// </summary>
    public class AuthServiceImplementation : ServiceBase, IAuthService {

        /// <summary>
        ///     List of known auth-servers
        /// </summary>
        protected List<IServiceClient> _knownServers;
        protected dynamic _serverConfig;


        public AuthServiceImplementation(dynamic serverConfig) {
            _serverConfig = serverConfig;

            _knownServers = new List<IServiceClient>();
        }


        public bool AuthServerLogin(string passphrase) {
            ServerConsole.InfoLine("Auth-server successfull connected: {0}", CurrentClient.RemoteEndPoint);
            // @TODO: Validate against config for allowed connections

            _knownServers.Add(CurrentClient);

            return true;
        }

        public List<IServerDescription> GetServerDescriptions() {
            ServerConsole.DebugLine("Auth-server requested server descriptions of known world server types..");

            // @TODO: Get list of world servers here
            return new List<IServerDescription>(new [] {
                new ServerDescription("Test server")
            });
        }

        public List<ICharacterInfo> GetCharacterInfos(IAccountInfo account, IServerDescription server) {
            ServerConsole.DebugLine("Auth-server requested character list for account: {0}", account.Username);

            return new List<ICharacterInfo>(new[] {
                new CharacterInfo(815, "Test Character")
            });
        }

    }

}