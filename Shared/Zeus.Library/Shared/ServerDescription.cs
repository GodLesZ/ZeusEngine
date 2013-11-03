
using System;

namespace Zeus.Library.Shared {

    /// <summary>
    ///     Describes a server which is chooseable from a client.
    ///     The later choosen WorldServer have to match this.
    ///     @TODO: This was the old Ragnarok way. Maybe we should think about realms or w/e.
    /// </summary>
    [Serializable]
    public class ServerDescription : IServerDescription {

        public string Name { get; private set; }
        
        public ServerDescription(string name) {
            Name = name;
        }


        /// <summary>
        /// Gibt eine Zeichenfolge zurück, die das aktuelle Objekt darstellt.
        /// </summary>
        /// <returns>
        /// Eine Zeichenfolge, die das aktuelle Objekt darstellt.
        /// </returns>
        public override string ToString() {
            return string.Format("{0}", Name);
        }

    }

}