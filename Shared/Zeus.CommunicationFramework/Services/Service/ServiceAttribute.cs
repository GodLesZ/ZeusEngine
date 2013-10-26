using System;

namespace Zeus.CommunicationFramework.Services.Service {

    /// <summary>
    ///     Any ZCF Service interface class must has this attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    public class ServiceAttribute : Attribute {

        /// <summary>
        ///     Service Version. This property can be used to indicate the code version.
        ///     This value is sent to client application on an exception, so, client application can know that service version is
        ///     changed.
        ///     Default value: NO_VERSION.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        ///     Creates a new ZcfServiceAttribute object.
        /// </summary>
        public ServiceAttribute() {
            Version = "NO_VERSION";
        }

    }

}