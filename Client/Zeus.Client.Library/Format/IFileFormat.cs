using System.IO;

namespace Zeus.Client.Library.Format {

    public interface IFileFormat {

        bool Read();
        bool Read(string filepath);
        bool Read(Stream stream);
        bool Write(string filepath, bool overwrite);
        void Flush();

    }

}