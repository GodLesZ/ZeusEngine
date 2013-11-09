using System;

namespace Zeus.Server.Library.Database {

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class ProviderPropertyAttribute : Attribute {

        public string Name {
            get;
            protected set;
        }


        public ProviderPropertyAttribute(string name) {
            Name = name;
        }


    }

}