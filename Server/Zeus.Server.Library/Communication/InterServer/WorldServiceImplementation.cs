using System.Collections.Generic;
using Zeus.CommunicationFramework.Services.Service;
using Zeus.Server.Library.Tools;

namespace Zeus.Server.Library.Communication.InterServer {

    /// <summary>
    ///     Implements the service for world-server to inter-server communication.
    /// </summary>
    public class WorldServiceImplementation : ServiceBase, IWorldService {

        /// <summary>
        ///     List of known world servers.
        /// </summary>
        protected List<IServiceClient> _knownServers;
        protected dynamic _serverConfig;


        public WorldServiceImplementation(dynamic serverConfig) {
            _serverConfig = serverConfig;

            _knownServers = new List<IServiceClient>();
        }


        public void WorldServerLogin(string passphrase) {
            ServerConsole.InfoLine("World-server successfull connected: {0}", CurrentClient.RemoteEndPoint);
            // @TODO: Validate against config for allowed connections

            _knownServers.Add(CurrentClient);
        }

    }

}