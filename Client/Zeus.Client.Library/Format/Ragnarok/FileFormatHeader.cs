using System.IO;
using System.Linq;

namespace Zeus.Client.Library.Format.Ragnarok {

    public class FileFormatHeader {
        protected char[] _expectedMagicBytes;

        public FileFormatVersion Version {
            get;
            protected set;
        }

        public char[] MagicBytes {
            get;
            protected set;
        }


        public FileFormatHeader(BinaryReader reader, char[] expectedMagicBytes = null) {
            _expectedMagicBytes = expectedMagicBytes;
            if (expectedMagicBytes != null) {
                MagicBytes = reader.ReadChars(expectedMagicBytes.Length);
            }
            Version = new FileFormatVersion(reader);
        }


        public bool IsValid() {
            if (_expectedMagicBytes == null) {
                return true;
            }
            if (MagicBytes == null) {
                return false;
            }
            if (_expectedMagicBytes.Length != MagicBytes.Length) {
                return false;
            }

            return _expectedMagicBytes.Where((t, i) => t != MagicBytes[i]).Any() == false;
        }
         
    }

}