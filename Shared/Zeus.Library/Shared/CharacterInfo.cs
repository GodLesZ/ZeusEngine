using System;

namespace Zeus.Library.Shared {

    /// <summary>
    ///     Simple interface for shared character informations between client and other servers.
    /// </summary>
    [Serializable]
    public class CharacterInfo : ICharacterInfo {

        public long Id { get; private set; }
        public string Name { get; private set; }

        public CharacterInfo(long id, string name) {
            Name = name;
            Id = id;
        }

    }

}