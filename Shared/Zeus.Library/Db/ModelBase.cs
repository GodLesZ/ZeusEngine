using System.Configuration;
using System.Data.Common;

namespace Zeus.Library.Db {

    /// <summary>
    ///     Model base class for all Zeus model objects
    /// </summary>
    public abstract class ModelBase : DynamicModel {

        protected ModelBase(string connectionStringName, string tableName = "", string primaryKeyField = "", string descriptorField = "")
            : base(connectionStringName, tableName, primaryKeyField, descriptorField) {
        }

        protected ModelBase(ConnectionStringSettings settings, string providerName = "System.Data.SqlClient")
            : base(settings, providerName) {
        }

        protected ModelBase(ConnectionStringSettings settings, DbProviderFactory factory)
            : base(settings, factory) {
        }

    }

}