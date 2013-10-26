namespace Zeus.Client.Tests.MonoGameClientTest.Formats {

	public class FileVersion {
		public byte MajorVersion;
		public byte MinorVersion;

		public FileVersion( byte v1, byte v2 ) {
			MajorVersion = v1;
			MinorVersion = v2;
		}

		public bool IsCompatible( byte Major, byte Minor ) {
			return ( MajorVersion > Major || ( MajorVersion == Major && MinorVersion >= Minor ) );
		}

	}

}
