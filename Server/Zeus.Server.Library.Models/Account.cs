using System.Collections.Generic;
using Zeus.Library.Db;
using Zeus.Library.Models;

namespace Zeus.Server.Models {

    /// <summary>
    ///     Deprecated
    /// </summary>
    public class Acccount : ModelBase, IAccount {

        protected const string DbAlias = "zeus_localhost";
        protected static DynamicModel _accountCollection;

        public static DynamicModel Collection {
            get { return _accountCollection ?? (_accountCollection = new Acccount()); }
        }

        public static IEnumerable<dynamic> All {
            get { return Collection.All(); }
        }


        public int Id { get; protected set; }

        public string Login { get; protected set; }

        public string Password { get; protected set; }


        public Acccount(string connectionStringName = DbAlias)
            : base(connectionStringName, "accounts", "id", "login") {
        }

    }

}