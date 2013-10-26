using System;

namespace Zeus.Library.Configuration.Yaml {

    public class NoYamlConfigurationLoaded : Exception {

        public NoYamlConfigurationLoaded()
            : base("No yaml configuration file loaded yet") {
        }
        
    }

}