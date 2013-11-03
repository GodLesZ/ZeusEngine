namespace Zeus.Library.Shared {

    /// <summary>
    ///     Simple interface for shared character informations between client and other servers.
    /// </summary>
    public interface ICharacterInfo {

        long Id { get; }
        string Name { get; }
         
    }

}