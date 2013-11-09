using XmlConfiguration = Zeus.Library.Configuration.Xml.Provider;

namespace Zeus.Server.Library {

    public static class Globals {

        public static string DatabaseConnectionString {
            get;
            private set;
        }

        
        public static void Initialize(dynamic databaseConfiguration) {
            DatabaseConnectionString = string.Format("SERVER={0};PORT={1};UID={2};PASSWORD={3};DATABASE={4};", databaseConfiguration.host, databaseConfiguration.port, databaseConfiguration.username, databaseConfiguration.password, databaseConfiguration.database);
        }
         
    }

}