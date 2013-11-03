using System;

namespace Zeus.Library.Shared {
    
    /// <summary>
    ///     Simple container for shared account informations between client and other servers.
    /// </summary>
    [Serializable]
    public class AccountInfo : IAccountInfo {

        public long Id { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }

        public AccountInfo(long id, string username, string password) {
            Password = password;
            Username = username;
            Id = id;
        }


        /// <summary>
        /// Gibt eine Zeichenfolge zurück, die das aktuelle Objekt darstellt.
        /// </summary>
        /// <returns>
        /// Eine Zeichenfolge, die das aktuelle Objekt darstellt.
        /// </returns>
        public override string ToString() {
            return string.Format("[{0}] {1} @ {2}", Id, Username, Password);
        }

    }

}