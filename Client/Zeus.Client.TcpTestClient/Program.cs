using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zeus.CommunicationFramework;
using Zeus.CommunicationFramework.EndPoints;
using Zeus.CommunicationFramework.Services.Client;
using Zeus.Library.Configuration;
using Zeus.Library.Configuration.Xml;
using Zeus.Library.Shared;
using Zeus.Server.Library.Communication.AuthServer;

namespace Zeus.Client.TcpTestClient {

    public class Program {
        protected static IServiceClient<IClientService> _service;

        public static void Main(string[] args) {
            var serverConfiguration = ConfigurationFactory.Create<XmlConfiguration>("conf/client.xml");
            var dynamicConfig = serverConfiguration.FirstAsExpando().configuration;
                        
            string address = string.Format("tcp://{0}:{1}", dynamicConfig.zeus.host, dynamicConfig.zeus.port);
            var endPoint = ProtocolEndPointBase.CreateEndPoint(address);

            _service = ServiceClientBuilder.CreateClient<IClientService>(endPoint);
            _service.Connected += (o, a) => Console.WriteLine("Connection to auth-server successfull!");
            _service.Disconnected += (o, a) => Console.WriteLine("Connection to inter-server lost.");
            _service.ConnectTimeout = 30;
            _service.Connect();

            do {

                var cmd = Console.ReadLine();
                if (cmd == "exit") {
                    _service.Disconnect();
                }
                if (cmd == "login") {
                    IAccountInfo account = _service.ServiceProxy.ClientLogin("test", "test");
                    Console.WriteLine("Got account info: {0}", account);
                }
                if (cmd == "servers") {
                    var servers = _service.ServiceProxy.GetServerDescriptions();
                    Console.WriteLine("Got server info:");
                    servers.ToList().ForEach(s => Console.WriteLine("\t{0}", s));
                }

            } while (_service.CommunicationState == CommunicationStates.Connected);

            Console.WriteLine("Press any key to exit.");
            Console.Read();
        }

    }

}
