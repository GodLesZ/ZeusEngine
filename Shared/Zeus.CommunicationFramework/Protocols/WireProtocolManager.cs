using System;
using System.Collections.Generic;
using System.Linq;

namespace Zeus.CommunicationFramework.Protocols {

    /// <summary>
    ///     This class is used to get default protocols.
    /// </summary>
    public static class WireProtocolManager {
        internal static Dictionary<string, Type> WireProtocolMap;
        internal static Dictionary<string, Type> WireProtocolFactoryMap;


        static WireProtocolManager() {
            // Fill our wireprotocol map
            WireProtocolMap = new Dictionary<string, Type>();

            // @TODO: This disables custom implementations in 3rd-party assemblies
            //var asm = Assembly.GetAssembly(typeof (WireProtocolManager));
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            // Get IWireProtocol types from this assembly
            var types =
                //from type in asm.GetTypes()
                from asm in assemblies
                from type in asm.GetTypes()
                from i in type.GetInterfaces()
                where i == typeof (IWireProtocol)
                select type;
            foreach (var type in types) {
                // Clean the name, we want to call: GetWireProtocol("Binary")
                var cleanName = type.Name.Replace("SerializationProtocol", "").ToLower();
                WireProtocolMap.Add(cleanName, type);
            }


            // Fill our wireprotocol factory map
            WireProtocolFactoryMap = new Dictionary<string, Type>();

            // Get IWireProtocolFactory types from this assembly
            types =
                //from type in asm.GetTypes()
                from asm in assemblies
                from type in asm.GetTypes()
                from i in type.GetInterfaces()
                where i == typeof(IWireProtocolFactory)
                select type;
            foreach (var type in types) {
                // Clean the name, we want to call: GetWireProtocolFactory("Binary")
                var cleanName = type.Name.Replace("SerializationProtocolFactory", "").ToLower();
                WireProtocolFactoryMap.Add(cleanName, type);
            }
        }



        /// <summary>
        ///     Creates a default wire protocol factory object to be used on communicating of applications.
        /// </summary>
        /// <returns>A new instance of default wire protocol</returns>
        public static IWireProtocolFactory GetDefaultWireProtocolFactory() {
            return GetWireProtocolFactory("Binary");
        }

        /// <summary>
        ///     Creates a wire protocol object to be used on communicating of applications
        ///     based on the given name.
        /// </summary>
        /// <param name="name">Name of the wireprotocol</param>
        /// <returns>A new instance of the wire protocol</returns>
        public static IWireProtocolFactory GetWireProtocolFactory(string name) {
            var nameLower = name.ToLower();
            if (WireProtocolFactoryMap.ContainsKey(nameLower) == false) {
                throw new Exception("Named wireprotocol factory \"" + name + "\" not found.");
            }

            var type = WireProtocolFactoryMap[nameLower];
            return Activator.CreateInstance(type) as IWireProtocolFactory;
        }

        /// <summary>
        ///     Creates a default wire protocol object to be used on communicating of applications.
        /// </summary>
        /// <returns>A new instance of default wire protocol</returns>
        public static IWireProtocol GetDefaultWireProtocol() {
            return GetWireProtocol("Binary");
        }

        /// <summary>
        ///     Creates a wire protocol object to be used on communicating of applications
        ///     based on the given name.
        /// </summary>
        /// <param name="name">Name of the wireprotocol</param>
        /// <returns>A new instance of the wire protocol</returns>
        public static IWireProtocol GetWireProtocol(string name) {
            var nameLower = name.ToLower();
            if (WireProtocolMap.ContainsKey(nameLower) == false) {
                throw new Exception("Named wireprotocol \"" + name + "\" not found.");
            }

            var type = WireProtocolMap[nameLower];
            return Activator.CreateInstance(type) as IWireProtocol;
        }

    }

}