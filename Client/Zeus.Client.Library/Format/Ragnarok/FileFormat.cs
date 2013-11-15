using System.IO;
using System.Text;

namespace Zeus.Client.Library.Format.Ragnarok {

    public abstract class FileFormat : FileFormatBase {

        public FileFormatHeader FileHeader {
            get;
            protected set;
        }

        public virtual char[] ExpectedMagicBytes {
            get { return new[] { 'G', 'o', 'd', 'L', 'e', 's', 'Z' }; }
        }

        protected FileFormat() {
        }

        protected FileFormat(Encoding enc)
            : base(enc) {
        }

        protected FileFormat(string filepath)
            : base(filepath) {
        }

        protected FileFormat(string filepath, Encoding enc)
            : base(filepath, enc) {
        }

        protected FileFormat(byte[] data)
            : base(data) {
        }

        protected FileFormat(Stream stream)
            : base(stream) {
        }

        protected FileFormat(Stream stream, Encoding enc)
            : base(stream, enc) {
        }


        protected override bool ReadInternal() {
            if (base.ReadInternal() == false) {
                return false;
            }

            FileHeader = new FileFormatHeader(Reader, ExpectedMagicBytes);
            return FileHeader.IsValid();
        }

    }

}