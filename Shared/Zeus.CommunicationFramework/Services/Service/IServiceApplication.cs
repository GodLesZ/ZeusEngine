using System;

namespace Zeus.CommunicationFramework.Services.Service {

    /// <summary>
    ///     Represents a ZCF service application that is used to construct and manage a ZCF service.
    /// </summary>
    public interface IServiceApplication {

        /// <summary>
        ///     Gets the state of the underyling server connection - connected or not.
        /// </summary>
        bool Connected { get; }

        /// <summary>
        ///     This event is raised when a new client connected to the service.
        /// </summary>
        event EventHandler<ServiceClientEventArgs> ClientConnected;

        /// <summary>
        ///     This event is raised when a client disconnected from the service.
        /// </summary>
        event EventHandler<ServiceClientEventArgs> ClientDisconnected;

        /// <summary>
        ///     Starts service application.
        /// </summary>
        void Start();

        /// <summary>
        ///     Stops service application.
        /// </summary>
        void Stop();

        /// <summary>
        ///     Adds a service object to this service application.
        ///     Only single service object can be added for a service interface type.
        /// </summary>
        /// <typeparam name="TServiceInterface">Service interface type</typeparam>
        /// <typeparam name="TServiceClass">
        ///     Service class type. Must be delivered from <see cref="ServiceBase" /> and must implement
        ///     TServiceInterface.
        /// </typeparam>
        /// <param name="service">An instance of TServiceClass.</param>
        void AddService<TServiceInterface, TServiceClass>(TServiceClass service)
            where TServiceClass : ServiceBase, TServiceInterface
            where TServiceInterface : class;

        /// <summary>
        ///     Removes a previously added service object from this service application.
        ///     It removes object according to interface type.
        /// </summary>
        /// <typeparam name="TServiceInterface">Service interface type</typeparam>
        /// <returns>True: removed. False: no service object with this interface</returns>
        bool RemoveService<TServiceInterface>() where TServiceInterface : class;

        /// <summary>
        ///     Returns the installed service object.
        /// </summary>
        /// <typeparam name="TServiceInterface">Service interface type</typeparam>
        /// <returns>The service object</returns>
        TServiceInterface GetService<TServiceInterface>() where TServiceInterface : class;
    }

}