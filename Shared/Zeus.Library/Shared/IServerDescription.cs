using Zeus.CommunicationFramework.EndPoints;

namespace Zeus.Library.Shared {

    /// <summary>
    ///     Describes a server which is chooseable from a client.
    ///     The later choosen WorldServer have to match this.
    ///     @TODO: This was the old Ragnarok way. Maybe we should think about realms or w/e.
    /// </summary>
    public interface IServerDescription {

        string Name { get; }
        
    }

}