using System;

namespace Zeus.CommunicationFramework.Services.Service {

    /// <summary>
    ///     Base class for all services that is serviced by <see cref="IServiceApplication" />.
    ///     A class must be derived from <see cref="ServiceBase" /> to serve as a ZCF service.
    /// </summary>
    public abstract class ServiceBase {

        /// <summary>
        ///     The current client for a thread that called service method.
        /// </summary>
        [ThreadStatic]
        private static IServiceClient _currentClient;

        /// <summary>
        ///     Gets the current client which called this service method.
        /// </summary>
        /// <remarks>
        ///     This property is thread-safe, if returns correct client when
        ///     called in a service method if the method is called by ZCF system,
        ///     else throws exception.
        /// </remarks>
        protected internal IServiceClient CurrentClient {
            get {
                if (_currentClient == null) {
                    throw new Exception("Client channel can not be obtained. CurrentClient property must be called by the thread which runs the service method.");
                }

                return _currentClient;
            }

            internal set { _currentClient = value; }
        }

    }

}