namespace Zeus.Library.Shared {

    /// <summary>
    ///     Simple interface for shared account informations between client and other servers.
    /// </summary>
    public interface IAccountInfo {

        long Id { get; }
        string Username { get; }
        string Password { get; }

    }

}