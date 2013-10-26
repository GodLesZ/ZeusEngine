using System;
using System.Collections.Generic;
using System.Reflection;
using Zeus.CommunicationFramework.Messages;
using Zeus.CommunicationFramework.Messengers;
using Zeus.CommunicationFramework.Server;
using Zeus.CommunicationFramework.Services.Messages;
using Zeus.CommunicationFramework.Threading;

namespace Zeus.CommunicationFramework.Services.Service {

    /// <summary>
    ///     Implements <see cref="IServiceApplication" /> and provides all functionallity.
    /// </summary>
    internal class ServiceApplication : IServiceApplication {

        /// <summary>
        ///     Represents a user service object.
        ///     It is used to invoke methods on a ZcfService object.
        /// </summary>
        private sealed class ServiceObject {

            /// <summary>
            ///     This collection stores a list of all methods of service object.
            ///     Key: Method name
            ///     Value: Informations about method.
            /// </summary>
            private readonly SortedList<string, MethodInfo> _methods;

            /// <summary>
            ///     The service object that is used to invoke methods on.
            /// </summary>
            public ServiceBase Service { get; private set; }

            /// <summary>
            ///     ZcfService attribute of Service object's class.
            /// </summary>
            public ServiceAttribute ServiceAttribute { get; private set; }

            /// <summary>
            ///     Creates a new ServiceObject.
            /// </summary>
            /// <param name="serviceInterfaceType">Type of service interface</param>
            /// <param name="service">The service object that is used to invoke methods on</param>
            public ServiceObject(Type serviceInterfaceType, ServiceBase service) {
                Service = service;
                var classAttributes = serviceInterfaceType.GetCustomAttributes(typeof(ServiceAttribute), true);
                if (classAttributes.Length <= 0) {
                    throw new Exception("Service interface (" + serviceInterfaceType.Name + ") must has ZcfService attribute.");
                }

                ServiceAttribute = classAttributes[0] as ServiceAttribute;
                _methods = new SortedList<string, MethodInfo>();
                foreach (var methodInfo in serviceInterfaceType.GetMethods()) {
                    _methods.Add(methodInfo.Name, methodInfo);
                }
            }

            /// <summary>
            ///     Invokes a method of Service object.
            /// </summary>
            /// <param name="methodName">Name of the method to invoke</param>
            /// <param name="parameters">Parameters of method</param>
            /// <returns>Return value of method</returns>
            public object InvokeMethod(string methodName, params object[] parameters) {
                // Check if there is a method with name methodName
                if (!_methods.ContainsKey(methodName)) {
                    throw new Exception("There is not a method with name '" + methodName + "' in service class.");
                }
                
                // Invoke method and return invoke result
                return _methods[methodName].Invoke(Service, parameters);
            }

        }

        /// <summary>
        ///     All connected clients to service.
        ///     Key: Client's unique Id.
        ///     Value: Reference to the client.
        /// </summary>
        private readonly ThreadSafeSortedList<long, IServiceClient> _serviceClients;

        /// <summary>
        ///     User service objects that is used to invoke incoming method invocation requests.
        ///     Key: Service interface type's name.
        ///     Value: Service object.
        /// </summary>
        private readonly ThreadSafeSortedList<string, ServiceObject> _serviceObjects;

        /// <summary>
        ///     Underlying server object to accept and manage client connections.
        /// </summary>
        private readonly IServer _zcfServer;

        /// <summary>
        ///     This event is raised when a new client connected to the service.
        /// </summary>
        public event EventHandler<ServiceClientEventArgs> ClientConnected;

        /// <summary>
        ///     This event is raised when a client disconnected from the service.
        /// </summary>
        public event EventHandler<ServiceClientEventArgs> ClientDisconnected;

        /// <summary>
        ///     Creates a new ZcfServiceApplication object.
        /// </summary>
        /// <param name="zcfServer">Underlying IZcfServer object to accept and manage client connections</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException if zcfServer argument is null</exception>
        public ServiceApplication(IServer zcfServer) {
            if (zcfServer == null) {
                throw new ArgumentNullException("zcfServer");
            }

            _zcfServer = zcfServer;
            _zcfServer.ClientConnected += ZcfServerClientConnected;
            _zcfServer.ClientDisconnected += ZcfServerClientDisconnected;
            _serviceObjects = new ThreadSafeSortedList<string, ServiceObject>();
            _serviceClients = new ThreadSafeSortedList<long, IServiceClient>();
        }

        /// <summary>
        ///     Starts service application.
        /// </summary>
        public void Start() {
            _zcfServer.Start();
        }

        /// <summary>
        ///     Stops service application.
        /// </summary>
        public void Stop() {
            _zcfServer.Stop();
        }

        /// <summary>
        ///     Adds a service object to this service application.
        ///     Only single service object can be added for a service interface type.
        /// </summary>
        /// <typeparam name="TServiceInterface">Service interface type</typeparam>
        /// <typeparam name="TServiceClass">
        ///     Service class type. Must be delivered from ZcfService and must implement
        ///     TServiceInterface.
        /// </typeparam>
        /// <param name="service">An instance of TServiceClass.</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException if service argument is null</exception>
        /// <exception cref="Exception">Throws Exception if service is already added before</exception>
        public void AddService<TServiceInterface, TServiceClass>(TServiceClass service)
            where TServiceClass : ServiceBase, TServiceInterface
            where TServiceInterface : class {
            if (service == null) {
                throw new ArgumentNullException("service");
            }

            var type = typeof(TServiceInterface);
            if (_serviceObjects[type.Name] != null) {
                throw new Exception("Service '" + type.Name + "' is already added before.");
            }

            _serviceObjects[type.Name] = new ServiceObject(type, service);
        }

