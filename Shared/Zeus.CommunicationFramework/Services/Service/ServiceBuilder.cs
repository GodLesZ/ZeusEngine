using Zeus.CommunicationFramework.EndPoints;
using Zeus.CommunicationFramework.Server;

namespace Zeus.CommunicationFramework.Services.Service {

    /// <summary>
    ///     This class is used to build ZcfService applications.
    /// </summary>
    public static class ServiceBuilder {

        /// <summary>
        ///     Creates a new ZCF Service application using an EndPoint.
        /// </summary>
        /// <param name="endPoint">EndPoint that represents address of the service</param>
        /// <returns>Created ZCF service application</returns>
        public static IServiceApplication CreateService(ProtocolEndPointBase endPoint) {
            return new ServiceApplication(ServerFactory.CreateServer(endPoint));
        }

    }

}