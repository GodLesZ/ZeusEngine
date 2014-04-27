using System;
using Zeus.CommunicationFramework.Messages;

namespace Zeus.CommunicationFramework.Services.Messages {

    /// <summary>
    ///     This message is sent as response message to a <see cref="RemoteInvokeMessage" />.
    ///     It is used to send return value of method invocation.
    /// </summary>
    [Serializable]
    public class RemoteInvokeReturnMessage : Message {

        /// <summary>
        ///     If any exception occured during method invocation, this field contains Exception object.
        ///     If no exception occured, this field is null.
        /// </summary>
        public RemoteException RemoteException { get; set; }

        /// <summary>
        ///     Return value of remote method invocation.
        /// </summary>
        public object ReturnValue { get; set; }

        public RemoteInvokeReturnMessage() {
            
        }

        public RemoteInvokeReturnMessage(ushort id) {
            Id = id;
        }


        /// <summary>
        ///     Represents this object as string.
        /// </summary>
        /// <returns>String representation of this object</returns>
        public override string ToString() {
            return string.Format("RemoteInvokeReturnMessage: Returns {0}, Exception = {1}", ReturnValue, RemoteException);
        }

    }

}