using System;
using System.Collections.Generic;
using System.Linq;
using Zeus.CommunicationFramework.EndPoints;
using Zeus.CommunicationFramework.Services.Client;
using Zeus.CommunicationFramework.Services.Service;
using Zeus.Library.Models;
using Zeus.Library.Shared;
using Zeus.Library.Shared.AuthServer;
using Zeus.Server.Library.Communication.InterServer;
using Zeus.Server.Library.Tools;

namespace Zeus.Server.Library.Communication.AuthServer {

    /// <summary>
    ///     Implements the service for client to auth-server communication.
    /// </summary>
    public class ClientServiceImplementation : ServiceBase, IClientService {
        protected Dictionary<IServiceClient, IAccountInfo> _knownAcccounts;
        protected IAuthService _interClient;


        public ClientServiceImplementation(IAuthService interClient) {
            _interClient = interClient;

            _knownAcccounts = new Dictionary<IServiceClient, IAccountInfo>();
        }


        public IAccountInfo ClientLogin(string username, string password) {
            ServerConsole.DebugLine("Client login: {0} @ {1}", username, password);

            var accountInfo  = new AccountInfo(1337, username, password);
            // Same key twice means the same connection logged in again
            // @TODO: Verify this about not beeing a hack
            if (_knownAcccounts.ContainsKey(CurrentClient)) {
                _knownAcccounts.Remove(CurrentClient);
            }
            _knownAcccounts.Add(CurrentClient, accountInfo);

            return accountInfo;
        }


        public IEnumerable<IServerDescription> GetServerDescriptions() {
            ServerConsole.DebugLine("Client requests server descriptions");

            var currentAccount = GetCurrentAccount();
            if (currentAccount == null) {
                throw new NotLoggedInException();
            }

            return _interClient.GetServerDescriptions();
        }

        public IEnumerable<ICharacterInfo> SelectServer(IServerDescription server) {
            ServerConsole.DebugLine("Client selected server: {0}", server.Name);

            var currentAccount = GetCurrentAccount();
            if (currentAccount == null) {
                throw new NotLoggedInException();
            }

            var characters = _interClient.GetCharacterInfos(currentAccount, server);
            ServerConsole.DebugLine("Returning {0} character infos..", characters.Count());
            return characters;
        }


        private IAccountInfo GetCurrentAccount() {
            IAccountInfo account;
            return _knownAcccounts.TryGetValue(CurrentClient, out account) ? account : null;
        }

    }

}