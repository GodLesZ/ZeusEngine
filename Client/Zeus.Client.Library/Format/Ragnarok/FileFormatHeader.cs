using System.IO;

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
            return _expectedMagicBytes == null || _expectedMagicBytes == MagicBytes;
        }
         
    }

}