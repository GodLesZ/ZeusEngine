using System;

namespace Zeus.Server.Library.Tests.TcpServiceServerTest.Shared {

    /// <summary>
    ///     A serializable POCO-style object for sending between the service-channel pipe test
    /// </summary>
    [Serializable]
    public class SimplePoco {
        
        public int Id { get; set; }

        public string Title { get; set; }

    }

}