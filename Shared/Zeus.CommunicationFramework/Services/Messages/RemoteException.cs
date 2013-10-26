using System;
using System.Runtime.Serialization;

namespace Zeus.CommunicationFramework.Services.Messages {

    /// <summary>
    ///     Represents a Zcf Remote Exception.
    ///     This exception is used to send an exception from an application to another application.
    /// </summary>
    [Serializable]
    public class RemoteException : Exception {

        public RemoteException() {

        }

        public RemoteException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context) {

        }

        public RemoteException(string message)
            : base(message) {

        }

        public RemoteException(string message, Exception innerException)
            : base(message, innerException) {

        }

    }

}