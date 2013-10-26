using Zeus.CommunicationFramework.EndPoints;

namespace Zeus.CommunicationFramework.Client {

    /// <summary>
    ///     This class is used to create ZCF clients to connect to a ZCF server.
    /// </summary>
    public static class ClientFactory {

        /// <summary>
        ///     Creates a new client to connect to a server using an end point.
        /// </summary>
        /// <param name="endpoint">End point of the server to connect it</param>
        /// <returns>Created TCP client</returns>
        public static IClient CreateClient(ProtocolEndPointBase endpoint) {
            return endpoint.CreateClient();
        }

        /// <summary>
        ///     Creates a new client to connect to a server using an end point.
        /// </summary>
        /// <param name="endpointAddress">End point address of the server to connect it</param>
        /// <returns>Created TCP client</returns>
        public static IClient CreateClient(string endpointAddress) {
            return CreateClient(ProtocolEndPointBase.CreateEndPoint(endpointAddress));
        }

    }

}