using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zeus.CommunicationFramework.EndPoints;
using Zeus.CommunicationFramework.EndPoints.Tcp;
using Zeus.CommunicationFramework.Server;
using Zeus.CommunicationFramework.Services.Service;
using Zeus.Server.Library.Tests.TcpServiceServerTest.Shared;
using Zeus.Server.Library.Tools;

namespace Zeus.Server.Library.Tests.TcpServiceServerTest {

    // Note: Common interface for client and server are defined in this project.
    //       The client test project just uses this reference.

    public class Program {
        
        protected static IServiceApplication _server;
        

        public static void Main(string[] args) {
            var endPoint = ProtocolEndPointBase.CreateEndPoint("tcp://127.0.0.1:13378");

            _server = ServiceBuilder.CreateService(endPoint);
            _server.AddService<ITestServerService, TestServerService>(new TestServerService());
            
            _server.ClientConnected += ServerOnClientConnected;
            _server.ClientDisconnected += ServerOnClientDisconnected;

            _server.Start();
            
            ServerConsole.DebugLine("Service running on {0}", endPoint);
            Console.Read();
        }


        private static void ServerOnClientConnected(object sender, ServiceClientEventArgs args) {
            ServerConsole.InfoLine("Client connected: {0}", args.Client.RemoteEndPoint);
        }

        private static void ServerOnClientDisconnected(object sender, ServiceClientEventArgs args) {
            ServerConsole.InfoLine("Client disconnected: {0}", args.Client.RemoteEndPoint);
        }

    }

}
