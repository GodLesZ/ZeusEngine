using Zeus.CommunicationFramework.Protocols.BinarySerialization;

namespace Zeus.CommunicationFramework.Protocols {

    /// <summary>
    ///     This class is used to get default protocols.
    /// </summary>
    internal static class WireProtocolManager {

        /// <summary>
        ///     Creates a default wire protocol factory object to be used on communicating of applications.
        /// </summary>
        /// <returns>A new instance of default wire protocol</returns>
        public static IWireProtocolFactory GetDefaultWireProtocolFactory() {
            return new BinarySerializationProtocolFactory();
        }

        /// <summary>
        ///     Creates a default wire protocol object to be used on communicating of applications.
        /// </summary>
        /// <returns>A new instance of default wire protocol</returns>
        public static IWireProtocol GetDefaultWireProtocol() {
            return new BinarySerializationProtocol();
        }

    }

}