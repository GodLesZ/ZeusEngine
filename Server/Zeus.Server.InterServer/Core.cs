using System;
using System.Reflection;
using Zeus.CommunicationFramework.EndPoints;
using Zeus.CommunicationFramework.Services.Service;
using Zeus.Library.Configuration;
using Zeus.Library.Configuration.Xml;
using Zeus.Server.Library.Communication.InterServer;
using Zeus.Server.Library.Tools;

namespace Zeus.Server.InterServer {

    public class Core {

        private static IServiceApplication _server;

        public static void Main(string[] args) {
            // Try access a config
            var serverConfiguration = Factory.Create<Provider>("conf/server.xml");
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
            Console.Title = string.Format("Zeus inter-server v{0}.{1}", ver.Major, ver.Minor);

            ServerConsole.StatusLine(Console.Title);

            // Create a service
            var endPointAddress = string.Format("tcp://{0}:{1}", dynamicConfig.network.host, dynamicConfig.network.port);
            _server = ServiceBuilder.CreateService(ProtocolEndPointBase.CreateEndPoint(endPointAddress));
            // Add interface for auth-server
            _server.AddService<IAuthService, AuthServiceImplementation>(new AuthServiceImplementation(dynamicConfig));
            // Add interface for world-server
            _server.AddService<IWorldService, WorldServiceImplementation>(new WorldServiceImplementation(dynamicConfig));
            // Start listener service
            _server.Start();

            ServerConsole.StatusLine("Inter-server is listening to: {0}", endPointAddress);

            do {

                var cmd = Console.ReadLine();
                if (cmd == "exit") {
                    _server.Stop();
                }

            } while (_server.Connected);

            ServerConsole.WarningLine("Press any key to exit.");
            Console.Read();
        }

    }

}