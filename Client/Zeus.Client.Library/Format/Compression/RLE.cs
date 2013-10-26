using System.Collections.Generic;

namespace Zeus.Client.Library.Format.Compression {

	public static class RLE {

		public static byte[] Decode(byte[] Data) {
			List<byte> returnBuf = new List<byte>();

			for (int Offset = 0; Offset < Data.Length; Offset++) {
				byte c = Data[Offset];
				// TODO: ++Offset could skip some chunks; if problems occur, test it here
				if (c == 0 && ++Offset < Data.Length) {
					byte len = Data[Offset];
					for (int j = 0; j < len; j++) {
						returnBuf.Add(0);
					}
				} else {
					returnBuf.Add(c);
				}
			}

			return returnBuf.ToArray();
		}

		public static byte[] Encode(byte[] Data) {
			List<byte> returnBuf = new List<byte>();

			for (int Offset = 0; Offset < Data.Length; Offset++) {
				byte c = Data[Offset];
				if (c != 0) {
					returnBuf.Add(c);
					continue;
				}

				byte len = 1;
				while (++Offset < Data.Length && Data[Offset] == 0) {
					len++;
				}
				returnBuf.Add(0);
				returnBuf.Add(len);
				Offset--;
			}

			return returnBuf.ToArray();
		}


	}

}
