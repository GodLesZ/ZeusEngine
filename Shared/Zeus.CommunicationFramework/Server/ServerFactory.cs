using System;
using Zeus.CommunicationFramework.EndPoints;

namespace Zeus.CommunicationFramework.Server {

    /// <summary>
    ///     This class is used to create ZCF servers.
    /// </summary>
    public static class ServerFactory {

        /// <summary>
        ///     Creates a new ZCF server using an EndPoint.
        /// </summary>
        /// <param name="endPoint">Endpoint that represents address of the server</param>
        /// <returns>Created server</returns>
        public static IServer CreateServer(ProtocolEndPointBase endPoint) {
            return endPoint.CreateServer();
        }

        /// <summary>
        ///     Creates a EndPoint and a ZCF server using the given address.
        /// </summary>
        /// <typeparam name="T">ZcfEndPoint type</typeparam>
        /// <param name="address">Adresse as a single string, like tcp://127.0.0.1:1337</param>
        /// <returns>Created server</returns>
        public static IServer CreateServer<T>(string address) where T: ProtocolEndPointBase {
            // Get constructor for single address
            var endPointCtor = typeof(T).GetConstructor(new[] {
                typeof(string)
            });
            if (endPointCtor == null) {
                throw new ArgumentException("Failed to get string-constructor for type \"" + typeof(T) + "\"");
            }
            // Invoke ctor to get the end-point object
            var endPoint = (ProtocolEndPointBase)endPointCtor.Invoke(new object[] {
                address
            });
            // Create and return server object
            return endPoint.CreateServer();
        }

    }

}