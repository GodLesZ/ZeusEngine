using System;

namespace Zeus.Server.Library.Database {

    /// <summary>
    /// Provides general infos about a data-driven entity like name of the data table.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ProviderEntryAttribute : Attribute {

        public string TableName {
            get;
            protected set;
        }


        public ProviderEntryAttribute(string tableName) {
            TableName = tableName;
        }
         
    }

}