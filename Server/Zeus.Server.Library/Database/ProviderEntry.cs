using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using Zeus.Server.Library.Database;

namespace Zeus.Server.Library.Database {

    /// <summary>
    /// Generic and abstract class to be used in database model entities.
    /// All properties should be lazy-loaded.
    /// Changes will be automatically saved upon disposing.
    /// 
    /// @TODO: How to provide connection info to each entity?
    /// @TODO: Create more global/static alike methods
    /// @TODO: Finish save method
    /// </summary>
    [ProviderEntryAttribute("table_test")]
    public class ProviderEntry : IDisposable, INotifyPropertyChanged, IProviderEntryCollection {

        // @TODO: We hate (un-)boxing, find another way to store the values as PropertyInfo.SetValue()
        protected static Dictionary<Type, List<ProviderEntryPropertyInfo>> _dynamicProperties = new Dictionary<Type, List<ProviderEntryPropertyInfo>>();
        protected static Dictionary<Type, ProviderEntryAttribute> _dynamicEntityInfo = new Dictionary<Type, ProviderEntryAttribute>();
        protected static IProviderEntryCollection _instance;

        // @TODO: Loadconnection settings from a global storage
        // @TODO: We need multiple provider, but not per entity instance; maybe also a cache per entity type?
        protected static IProvider _provider;

        protected bool _dataLoaded = false;
        protected bool _dataChanged = false;

        protected long _id = 0;


        public static IProviderEntryCollection All {
            get { return _instance ?? (_instance = new ProviderEntry()); }
        }

        [ProviderProperty("id")]
        public long Id {
            get { return _id; }
            protected set {
                _id = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        ~ProviderEntry() {
            Dispose();
        }


        /// <summary>
        /// Saves any changes to the underlying entity 
        /// </summary>
        public void Save() {
            // Anything to save?
            if (_dataChanged == false) {
                return;
            }

            // Mark as updated
            _dataChanged = false;


        }


        /// <summary>
        /// Retruns true if the given record id exists.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool RecordExists(long id) {
            var entityType = GetType();
            var entityInfo = _dynamicEntityInfo[entityType];
            var sql = string.Format("SELECT `id` FROM `{0}` WHERE `id` = {1}", entityInfo.TableName, id);
            var result = _provider.Query(sql);
            return result.Rows.Count == 1;
        }


        /// <summary>
        /// Saves any changes to the underlying entity and free's all resources
        /// </summary>
        public void Dispose() {
            // Save changes
            Save();
        }


        /// <summary>
        /// Stores all dynamic infos on the given entity in a static cache.
        /// </summary>
        /// <param name="type"></param>
        protected static void CacheEntityInfo(Type type) {

            // Cache general entity infos
            if (_dynamicEntityInfo.ContainsKey(type) == false) {
                var infoAttribute = type.GetCustomAttributes(typeof(ProviderEntryAttribute)).FirstOrDefault() as ProviderEntryAttribute;
                if (infoAttribute == null) {
                    throw new ArgumentException("Given type \"" + type + "\" needs a attribute of type DataDrivenEntryAttribute");
                }

                _dynamicEntityInfo.Add(type, infoAttribute);
            }

            // Cache list of dynamic properties (for save and load)
            if (_dynamicProperties.ContainsKey(type) == false) {

                // Get and store public properties
                var properties =
                    from m in type.GetProperties(BindingFlags.Public)
                    where m.IsDefined(typeof(ProviderPropertyAttribute))
                    select m;
                var propertyList = (
                    from propertyInfo in properties
                    let propertyAttribute = propertyInfo.GetCustomAttributes(typeof(ProviderPropertyAttribute)).FirstOrDefault() as ProviderPropertyAttribute
                    select new ProviderEntryPropertyInfo(propertyInfo, propertyAttribute)).ToList();

                // Append to static cache
                _dynamicProperties.Add(type, propertyList);
            }

        }


        protected void OnPropertyChanged(PropertyChangedEventArgs e) {
            var handler = PropertyChanged;
            if (handler != null) {
                handler(this, e);
            }
        }

        protected void OnPropertyChanged(string propertyName) {
            // Mark this object as dirty (something changed and needs to be saved)
            _dataChanged = true;
            // Trigger to listeners
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Loads all properties of thsi eneity from the database.
        /// </summary>
        protected void LazyLoad() {
            // Already loaded?
            if (_dataLoaded) {
                return;
            }

            // Mark as loaded
            _dataLoaded = true;

            // Get cached infos
            var entityType = GetType();
            var entityInfo = _dynamicEntityInfo[entityType];
            var entityProperties = _dynamicProperties[entityType];
            // Any property to read? (dynamic)
            if (entityProperties.Count == 0) {
                return;
            }

            // Unable to load something without primary key
            if (Id == 0) {
                return;
            }

            // Build query
            var sql = string.Format("SELECT * FROM `{0}` WHERE `id` = {1}", entityInfo.TableName, Id);
            // Fetch result
            var result = _provider.Query(sql);
            // Maybe entity does not exists?
            if (result.Rows.Count == 0) {
                // @TDOO: This means, we have an invalid/out-of-date record; maybe throw an exception?
                return;
            }

            var row = result.Rows[0];
            foreach (var prop in entityProperties) {
                var columnName = prop.DataInfo.Name;
                var propValue = row[columnName];
                prop.ClassPropertyInfo.SetValue(this, propValue);
            }
        }

    }

}