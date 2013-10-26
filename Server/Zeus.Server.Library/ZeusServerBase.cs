using System;
using System.Collections.Generic;
using Zeus.CommunicationFramework.EndPoints.Tcp;
using Zeus.CommunicationFramework.Messages;
using Zeus.CommunicationFramework.Server;

namespace Zeus.Server.Library {

    /// <summary>
    ///     A base server interface used for auth and world server.
    /// </summary>
    public class ZeusServerBase {

        protected TcpEndPoint _endPoint;
        protected IServer _server;

        /// <summary>
        ///     A collection of clients that are connected to the server.
        /// </summary>
        public List<IServerClient> Clients {
            get { return _server.Clients.GetAllItems(); }
        }

        /// <summary>
        ///     Gets if the connection is still active.
        /// </summary>
        public bool IsActive {
            get { return _server != null && _server.Connection != null && _server.Connection.IsActive; }
        }


        public ZeusServerBase(string address, int port)
            : this(string.Format("{0}:{1}", address, port)) {
        }

        public ZeusServerBase(string address) {
            // Minimal address content validation
            if (string.IsNullOrEmpty(address)) {
                throw new ArgumentNullException("address");
            }

            // Create end-point and server
            _endPoint = new TcpEndPoint(address);
            _server = ServerFactory.CreateServer(_endPoint);

            // Attach client events 
            _server.ClientConnected += ServerOnClientConnected;
            _server.ClientDisconnected += ServerOnClientDisconnected;
        }


        /// <summary>
        ///     Starts the server loop in a new thread.
        /// </summary>
        /// <returns></returns>
        public virtual bool Start() {
            // Ensure the server is initialized and ready
            if (_server == null) {
                throw new Exception("Server not initialized");
            }

            // Server already online? 
            if (_server.Connection != null && _server.Connection.IsActive) {
                return false;
            }

            _server.Start();
            return true;
        }

        public virtual void Stop() {
            // Ensure the server is initialized and ready
            if (_server == null || _server.Connection == null || _server.Connection.IsActive == false) {
                return;
            }

            _server.Stop();
        }


        /// <summary>
        ///     Sends a <see cref="IMessage" /> to all connected clients.
        /// </summary>
        /// <param name="message"></param>
        public void Broadcast(IMessage message) {
            foreach (var client in Clients) {
                client.SendMessage(message);
            }
        }


        protected virtual void ServerOnClientDisconnected(object sender, ServerClientEventArgs args) {

        }

        protected virtual void ServerOnClientConnected(object sender, ServerClientEventArgs args) {
            // Install event handler
            var client = args.Client;

            client.MessageReceived += ClientOnMessageReceived;
            client.MessageSent += ClientOnMessageSent;
        }

        protected virtual void ClientOnMessageSent(object sender, MessageEventArgs args) {

        }

        protected virtual void ClientOnMessageReceived(object sender, MessageEventArgs args) {

        }

    }

}