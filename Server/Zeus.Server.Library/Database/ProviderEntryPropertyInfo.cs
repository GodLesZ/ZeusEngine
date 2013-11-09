using System.Reflection;

namespace Zeus.Server.Library.Database {

    public class ProviderEntryPropertyInfo {
        
        public PropertyInfo ClassPropertyInfo {
            get;
            protected set;
        }

        public ProviderPropertyAttribute DataInfo {
            get;
            protected set;
        }


        public ProviderEntryPropertyInfo(PropertyInfo propertyInfo, ProviderPropertyAttribute dataInfo) {
            ClassPropertyInfo = propertyInfo;
            DataInfo = dataInfo;
        }

    }

}