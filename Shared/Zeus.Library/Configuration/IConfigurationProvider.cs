using System.IO;

namespace Zeus.Library.Configuration {

    public interface IConfigurationProvider {

        void Load(string filepath);
        void Load(TextReader inputStream);

        dynamic AsExpando();
        dynamic FirstAsExpando();

    }

}