using System;
using System.IO;
using System.IO.Compression;

namespace Zeus.Client.Library.Format.Compression {

	public class Deflate {
		public static byte[] MagicZlibHead1 = new byte[2] { 0x78, 0x9C };
		public static byte[] MagicZlibHead2 = new byte[2] { 0x78, 0x5E }; // TODO: This seems to be not offical?
		public static byte[] MagicZlibHead3 = new byte[2] { 0x78, 0xDA };
		public static byte[] MagicZlibHead4 = new byte[2] { 0x78, 0x01 };

		public static bool IsMagicHead(byte[] buf) {
			bool isMagicHead =
				(buf[0] == MagicZlibHead1[0] && buf[1] == MagicZlibHead1[1]) ||
				(buf[0] == MagicZlibHead2[0] && buf[1] == MagicZlibHead2[1]) ||
				(buf[0] == MagicZlibHead3[0] && buf[1] == MagicZlibHead3[1]) ||
				(buf[0] == MagicZlibHead4[0] && buf[1] == MagicZlibHead4[1]);
			return isMagicHead;
		}

		public static byte[] Decompress(byte[] input) {
			if (input.Length < 3) {
				return input;
			}

			bool stripBytes = IsMagicHead(input);
			return Decompress(input, stripBytes);
		}

		public static byte[] Decompress(byte[] input, bool stripBytes) {
			byte[] output = new byte[0];
			using (MemoryStream toDeflate = new MemoryStream(input)) {
				output = Decompress(toDeflate, stripBytes);
			}
			return output;
		}

		public static byte[] Decompress(Stream input, bool stripBytes) {
			if (stripBytes == true) {
				byte b1 = (byte)input.ReadByte();
				byte b2 = (byte)input.ReadByte();
			}

			MemoryStream output = new MemoryStream();
			using (DeflateStream Deflate = new DeflateStream(input, CompressionMode.Decompress)) {
				byte[] buffer = new byte[input.Length];
				int count = 0;

				try {
					while ((count = Deflate.Read(buffer, 0, buffer.Length)) > 0) {
						output.Write(buffer, 0, count);
					}
				} catch (Exception e) {
					System.Diagnostics.Debug.WriteLine(e);
				}
			}

			return output.ToArray();
		}

		public static byte[] Compress(byte[] input, bool addFirst2Bytes) {
			if (input.Length == 0) {
				return input;
			}


			MemoryStream output = new MemoryStream();
			if (addFirst2Bytes) {
				output.Write(new byte[] { MagicZlibHead1[0], MagicZlibHead1[1] }, 0, 2);
			}

			using (DeflateStream Deflate = new DeflateStream(output, CompressionMode.Compress)) {
				try {
					Deflate.Write(input, 0, input.Length);
				} catch (Exception e) {
					System.Diagnostics.Debug.WriteLine(e);
				}
			}

			return output.ToArray();
		}






		public static ulong GetDecompressedLength(byte[] input) {
			byte[] decompressedBuf = Decompress(input);
			ulong len = (ulong)decompressedBuf.Length;
			decompressedBuf = null;

			return len;
		}

		public static ulong GetCompressedLength(byte[] input) {
			byte[] compressedBuf = Compress(input, true);
			ulong len = (ulong)compressedBuf.Length;
			compressedBuf = null;

			return len;
		}

	}

}
