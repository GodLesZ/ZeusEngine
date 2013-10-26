using System.IO;

namespace Zeus.Client.Library.Format.Compression {

	public static class Adler32 {
		public readonly static uint BASE = 65521;

		public static uint Checksum = 0;

		public static event UpdateStatusHandler OnUpdate;
		public static event FinishedHandler OnFinish;



		public static uint Build(byte[] buf) {
			int off = 0;
			int state = 0, newstate = 0;
			uint s1 = 1;
			uint s2 = 0;

			Checksum = 0;

			do {
				s1 += buf[off++];
				s2 += s1;
				newstate = (int)((off / buf.Length) * 100);
				if (newstate > state && OnUpdate != null) {
					state = newstate;
					OnUpdate(state);
				}
			} while (off < buf.Length);
			buf = null;

			s1 %= BASE;
			s2 %= BASE;


			Checksum = (s2 << 16) | s1;
			if (OnFinish != null) {
				OnFinish(Checksum);
			}

			return Checksum;
		}


		public static uint Build(string Filename) {
			int i = 0, off = 0;
			int state = 0, newstate = 0;
			uint s1 = 1;
			uint s2 = 0;
			long len = 0;

			Checksum = 0;

			using (FileStream fs = File.OpenRead(Filename)) {
				byte[] buf;
				long streamLength = fs.Length;
				len = streamLength;

				while (len > 0) {
					int tlen = (int)(len > 5550 ? 5550 : len);
					len -= tlen;
					off = 0;
					buf = new byte[tlen];
					fs.Read(buf, 0, buf.Length);
					do {
						s1 += buf[off++];
						s2 += s1;
						i++;
						newstate = (int)((i / streamLength) * 100);
						if (newstate > state && OnUpdate != null) {
							state = newstate;
							OnUpdate(state);
						}
					} while (--tlen != 0);
					buf = null;

					s1 %= BASE;
					s2 %= BASE;
				}

			}

			Checksum = (s2 << 16) | s1;
			if (OnFinish != null) {
				OnFinish(Checksum);
			}

			return Checksum;
		}

	}

	public delegate void UpdateStatusHandler(int State);
	public delegate void FinishedHandler(uint Checksum);

}
