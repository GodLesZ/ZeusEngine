using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Zeus.Library.Db;
using Zeus.Server.Library;

namespace Zeus.Library.Tests.DynamicDbModelTests {

    public class Program {

        public static void Main(string[] args) {
            var dbSserver = "localhost";
            var dbUser = "root";
            var dbPassword = "";
            var dbDatabase = "zeus_db";
            var connStr = String.Format("server={0};user id={1}; password={2}; database={3}; pooling=false", dbSserver, dbUser, dbPassword, dbDatabase);

            Globals.Initialize(connStr);

            try {
                var defaultProvider = Zeus.Server.Library.Database.Factory.CreateProvider<Zeus.Server.Library.Database.Mysql.Provider>();
                


                var conn = new MySqlConnection(connStr);
                conn.Open();

                MySqlDataReader reader = null;

                var cmd = new MySqlCommand("SHOW DATABASES", conn);
                try {
                    reader = cmd.ExecuteReader();
                    foreach (var row in (DbDataReader)reader) {
                        
                    }
                    while (reader.Read()) {
                        var database = reader.GetString(0);
                    }
                } catch (MySqlException ex) {
                    Console.WriteLine("Failed to populate database list: " + ex.Message);
                } finally {
                    if (reader != null) {
                        reader.Close();
                    }
                }
            } catch (MySqlException ex) {
                Console.WriteLine("Error connecting to the server: " + ex.Message);
            }

        }

    }


    // Zeus.Server.Library
    public class ModelBase {
        // Magic
    }

    // Zeus.Library.Model
    public partial class Account {
        public int Id { get; protected set; }
    }

    // Zeus.Server.Model
    public partial class Account : ModelBase {
        public object ServerOnlyProp { get; protected set; }


        public bool Login() {
            return false;
        }
    }

    public class Client {
        public void Main() {
            // Aus Zeus.Model
            var clientModel = new Account();
        }
    }
    public class AuthServer {
        public void Main() {
            // Aus Zeus.Server.Model
            var serverModel = new Account();
            serverModel.Login();
        }
    }


}
