using System;
using Zeus.CommunicationFramework.Messages;

namespace Zeus.CommunicationFramework.Services.Messages {

    /// <summary>
    ///     This message is sent to invoke a method of a remote application.
    /// </summary>
    [Serializable]
    public class RemoteInvokeMessage : Message {

        /// <summary>
        ///     Method of remote application to invoke.
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        ///     Parameters of method.
        /// </summary>
        public object[] Parameters { get; set; }

        /// <summary>
        ///     Name of the remove service class.
        /// </summary>
        public string ServiceClassName { get; set; }

        /// <summary>
        ///     Represents this object as string.
        /// </summary>
        /// <returns>String representation of this object</returns>
        public override string ToString() {
            return string.Format("ZcfRemoteInvokeMessage: {0}.{1}(...)", ServiceClassName, MethodName);
        }

    }

}