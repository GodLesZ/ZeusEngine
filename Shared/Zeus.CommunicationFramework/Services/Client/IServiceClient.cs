using Zeus.CommunicationFramework.Client;

namespace Zeus.CommunicationFramework.Services.Client {

    /// <summary>
    ///     Represents a service client that consumes a ZCF service.
    /// </summary>
    /// <typeparam name="T">Type of service interface</typeparam>
    public interface IServiceClient<out T> : IConnectableClient where T : class {

        /// <summary>
        ///     Reference to the service proxy to invoke remote service methods.
        /// </summary>
        T ServiceProxy { get; }

        /// <summary>
        ///     Timeout value when invoking a service method.
        ///     If timeout occurs before end of remote method call, an exception is thrown.
        ///     Use -1 for no timeout (wait indefinite).
        ///     Default value: 60000 (1 minute).
        /// </summary>
        int Timeout { get; set; }

    }

}