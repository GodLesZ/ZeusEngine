using System;
using System.Reflection;
using Zeus.CommunicationFramework.Messages;
using Zeus.Library.Configuration;
using Zeus.Library.Configuration.Xml;
using Zeus.Server.Library.Tools;

namespace Zeus.Server.Library.Tests.TcpMessageServerTest {

    public class Program {
        private static SimpleTcpMessageServer _server;

        public static void Main(string[] args) {
            // Try access a config
            var conf = Factory.Create<Provider>("server-conf.xml");
            var dynConf = conf.AsExpando().configuration;

            // Prepare console for a large output
            var width = Math.Min(100, Console.LargestWindowWidth - 2);
            Console.CursorVisible = false;
            Console.Clear();
            Console.WindowLeft = Console.WindowTop = 0;
            if (Console.WindowWidth < width) {
                Console.WindowWidth = width;
            }

            Console.Title = "Simple TCP message server test";


            // Create a test server
            _server = new SimpleTcpMessageServer(dynConf.network.host, dynConf.network.port);
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
                if (cmd == "testobj") {

                    var msg = new TextMessage("Hello from server! äöüß");
                    _server.Broadcast(msg);
                }
            } while (_server.IsActive);

            ServerConsole.DebugLine("Press any key to exit.");
            Console.Read();
        }
    }

}