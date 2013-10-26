using System;
using System.Runtime.Remoting.Proxies;
using Zeus.CommunicationFramework.EndPoints;
using Zeus.CommunicationFramework.Messengers;
using Zeus.CommunicationFramework.Server;

namespace Zeus.CommunicationFramework.Services.Service {

    /// <summary>
    ///     Implements IServiceClient.
    ///     It is used to manage and monitor a service client.
    /// </summary>
    internal class ServiceClient : IServiceClient {

        /// <summary>
        ///     This object is used to send messages to client.
        /// </summary>
        private readonly RequestReplyMessenger<IServerClient> _requestReplyMessenger;

        /// <summary>
        ///     Reference to underlying IZcfServerClient object.
        /// </summary>
        private readonly IServerClient _serverClient;

        /// <summary>
        ///     Last created proxy object to invoke remote medhods.
        /// </summary>
        private RealProxy _realProxy;

        /// <summary>
        ///     Unique identifier for this client.
        /// </summary>
        public long ClientId {
            get { return _serverClient.ClientId; }
        }

        /// <summary>
        ///     Gets the communication state of the Client.
        /// </summary>
        public CommunicationStates CommunicationState {
            get { return _serverClient.CommunicationState; }
        }

        /// <summary>
        ///     Gets endpoint of remote application.
        /// </summary>
        public ProtocolEndPointBase RemoteEndPoint {
            get { return _serverClient.RemoteEndPoint; }
        }

        /// <summary>
        ///     This event is raised when this client is disconnected from server.
        /// </summary>
        public event EventHandler Disconnected;

        /// <summary>
        ///     Creates a new ServiceClient object.
        /// </summary>
        /// <param name="serverClient">Reference to underlying IZcfServerClient object</param>
        /// <param name="requestReplyMessenger">RequestReplyMessenger to send messages</param>
        public ServiceClient(IServerClient serverClient, RequestReplyMessenger<IServerClient> requestReplyMessenger) {
            _serverClient = serverClient;
            _serverClient.Disconnected += Client_Disconnected;
            _requestReplyMessenger = requestReplyMessenger;
        }

        /// <summary>
        ///     Closes client connection.
        /// </summary>
        public void Disconnect() {
            _serverClient.Disconnect();
        }

        /// <summary>
        ///     Gets the client proxy interface that provides calling client methods remotely.
        /// </summary>
        /// <typeparam name="T">Type of client interface</typeparam>
        /// <returns>Client interface</returns>
        public T GetClientProxy<T>() where T : class {
            _realProxy = new RemoteInvokeProxy<T, IServerClient>(_requestReplyMessenger);
            return (T)_realProxy.GetTransparentProxy();
        }

        /// <summary>
        ///     Handles disconnect event of _serverClient object.
        /// </summary>
        /// <param name="sender">Source of event</param>
        /// <param name="e">Event arguments</param>
        private void Client_Disconnected(object sender, EventArgs e) {
            _requestReplyMessenger.Stop();
            OnDisconnected();
        }

        /// <summary>
        ///     Raises Disconnected event.
        /// </summary>
        private void OnDisconnected() {
            var handler = Disconnected;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

    }

}