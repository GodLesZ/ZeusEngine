using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using YamlDotNet.RepresentationModel;
using Zeus.Library.Extensions;

namespace Zeus.Library.Configuration.Yaml {

    /// <summary>
    ///     Wraps a <see cref="YamlStream" />
    /// </summary>
    public class YamlConfiguration : IConfigurationProvider {

        protected YamlStream _yamlStream;

        /// <summary>
        ///     Gets a list of all <see cref="YamlDocument" />'s in the loaded configuration file.
        /// </summary>
        public IEnumerable<YamlDocument> Documents {
            get {
                EnsureActiveYamlStream();
                return _yamlStream.Documents;
            }
        }


        /// <summary>
        ///     Creates a new instance without an attached configuration file.
        /// </summary>
        public YamlConfiguration() {

        }

        /// <summary>
        ///     Creates a new instances an attaches the given <see cref="YamlDocument" /> to the underlying stream.
        /// </summary>
        /// <param name="doc"></param>
        public YamlConfiguration(YamlDocument doc) {
            EnsureActiveYamlStream();
            _yamlStream.Documents.Add(doc);
        }

        /// <summary>
        ///     Creates a new instance and tries to load from the given <paramref name="filepath" />.
        /// </summary>
        /// <param name="filepath"></param>
        public YamlConfiguration(string filepath)
            : this() {
            Load(filepath);
        }

        /// <summary>
        ///     Creates a new instance and tries to load from the given <paramref name="inputStream" />.
        /// </summary>
        /// <param name="inputStream"></param>
        public YamlConfiguration(TextReader inputStream)
            : this() {
            Load(inputStream);
        }


        /// <summary>
        ///     Tries to load a yaml configuration from the given <paramref name="filepath" />.
        /// </summary>
        /// <param name="filepath"></param>
        public void Load(string filepath) {
            if (string.IsNullOrEmpty(filepath)) {
                throw new ArgumentNullException("filepath");
            }
            if (File.Exists(filepath) == false) {
                throw new FileNotFoundException("Yaml-configuration file not found", filepath);
            }

            using (var input = new StreamReader(filepath)) {
                Load(input);
            }
        }

        /// <summary>
        ///     Tries to load a yaml configuration from the given <paramref name="inputStream" />.
        /// </summary>
        /// <param name="inputStream"></param>
        public void Load(TextReader inputStream) {
            if (inputStream == null) {
                throw new ArgumentNullException("inputStream");
            }

            _yamlStream = new YamlStream();
            _yamlStream.Load(inputStream);
        }


        /// <summary>
        ///     Returns all <see cref="YamlDocument" /> in the current loaded configuration as a dynamic
        ///     <see cref="ExpandoObject" />.
        /// </summary>
        /// <returns></returns>
        public dynamic AsExpando() {
            if (!Documents.Any()) {
                throw new NoYamlConfigurationLoaded();
            }
            
            return
                Documents
                    .Select(doc => doc.RootNode as YamlMappingNode)
                    .Select(yamlMap => yamlMap.Children.ToExpando());
        }

        /// <summary>
        ///     Returns the first <see cref="YamlDocument" /> in the current loaded configuration as a dynamic
        ///     <see cref="ExpandoObject" />.
        /// </summary>
        /// <returns></returns>
        public dynamic FirstAsExpando() {
            if (!Documents.Any()) {
                throw new NoYamlConfigurationLoaded();
            }

            var yamlMap = Documents.Select(doc => doc.RootNode as YamlMappingNode).First();
            return yamlMap.Children.ToExpando();
        }


        /// <summary>
        ///     Ensures an active <see cref="YamlStream" />, even if no configuration file loaded yet.
        /// </summary>
        protected void EnsureActiveYamlStream() {
            if (_yamlStream != null) {
                return;
            }

            _yamlStream = new YamlStream();
        }

    }

}