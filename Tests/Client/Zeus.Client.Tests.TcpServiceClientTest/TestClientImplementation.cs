using System;
using Zeus.Server.Library.Tests.TcpServiceServerTest.Shared;

namespace Zeus.Client.Tests.TcpServiceClientTest {

    public class TestClientImplementation : ITestClient {

        public void SomeClientMagic(string msg) {
            Console.WriteLine("Some magic: {0}", msg);
        }

    }

}