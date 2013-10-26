using System;
using System.IO;
using System.Text;

namespace Zeus.Client.Library.Format {

    public abstract class FileFormatBase : IFileFormat, IDisposable {

        public Encoding Encoding { get; internal set; }

        public string Filepath { get; internal set; }

        public BinaryReader Reader { get; internal set; }

        public FileFormatVersion Version { get; internal set; }

        public BinaryWriter Writer { get; internal set; }

        public event OnFlushHandler OnFlush;


        protected FileFormatBase()
            : this("") {
        }

        protected FileFormatBase(Encoding enc)
            : this("", enc) {
        }

        protected FileFormatBase(string filepath)
            : this(filepath, null) {
        }

        protected FileFormatBase(string filepath, Encoding enc) {
            if (enc != null) {
                Encoding = enc;
            }

            if (string.IsNullOrEmpty(filepath)) {
                return;
            }

            if (File.Exists(filepath) == false) {
                throw new ArgumentException("Die Datei \"" + filepath + "\" konnte nicht gefunden werden.");
            }

            Read(filepath);
        }

        protected FileFormatBase(Stream stream)
            : this(stream, null) {
        }

        protected FileFormatBase(Stream stream, Encoding enc) {
            if (enc != null) {
                Encoding = enc;
            }

            if (stream == null) {
                return;
            }

            Read(stream);
        }

        public static void FreeSymbol(object obj) {
            if (obj == null) {
                return;
            }

            var disposable = obj as IDisposable;
            if (disposable != null) {
                disposable.Dispose();
            }

            obj = null;
        }

        public void Dispose() {
            Flush(true);
        }

        ~FileFormatBase() {
            Dispose();
        }

        #region Read
        public virtual bool Read() {
            return Reader != null && ReadInternal();

        }

        public virtual bool Read(Stream stream) {
            if (stream == null || stream.CanRead == false) {
                throw new Exception("Failed to read form stream!");
            }
            if (Encoding == null) {
                Encoding = Encoding.Default;
            }

            Filepath = null;
            Reader = new BinaryReader(stream, Encoding);

            return ReadInternal();
        }

        public virtual bool Read(string filepath) {
            if (File.Exists(filepath) == false) {
                throw new ArgumentException("File \"" + filepath + "\" not found!");
            }

            Filepath = filepath;
            Reader = new BinaryReader(File.OpenRead(filepath), Encoding);

            return ReadInternal();
        }


        protected virtual bool ReadInternal() {
            return false;
        }
        #endregion

        #region Write
        public virtual bool Write(string destinationPath, bool overwrite) {
            if (File.Exists(destinationPath)) {
                if (overwrite == false) {
                    return false;
                }
                File.Delete(destinationPath);
            }

            Writer = new BinaryWriter(File.OpenWrite(destinationPath), Encoding);
            return true;
        }

        public virtual bool Write(string destinationPath) {
            return Write(destinationPath, true);
        }
        #endregion

        #region Flush
        public virtual void Flush() {
            Flush(false);
        }

        public virtual void Flush(bool onDestruct) {
            if (OnFlush != null) {
                OnFlush(onDestruct);
            }

            if (Reader != null) {
                try {
                    Reader.Close();
                    Reader = null;
                } catch {
                }
            }

            if (Writer != null) {
                try {
                    Writer.Close();
                    Writer = null;
                } catch {
                }
            }
        }
        #endregion
    }

    public delegate void OnFlushHandler(bool onDestruct);

}