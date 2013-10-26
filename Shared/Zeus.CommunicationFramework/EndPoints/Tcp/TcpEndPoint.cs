using System;
using Zeus.CommunicationFramework.Client;
using Zeus.CommunicationFramework.Client.Tcp;
using Zeus.CommunicationFramework.Server;
using Zeus.CommunicationFramework.Server.Tcp;

namespace Zeus.CommunicationFramework.EndPoints.Tcp {

    /// <summary>
    ///     Represens a TCP end point in ZCF.
    /// </summary>
    public sealed class TcpEndPoint : ProtocolEndPointBase {

        /// <summary>
        ///     IP address of the server.
        /// </summary>
        public string IpAddress { get; private set; }

        /// <summary>
        ///     Listening TCP Port for incoming connection requests on server.
        /// </summary>
        public int TcpPort { get; private set; }


        public TcpEndPoint(int tcpPort) {
            TcpPort = tcpPort;
        }

        public TcpEndPoint(string ipAddress, int port) {
            IpAddress = ipAddress;
            TcpPort = port;
        }

        /// <summary>
        ///     Address format must be like IPAddress:Port (For example: 127.0.0.1:10085).
        /// </summary>
        /// <param name="address">TCP end point Address</param>
        public TcpEndPoint(string address) {
            var splittedAddress = address.Trim().Split(':');
            IpAddress = splittedAddress[0].Trim();
            TcpPort = Convert.ToInt32(splittedAddress[1].Trim());
        }

        /// <summary>
        ///     Creates a Zcf Server that uses this end point to listen incoming connections.
        /// </summary>
        /// <returns>Zcf Server</returns>
        internal override IServer CreateServer() {
            return new TcpServer(this);
        }

        /// <summary>
        ///     Creates a Zcf Client that uses this end point to connect to server.
        /// </summary>
        /// <returns>Zcf Client</returns>
        internal override IClient CreateClient() {
            return new TcpClient(this);
        }

        /// <summary>
        ///     Generates a string representation of this end point object.
        /// </summary>
        /// <returns>String representation of this end point object</returns>
        public override string ToString() {
            return string.IsNullOrEmpty(IpAddress) ? ("tcp://" + TcpPort) : ("tcp://" + IpAddress + ":" + TcpPort);
        }

    }

}