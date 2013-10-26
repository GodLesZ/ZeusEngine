using System;
using System.IO;
using System.Text;

namespace Zeus.Library.IO {

    public class Tools {

        public static void EnsureDirectory(string dir) {
            // @TODO: Working with Mono on Linux?
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dir);

            if (Directory.Exists(path) == false) {
                Directory.CreateDirectory(path);
            }
        }


        public static void FormatBuffer(TextWriter output, Stream input, int length) {
            output.WriteLine("        0  1  2  3  4  5  6  7   8  9  A  B  C  D  E  F");
            output.WriteLine("       -- -- -- -- -- -- -- --  -- -- -- -- -- -- -- --");

            var byteIndex = 0;
            var whole = length >> 4;
            var rem = length & 0xF;

            for (var i = 0; i < whole; ++i, byteIndex += 16) {
                var bytes = new StringBuilder(49);
                var chars = new StringBuilder(16);

                for (var j = 0; j < 16; ++j) {
                    var c = input.ReadByte();
                    bytes.Append(c.ToString("X2"));

                    if (j != 7) {
                        bytes.Append(' ');
                    } else {
                        bytes.Append("  ");
                    }

                    if (c >= 32 && c < 128) {
                        chars.Append((char)c);
                    } else {
                        chars.Append('.');
                    }
                }

                output.Write(byteIndex.ToString("X4"));
                output.Write("   ");
                output.Write(bytes.ToString());
                output.Write("  ");
                output.WriteLine(chars.ToString());
            }

            if (rem == 0) {
                return;
            }

            var byteBuffer = new StringBuilder(49);
            var charBuffer = new StringBuilder(rem);

            for (var j = 0; j < 16; ++j) {
                if (j < rem) {
                    var c = input.ReadByte();
                    byteBuffer.Append(c.ToString("X2"));

                    if (j != 7) {
                        byteBuffer.Append(' ');
                    } else {
                        byteBuffer.Append("  ");
                    }

                    if (c >= 32 && c < 128) {
                        charBuffer.Append((char)c);
                    } else {
                        charBuffer.Append('.');
                    }
                } else {
                    byteBuffer.Append("   ");
                }
            }

            output.Write(byteIndex.ToString("X4"));
            output.Write("   ");
            output.Write(byteBuffer.ToString());
            output.Write("  ");
            output.WriteLine(charBuffer.ToString());
        }

    }

}