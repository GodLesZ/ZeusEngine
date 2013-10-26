using System;
using Zeus.CommunicationFramework;
using Zeus.CommunicationFramework.Client;
using Zeus.CommunicationFramework.Messages;

namespace Zeus.Client.Tests.TcpMessageClientTest {

    public class Program {

        public static void Main(string[] args) {
            Console.Title = string.Format("Tcp-client test");

            var client = ClientFactory.CreateClient("127.0.0.1:1337");
            client.Connected += ClientOnConnected;
            client.Disconnected += ClientOnDisconnected;
            client.MessageReceived += ClientOnMessageReceived;
            client.MessageSent += ClientOnMessageSent;
            client.Connect();

            do {
                var cmd = Console.ReadLine();
                if (cmd == "exit") {
                    client.Disconnect();
                }
                if (cmd == "testmsg") {
                    var msg = new TextMessage("Hello from client! äöüß");
                    client.SendMessage(msg);
                }
            } while (client.CommunicationState == CommunicationStates.Connected);


            Console.WriteLine("Press any key to exit.");
            Console.Read();
        }

        private static void ClientOnMessageSent(object sender, MessageEventArgs args) {
            var textMessage = args.Message as TextMessage;
            if (textMessage != null) {
                Console.WriteLine("SENT: {0}", textMessage.Text);
            } else {
                Console.WriteLine("SENT: {0}", args.Message.GetType());
            }
        }

        private static void ClientOnMessageReceived(object sender, MessageEventArgs args) {
            var textMessage = args.Message as TextMessage;
            if (textMessage != null) {
                Console.WriteLine("RECV: {0}", textMessage.Text);
            } else {
                Console.WriteLine("RECV: {0}", args.Message.GetType());
            }
        }

        private static void ClientOnDisconnected(object sender, EventArgs args) {
            Console.WriteLine("Disconnected");
        }

        private static void ClientOnConnected(object sender, EventArgs args) {
            Console.WriteLine("Connected");
        }

    }

}