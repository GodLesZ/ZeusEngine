using System.IO;
using System.Xml.Linq;
using Zeus.Library;
using Zeus.Library.Configuration;

namespace Zeus.Library.Configuration.Xml {

    public class XmlConfiguration : IConfigurationProvider {

        protected XElement _root;


        /// <summary>
        ///     Creates a new instance without an attached configuration file.
        /// </summary>
        public XmlConfiguration() {

        }

        /// <summary>
        ///     Creates a new instance and tries to load from the given <paramref name="filepath" />.
        /// </summary>
        /// <param name="filepath"></param>
        public XmlConfiguration(string filepath)
            : this() {
            Load(filepath);
        }


        public void Load(string filepath) {
            _root = XDocument.Load(filepath).Root;
        }

        public void Load(TextReader inputStream) {
            _root = XDocument.Load(inputStream).Root;
        }

        public dynamic AsExpando() {
            return DynamicXml.Parse(_root);
        }

        public dynamic FirstAsExpando() {
            return AsExpando();
        }

    }

}