using System.IO;

namespace Zeus.Library.Configuration {

    public interface IProvider {

        void Load(string filepath);
        void Load(TextReader inputStream);

        dynamic AsExpando();
        dynamic FirstAsExpando();

    }

}