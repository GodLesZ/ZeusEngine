namespace Zeus.CommunicationFramework.Protocols.JsonSerialization {

    public class JsonSerializationProtocolFactory : IWireProtocolFactory {

        /// <summary>
        ///     Creates a new Wire Protocol object.
        /// </summary>
        /// <returns>Newly created wire protocol object</returns>
        public IWireProtocol CreateWireProtocol() {
            return new JsonSerializationProtocol();
        }

    }

}