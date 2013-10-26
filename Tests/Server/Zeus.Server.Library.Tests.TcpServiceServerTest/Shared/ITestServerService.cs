using Zeus.CommunicationFramework.Services.Service;

namespace Zeus.Server.Library.Tests.TcpServiceServerTest.Shared {

    [Service(Version = "1.0.0.0")]
    public interface ITestServerService {

        void SomeRemoteMagic(SimplePoco poco);
        void RaiseTheException();

    }

}