        /// <summary>
        ///     Removes a previously added service object from this service application.
        ///     It removes object according to interface type.
        /// </summary>
        /// <typeparam name="TServiceInterface">Service interface type</typeparam>
        /// <returns>True: removed. False: no service object with this interface</returns>
        public bool RemoveService<TServiceInterface>()
            where TServiceInterface : class {
            return _serviceObjects.Remove(typeof(TServiceInterface).Name);
        }

        /// <summary>
        ///     Handles ClientConnected event of _zcfServer object.
        /// </summary>
        /// <param name="sender">Source of event</param>
        /// <param name="e">Event arguments</param>
        private void ZcfServerClientConnected(object sender, ServerClientEventArgs e) {
            var requestReplyMessenger = new RequestReplyMessenger<IServerClient>(e.Client);
            requestReplyMessenger.MessageReceived += Client_MessageReceived;
            requestReplyMessenger.Start();

            var serviceClient = ServiceClientFactory.CreateServiceClient(e.Client, requestReplyMessenger);
            _serviceClients[serviceClient.ClientId] = serviceClient;
            OnClientConnected(serviceClient);
        }

        /// <summary>
        ///     Handles ClientDisconnected event of _zcfServer object.
        /// </summary>
        /// <param name="sender">Source of event</param>
        /// <param name="e">Event arguments</param>
        private void ZcfServerClientDisconnected(object sender, ServerClientEventArgs e) {
            var serviceClient = _serviceClients[e.Client.ClientId];
            if (serviceClient == null) {
                return;
            }

            _serviceClients.Remove(e.Client.ClientId);
            OnClientDisconnected(serviceClient);
        }

        /// <summary>
        ///     Handles MessageReceived events of all clients, evaluates each message,
        ///     finds appropriate service object and invokes appropriate method.
        /// </summary>
        /// <param name="sender">Source of event</param>
        /// <param name="e">Event arguments</param>
        private void Client_MessageReceived(object sender, MessageEventArgs e) {
            // Get RequestReplyMessenger object (sender of event) to get client
            var requestReplyMessenger = (RequestReplyMessenger<IServerClient>)sender;

            // Cast message to ZcfRemoteInvokeMessage and check it
            var invokeMessage = e.Message as RemoteInvokeMessage;
            if (invokeMessage == null) {
                return;
            }

            try {
                // Get client object
                var client = _serviceClients[requestReplyMessenger.Messenger.ClientId];
                if (client == null) {
                    requestReplyMessenger.Messenger.Disconnect();
                    return;
                }

                // Get service object
                var serviceObject = _serviceObjects[invokeMessage.ServiceClassName];
                if (serviceObject == null) {
                    SendInvokeResponse(requestReplyMessenger, invokeMessage, null,
                        new RemoteException("There is no service with name '" + invokeMessage.ServiceClassName + "'"));
                    return;
                }

                // Invoke method
                try {
                    object returnValue;
                    // Set client to service, so user service can get client
                    // in service method using CurrentClient property.
                    serviceObject.Service.CurrentClient = client;
                    try {
                        returnValue = serviceObject.InvokeMethod(invokeMessage.MethodName, invokeMessage.Parameters);
                    } finally {
                        // Set CurrentClient as null since method call completed
                        serviceObject.Service.CurrentClient = null;
                    }

                    // Send method invocation return value to the client
                    SendInvokeResponse(requestReplyMessenger, invokeMessage, returnValue, null);
                } catch (TargetInvocationException ex) {
                    var innerEx = ex.InnerException;
                    SendInvokeResponse(requestReplyMessenger, invokeMessage, null,
                        new RemoteException(innerEx.Message + Environment.NewLine + "Service Version: " + serviceObject.ServiceAttribute.Version, innerEx));
                } catch (Exception ex) {
                    SendInvokeResponse(requestReplyMessenger, invokeMessage, null,
                        new RemoteException(ex.Message + Environment.NewLine + "Service Version: " + serviceObject.ServiceAttribute.Version, ex));
                }
            } catch (Exception ex) {
                SendInvokeResponse(requestReplyMessenger, invokeMessage, null, new RemoteException("An error occured during remote service method call.", ex));
            }
        }

        /// <summary>
        ///     Sends response to the remote application that invoked a service method.
        /// </summary>
        /// <param name="client">Client that sent invoke message</param>
        /// <param name="requestMessage">Request message</param>
        /// <param name="returnValue">Return value to send</param>
        /// <param name="exception">Exception to send</param>
        private static void SendInvokeResponse(IMessenger client, IMessage requestMessage, object returnValue, RemoteException exception) {
            try {
                client.SendMessage(
                    new RemoteInvokeReturnMessage {
                        RepliedId = requestMessage.Id,
                        ReturnValue = returnValue,
                        RemoteException = exception
                    });
            } catch {

            }
        }

        /// <summary>
        ///     Raises ClientConnected event.
        /// </summary>
        /// <param name="client"></param>
        private void OnClientConnected(IServiceClient client) {
            var handler = ClientConnected;
            if (handler != null) {
                handler(this, new ServiceClientEventArgs(client));
            }
        }

        /// <summary>
        ///     Raises ClientDisconnected event.
        /// </summary>
        /// <param name="client"></param>
        private void OnClientDisconnected(IServiceClient client) {
            var handler = ClientDisconnected;
            if (handler != null) {
                handler(this, new ServiceClientEventArgs(client));
            }
        }

    }

}