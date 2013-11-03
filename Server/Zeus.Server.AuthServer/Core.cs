using System;
using System.Linq;
using System.Reflection;
using Zeus.CommunicationFramework.EndPoints;
using Zeus.CommunicationFramework.Services.Client;
using Zeus.CommunicationFramework.Services.Service;
using Zeus.Library.Configuration;
using Zeus.Library.Configuration.Xml;
using Zeus.Library.Shared;
using Zeus.Server.Library.Communication.AuthServer;
using Zeus.Server.Library.Communication.InterServer;
using Zeus.Server.Library.Tools;

namespace Zeus.Server.AuthServer {

    public class Core {

        private static IServiceApplication _clientService;
        private static IServiceClient<IAuthService> _interClient;

        public static void Main(string[] args) {
            // Try access a config
            var serverConfiguration = ConfigurationFactory.Create<XmlConfiguration>("conf/server.xml");
            var dynamicConfig = serverConfiguration.FirstAsExpando().configuration;

            // Prepare console for a large output
#if WINDOWS
            var width = Math.Min(100, Console.LargestWindowWidth - 2);
            Console.CursorVisible = false;
            Console.Clear();
            Console.WindowLeft = Console.WindowTop = 0;
            if (Console.WindowWidth < width) {
                Console.WindowWidth = width;
            }
#endif

            var ver = Assembly.GetExecutingAssembly().GetName().Version;
            Console.Title = string.Format("Zeus auth-server v{0}.{1}", ver.Major, ver.Minor);

            ServerConsole.StatusLine(Console.Title);

            // Create inter-server listener service
            string interServerEndPointAddress = string.Format("tcp://{0}:{1}", dynamicConfig.network.inter_server.host, dynamicConfig.network.inter_server.port);
            var interServerEndPoint = ProtocolEndPointBase.CreateEndPoint(interServerEndPointAddress);

            ServerConsole.Info("Start connecting to intser-server..");
            // @TODO: Is any action coming from inter-server to auth-server?
            // @TODO: Create inter -> auth service (character goes back to char select)
            _interClient = ServiceClientBuilder.CreateClient<IAuthService>(interServerEndPoint, new ServerServiceImplementation());
            _interClient.Connected += (o, a) => {
                ServerConsole.WriteLine(ServerConsoleColor.Status, " successfull!");
                // AuthServerLogin 
                _interClient.ServiceProxy.AuthServerLogin((string)dynamicConfig.network.inter_server.password);
            };
            _interClient.Disconnected += delegate {
                ServerConsole.ErrorLine("Connection to inter-server lost.");
                // @TODO: Reconnect?
            };
            _interClient.ConnectTimeout = 30;
            _interClient.Connect();

            // Create a client listener service
            string endPointAddress = string.Format("tcp://{0}:{1}", dynamicConfig.network.host, dynamicConfig.network.port);
            _clientService = ServiceBuilder.CreateService(ProtocolEndPointBase.CreateEndPoint(endPointAddress));
            _clientService.ClientConnected += (sender, clientEventArgs) => ServerConsole.DebugLine("Client connected #{0}", clientEventArgs.Client.ClientId);
            _clientService.ClientDisconnected += (sender, clientEventArgs) => ServerConsole.DebugLine("Client disconnected #{0}", clientEventArgs.Client.ClientId);
            // Add interface for client connections
            var clientService = new ClientServiceImplementation(_interClient.ServiceProxy);
            _clientService.AddService<IClientService, ClientServiceImplementation>(clientService);
            // Start listener service
            _clientService.Start();

            ServerConsole.StatusLine("Auth-server is listening to: {0}", endPointAddress);

            do {

                var cmd = Console.ReadLine();
                if (cmd == "exit") {
                    _clientService.Stop();
                }
                
            } while (_clientService.Connected);

            ServerConsole.WarningLine("Press any key to exit.");
            Console.Read();
        }

    }

}