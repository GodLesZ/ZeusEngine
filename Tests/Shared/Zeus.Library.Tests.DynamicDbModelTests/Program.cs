using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zeus.Library.Db;

namespace Zeus.Library.Tests.DynamicDbModelTests {

    public class Program {

        public static void Main(string[] args) {
            dynamic table = new DynamicModel("zeus_localhost", "account");
            var searchResult = table.Find(columns: "id, login AS LoginProp");

            var i = 0;
            Console.WriteLine("== Dump {0} results", searchResult.Count);
            foreach (var account in searchResult) {
                Console.WriteLine("{0}: {1} (id #{2})", i++, account.LoginProp, account.id);
            }


            Console.WriteLine();
            
            var newAcc = table.Insert(new { login = string.Format("dynamic_created_account{0}", i) });
            Console.WriteLine("New account: {0} (id #{1})", newAcc.login, newAcc.id);

            Console.Read();
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
