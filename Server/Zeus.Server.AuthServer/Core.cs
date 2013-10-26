using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Zeus.CommunicationFramework.Messages;
using Zeus.Server.AuthServer.Library;
using Zeus.Library.Configuration;
using Zeus.Library.Configuration.Xml;
using Zeus.Library.Configuration.Yaml;
using Zeus.Server.Library.Tools;
using Zeus.Server.Models;

namespace Zeus.Server.AuthServer {

    public class Core {

        private static ZeusAuthServer _server;

        public static void Main(string[] args) {
            // Try access a config
            var yamlConf = ConfigurationFactory.Create<YamlConfiguration>("conf/auth-server.yaml");
            var yamlDynamicConf = yamlConf.FirstAsExpando();

            //var xmlConf = ConfigurationFactory.Create<XmlConfiguration>("conf/auth-server.xml");
            //var xmlDynamicConf = xmlConf.AsExpando();

            // Create a test server
            _server = new ZeusAuthServer(yamlDynamicConf.network.host, yamlDynamicConf.network.port);


            // Prepare console for a large output
            var width = Math.Min(100, Console.LargestWindowWidth - 2);
            Console.CursorVisible = false;
            Console.Clear();
            Console.WindowLeft = Console.WindowTop = 0;
            if (Console.WindowWidth < width) {
                Console.WindowWidth = width;
            }

            var ver = Assembly.GetExecutingAssembly().GetName().Version;
            Console.Title = string.Format("Zeus auth-server v{0}.{1}", ver.Major, ver.Minor);


            // Create a test server
            _server = new ZeusAuthServer(yamlDynamicConf.network.host, yamlDynamicConf.network.port);
            _server.Start();
            
            do {

                var cmd = Console.ReadLine();
                if (cmd == "exit") {
                    _server.Stop();
                }
                if (cmd == "testmsg") {
                    var msg = new TextMessage("Hello from server! äöüß");
                    _server.Broadcast(msg);
                }
            } while (_server.IsActive);

            ServerConsole.WarningLine("Press any key to exit.");
            Console.Read();
        }

    }

}