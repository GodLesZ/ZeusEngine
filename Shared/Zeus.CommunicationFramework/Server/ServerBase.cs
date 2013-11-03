using System;
using Zeus.CommunicationFramework.Channels;
using Zeus.CommunicationFramework.Protocols;
using Zeus.CommunicationFramework.Threading;

namespace Zeus.CommunicationFramework.Server {

    /// <summary>
    ///     This class provides base functionality for server classes.
    /// </summary>
    internal abstract class ServerBase : IServer {

        /// <summary>
        ///     Gets the underlying connection listener.
        /// </summary>
        public IConnectionListener Connection { get; private set; }

        /// <summary>
        ///     Gets the state of the connection - connected or not.
        /// </summary>
        public bool Connected {
            get {
                return Connection != null && Connection.IsActive;
            } 
        }

        /// <summary>
        ///     A collection of clients that are connected to the server.
        /// </summary>
        public ThreadSafeSortedList<long, IServerClient> Clients { get; private set; }

        /// <summary>
        ///     Gets/sets wire protocol that is used while reading and writing messages.
        /// </summary>
        public IWireProtocolFactory WireProtocolFactory { get; set; }

        /// <summary>
        ///     This event is raised when a new client is connected.
        /// </summary>
        public event EventHandler<ServerClientEventArgs> ClientConnected;

        /// <summary>
        ///     This event is raised when a client disconnected from the server.
        /// </summary>
        public event EventHandler<ServerClientEventArgs> ClientDisconnected;


        protected ServerBase() {
            Clients = new ThreadSafeSortedList<long, IServerClient>();
            WireProtocolFactory = WireProtocolManager.GetDefaultWireProtocolFactory();
        }


        /// <summary>
        ///     Starts the server.
        /// </summary>
        public virtual void Start() {
            Connection = CreateConnectionListener();
            Connection.CommunicationChannelConnected += ConnectionListener_CommunicationChannelConnected;
            Connection.Start();
        }

        /// <summary>
        ///     Stops the server connection (listener) and disconnects all clients.
        /// </summary>
        public virtual void Stop() {
            if (Connection != null) {
                Connection.Stop();
            }

            foreach (var client in Clients.GetAllItems()) {
                client.Disconnect();
            }
        }

        /// <summary>
        ///     This method is implemented by derived classes to create appropriate connection listener to listen incoming
        ///     connection requets.
        /// </summary>
        /// <returns></returns>
        protected abstract IConnectionListener CreateConnectionListener();

        /// <summary>
        ///     Handles CommunicationChannelConnected event of _connectionListener object.
        /// </summary>
        /// <param name="sender">Source of event</param>
        /// <param name="e">Event arguments</param>
        private void ConnectionListener_CommunicationChannelConnected(object sender, CommunicationChannelEventArgs e) {
            var client = new ServerClient(e.Channel) {
                ClientId = ServerManager.GetClientId(),
                WireProtocol = WireProtocolFactory.CreateWireProtocol()
            };

            client.Disconnected += Client_Disconnected;
            Clients[client.ClientId] = client;
            OnClientConnected(client);
            e.Channel.Start();
        }

        /// <summary>
        ///     Handles Disconnected events of all connected clients.
        /// </summary>
        /// <param name="sender">Source of event</param>
        /// <param name="e">Event arguments</param>
        private void Client_Disconnected(object sender, EventArgs e) {
            var client = (IServerClient)sender;
            Clients.Remove(client.ClientId);
            OnClientDisconnected(client);
        }

        /// <summary>
        ///     Raises ClientConnected event.
        /// </summary>
        /// <param name="client">Connected client</param>
        protected virtual void OnClientConnected(IServerClient client) {
            var handler = ClientConnected;
            if (handler != null) {
                handler(this, new ServerClientEventArgs(client));
            }
        }

        /// <summary>
        ///     Raises ClientDisconnected event.
        /// </summary>
        /// <param name="client">Disconnected client</param>
        protected virtual void OnClientDisconnected(IServerClient client) {
            var handler = ClientDisconnected;
            if (handler != null) {
                handler(this, new ServerClientEventArgs(client));
            }
        }

    }

}