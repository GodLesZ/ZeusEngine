using System;
using MysqlProvider = Zeus.Server.Library.Database.Mysql.Provider;

namespace Zeus.Server.Library.Database {

    public static class Factory {

        private static IProvider CreateDefaultProvider() {
            return new MysqlProvider(Globals.DatabaseConnectionString);
        }

        public static T CreateProvider<T>() where T : IProvider {
            return (T)CreateDefaultProvider();
        }

        public static T CreateProvider<T>(string host, int port, string username, string password) where T : IProvider {
            // @TODO: This is common style for dynamic factories, any way to abstract it into a generic FactoryBase?
            var ctor = typeof(T).GetConstructor(new[] {
                typeof(string)
            });
            if (ctor == null) {
                // Search for same signature as our params
                ctor = typeof(T).GetConstructor(new [] {
                    typeof(string),
                    typeof(int),
                    typeof(string),
                    typeof(string)
                });
                if (ctor == null) {
                    throw new Exception("No constructor found in \"" + typeof(T) + "\"");
                }
            }

            return (T)ctor.Invoke(new object[] {
                host,
                port,
                username,
                password
            });
        }
         
    }

}