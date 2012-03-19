using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Configoo
{
    public class GetConfigurationValues : IGetConfigurationValues
    {
        #region privates
        private IDictionary<string, object> _valueDictionary;
        protected virtual IDictionary<string, object> ValueDictionary
        {
            get
            {
                if (_valueDictionary != null) return _valueDictionary;
                
                _valueDictionary = new Dictionary<string, object>();
                
                AppSettings.ForEach(x => _valueDictionary.Add(x.Key, x.Value));
                ConnectionStrings.ForEach(x => _valueDictionary.Add(x.Key, x.Value));
                
                return _valueDictionary;
            }
        }

        private static IEnumerable<KeyValuePair<string, object>> AppSettings
        {
            get
            {
                var valueCollection = new Dictionary<string, object>();
                var appSettingsKeyValuePairs = ConfigurationManager.AppSettings;
                appSettingsKeyValuePairs.AllKeys.ForEach(k => valueCollection.Add(k.Trim().ToLower(), appSettingsKeyValuePairs[k]));
                return valueCollection;
            }
        }

        private static IEnumerable<KeyValuePair<string, object>> ConnectionStrings
        {
            get
            {
                var connectionStrings = ConfigurationManager.ConnectionStrings ?? new ConnectionStringSettingsCollection();

                return connectionStrings.Cast<ConnectionStringSettings>()
                    .ToDictionary<ConnectionStringSettings, string, object>(connectionString => connectionString.Name.Trim().ToLower(), connectionString => connectionString);
            }
        }
        #endregion

        public IEnumerable<string> Keys { get { return ValueDictionary.Keys; } }

        public TValue Get<TValue>(string key, TValue @default)
        {
            var lowered = key.Trim().ToLower();
            if (!ValueDictionary.ContainsKey(lowered))
                ValueDictionary.Add(lowered, @default);

            return (TValue)ValueDictionary[lowered];
        }
    }
}