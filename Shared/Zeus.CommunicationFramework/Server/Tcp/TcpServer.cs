using Zeus.CommunicationFramework.Channels;
using Zeus.CommunicationFramework.Channels.Tcp;
using Zeus.CommunicationFramework.EndPoints.Tcp;

namespace Zeus.CommunicationFramework.Server.Tcp {

    /// <summary>
    ///     This class is used to create a TCP server.
    /// </summary>
    internal class TcpServer : ServerBase {

        /// <summary>
        ///     The endpoint address of the server to listen incoming connections.
        /// </summary>
        private readonly TcpEndPoint _endPoint;

        public TcpServer(TcpEndPoint endPoint) {
            _endPoint = endPoint;
        }

        /// <summary>
        ///     Creates a TCP connection listener.
        /// </summary>
        /// <returns>Created listener object</returns>
        protected override IConnectionListener CreateConnectionListener() {
            return new TcpConnectionListener(_endPoint);
        }

    }

}