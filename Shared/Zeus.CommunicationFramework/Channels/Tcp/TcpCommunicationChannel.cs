using System;
using System.Net;
using System.Net.Sockets;
using Zeus.CommunicationFramework.EndPoints;
using Zeus.CommunicationFramework.EndPoints.Tcp;
using Zeus.CommunicationFramework.Messages;

namespace Zeus.CommunicationFramework.Channels.Tcp {

    /// <summary>
    ///     This class is used to communicate with a remote application over TCP/IP protocol.
    /// </summary>
    internal class TcpCommunicationChannel : CommunicationChannelBase {

        /// <summary>
        ///     Size of the buffer that is used to receive bytes from TCP socket.
        /// </summary>
        private const int ReceiveBufferSize = 4 * 1024; // 4KB

        /// <summary>
        ///     This buffer is used to receive bytes
        /// </summary>
        private readonly byte[] _buffer;

        /// <summary>
        ///     Socket object to send/reveice messages.
        /// </summary>
        private readonly Socket _clientSocket;

        private readonly TcpEndPoint _remoteEndPoint;

        /// <summary>
        ///     This object is just used for thread synchronizing (locking).
        /// </summary>
        private readonly object _syncLock;

        /// <summary>
        ///     A flag to control thread's running
        /// </summary>
        private volatile bool _running;

        /// <summary>
        ///     Gets the endpoint of remote application.
        /// </summary>
        public override ProtocolEndPointBase RemoteEndPoint {
            get { return _remoteEndPoint; }
        }

        /// <summary>
        ///     Creates a new TcpCommunicationChannel object.
        /// </summary>
        /// <param name="clientSocket">
        ///     A connected Socket object that is
        ///     used to communicate over network
        /// </param>
        public TcpCommunicationChannel(Socket clientSocket) {
            _clientSocket = clientSocket;
            _clientSocket.NoDelay = true;

            var ipEndPoint = (IPEndPoint)_clientSocket.RemoteEndPoint;
            _remoteEndPoint = new TcpEndPoint(ipEndPoint.Address.ToString(), ipEndPoint.Port);

            _buffer = new byte[ReceiveBufferSize];
            _syncLock = new object();
        }

        /// <summary>
        ///     Disconnects from remote application and closes channel.
        /// </summary>
        public override void Disconnect() {
            if (CommunicationState != CommunicationStates.Connected) {
                return;
            }

            _running = false;
            try {
                if (_clientSocket.Connected) {
                    _clientSocket.Close();
                }
            } catch {

            }

            CommunicationState = CommunicationStates.Disconnected;
            OnDisconnected();
        }

        /// <summary>
        ///     Starts the thread to receive messages from socket.
        /// </summary>
        protected override void StartInternal() {
            _running = true;
            _clientSocket.BeginReceive(_buffer, 0, _buffer.Length, 0, ReceiveCallback, null);
        }

        /// <summary>
        ///     Sends a message to the remote application.
        /// </summary>
        /// <param name="message">Message to be sent</param>
        protected override void SendMessageInternal(IMessage message) {
            // Send message
            var totalSent = 0;
            lock (_syncLock) {
                // Create a byte array from message according to current protocol
                var messageBytes = WireProtocol.GetBytes(message);
                // Send all bytes to the remote application
                while (totalSent < messageBytes.Length) {
                    var sent = _clientSocket.Send(messageBytes, totalSent, messageBytes.Length - totalSent, SocketFlags.None);
                    if (sent <= 0) {
                        throw new CommunicationException("Message could not be sent via TCP socket. Only " + totalSent + " bytes of " + messageBytes.Length +
                                                         " bytes are sent.");
                    }

                    totalSent += sent;
                }

                LastSentMessageTime = DateTime.Now;
                OnMessageSent(message);
            }
        }

        /// <summary>
        ///     This method is used as callback method in _clientSocket's BeginReceive method.
        ///     It reveives bytes from socker.
        /// </summary>
        /// <param name="ar">Asyncronous call result</param>
        private void ReceiveCallback(IAsyncResult ar) {
            if (!_running) {
                return;
            }

            try {
                // Get received bytes count
                var bytesRead = _clientSocket.EndReceive(ar);
                if (bytesRead > 0) {
                    LastReceivedMessageTime = DateTime.Now;

                    // Copy received bytes to a new byte array
                    var receivedBytes = new byte[bytesRead];
                    Array.Copy(_buffer, 0, receivedBytes, 0, bytesRead);

                    // Read messages according to current wire protocol
                    var messages = WireProtocol.CreateMessages(receivedBytes);

                    // Raise MessageReceived event for all received messages
                    foreach (var message in messages) {
                        OnMessageReceived(message);
                    }
                } else {
                    throw new CommunicationException("Tcp socket is closed");
                }

                // Read more bytes if still running
                if (_running) {
                    _clientSocket.BeginReceive(_buffer, 0, _buffer.Length, 0, ReceiveCallback, null);
                }
            } catch {
                Disconnect();
            }
        }

    }

}