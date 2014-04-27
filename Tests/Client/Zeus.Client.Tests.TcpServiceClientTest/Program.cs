using System;
using Zeus.CommunicationFramework;
using Zeus.CommunicationFramework.EndPoints;
using Zeus.CommunicationFramework.Services.Client;
using Zeus.Server.Library.Tests.TcpServiceServerTest.Shared;

namespace Zeus.Client.Tests.TcpServiceClientTest {

    public class Program {

        private static IServiceClient<ITestServerService> _serviceClient;
        private static TestClientImplementation _client;


        public static void Main(string[] args) {
            var endPoint = ProtocolEndPointBase.CreateEndPoint("tcp://127.0.0.1:6900");

            _client = new TestClientImplementation();
            _serviceClient = ServiceClientBuilder.CreateClient<ITestServerService>(endPoint, _client);
            _serviceClient.Connected += ClientOnConnected;
            _serviceClient.Disconnected += ClientOnDisconnected;
            _serviceClient.Connect();

            do {
                var cmd = Console.ReadLine();
                if (cmd == "exit") {
                    _serviceClient.Disconnect();
                }
                if (cmd == "void") {
                    _serviceClient.ServiceProxy.RemoteVoid();
                }
                if (cmd == "magic") {
                    var poco = new SimplePoco {Id = 1, Title = "My POCO obj"};
                    _serviceClient.ServiceProxy.SomeRemoteMagic(poco);
                }
                if (cmd == "ex") {
                    _serviceClient.ServiceProxy.RaiseTheException();
                }
            } while (_serviceClient.CommunicationState == CommunicationStates.Connected);


            Console.WriteLine("Press any key to exit.");
            Console.Read();
        }

        private static void ClientOnDisconnected(object sender, EventArgs args) {
            Console.WriteLine("Disconnected");
        }

        private static void ClientOnConnected(object sender, EventArgs args) {
            Console.WriteLine("Connected");
        }

    }

}