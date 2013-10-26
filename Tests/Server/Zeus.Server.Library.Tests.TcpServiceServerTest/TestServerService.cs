using System;
using System.Diagnostics;
using Zeus.CommunicationFramework.Services.Service;
using Zeus.Server.Library.Tests.TcpServiceServerTest.Shared;
using Zeus.Server.Library.Tools;

namespace Zeus.Server.Library.Tests.TcpServiceServerTest {

    public class TestServerService : ServiceBase, ITestServerService {

        /// <summary>
        ///     Invoked by the client including the parameter.
        /// </summary>
        /// <param name="poco"></param>
        public void SomeRemoteMagic(SimplePoco poco) {
            ServerConsole.DebugLine("Recv poco obj from client! {0}: {1}", poco.Id, poco.Title);
            Debug.WriteLine(poco);

            // Get client proxy and invoke a answer to the poco
            var proxy = CurrentClient.GetClientProxy<ITestClient>();
            proxy.SomeClientMagic("Got your poco, ty client!");
        }

        public void RaiseTheException() {
            // Will be delegated to the client
            throw new Exception("Test exception, raised on server");
        }

    }

}