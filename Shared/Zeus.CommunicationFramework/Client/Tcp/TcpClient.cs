using System.Net;
using Zeus.CommunicationFramework.Channels;
using Zeus.CommunicationFramework.Channels.Tcp;
using Zeus.CommunicationFramework.EndPoints.Tcp;

namespace Zeus.CommunicationFramework.Client.Tcp {

    /// <summary>
    ///     This class is used to communicate with server over TCP/IP protocol.
    /// </summary>
    internal class TcpClient : ClientBase {

        /// <summary>
        ///     The endpoint address of the server.
        /// </summary>
        private readonly TcpEndPoint _serverEndPoint;

        public TcpClient(TcpEndPoint serverEndPoint) {
            _serverEndPoint = serverEndPoint;
        }

        /// <summary>
        ///     Creates a communication channel using ServerIpAddress and ServerPort.
        /// </summary>
        /// <returns>Ready communication channel to communicate</returns>
        protected override ICommunicationChannel CreateCommunicationChannel() {
            var endpoint = new IPEndPoint(IPAddress.Parse(_serverEndPoint.IpAddress), _serverEndPoint.TcpPort);
            var socket = TcpHelper.ConnectToServer(endpoint, ConnectTimeout);
            return new TcpCommunicationChannel(socket);
        }

    }

}