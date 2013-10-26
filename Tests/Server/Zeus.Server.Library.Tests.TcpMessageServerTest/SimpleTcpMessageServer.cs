using System;
using Zeus.CommunicationFramework.Messages;
using Zeus.CommunicationFramework.Server;
using Zeus.Server.Library.Tools;

namespace Zeus.Server.Library.Tests.TcpMessageServerTest {

    public class SimpleTcpMessageServer : ZeusServerBase {

        public SimpleTcpMessageServer(string address)
            : base(address) {
        }

        public SimpleTcpMessageServer(string address, int port)
            : base(address, port) {
        }

        public SimpleTcpMessageServer(string address, string port)
            : this(address, int.Parse(port)) {
        }


        /// <summary>
        ///     Starts the server loop in a new thread.
        /// </summary>
        /// <returns></returns>
        public override bool Start() {
            if (base.Start() == false) {
                return false;
            }

            ServerConsole.InfoLine("Server starts listening on: {0}", _endPoint);
            return true;
        }

        /// <summary>
        ///     Stops the server (listener) and disconnects all clients.
        /// </summary>
        /// <returns></returns>
        public override void Stop() {
            ServerConsole.InfoLine("Server stops listening..");
            _server.Stop();
        }


        protected override void ServerOnClientDisconnected(object sender, ServerClientEventArgs args) {
            ServerConsole.InfoLine("Client disconnected: {0}", args.Client.RemoteEndPoint);
            base.ServerOnClientDisconnected(sender, args);
        }

        protected override void ServerOnClientConnected(object sender, ServerClientEventArgs args) {
            ServerConsole.InfoLine("Client connected: {0}", args.Client.RemoteEndPoint);
            base.ServerOnClientConnected(sender, args);
        }

        protected override void ClientOnMessageSent(object sender, MessageEventArgs args) {
            base.ClientOnMessageSent(sender, args);
            var client = sender as IServerClient;
            if (client == null) {
                throw new ArgumentNullException("sender", "Internal server error - sender-client can not be null");
            }

            // Simple debug
            var textMessage = args.Message as TextMessage;
            if (textMessage != null) {
                ServerConsole.DebugLine("[{0}] SENT: {1}", client.RemoteEndPoint, textMessage.Text);
            } else {
                ServerConsole.DebugLine("[{0}] SENT: {1}", client.RemoteEndPoint, args.Message.GetType());
            }
        }

        protected override void ClientOnMessageReceived(object sender, MessageEventArgs args) {
            base.ClientOnMessageReceived(sender, args);
            var client = sender as IServerClient;
            if (client == null) {
                throw new ArgumentNullException("sender", "Internal server error - sender-client can not be null");
            }

            // Simple debug
            var textMessage = args.Message as TextMessage;
            if (textMessage != null) {
                ServerConsole.DebugLine("[{0}] RECV: {1}", client.RemoteEndPoint, textMessage.Text);
            } else {
                ServerConsole.DebugLine("[{0}] RECV: {1}", client.RemoteEndPoint, args.Message.GetType());
            }
        }

    }

}