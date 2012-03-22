using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Configoo
{

    public class LookupValues : ILookupValues
    {
        #region privates
        private IDictionary<string, object> _valueDictionary;
        protected virtual IDictionary<string, object> ValueDictionary
        {
            get
            {
                if (_valueDictionary != null) return _valueDictionary;

                _valueDictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                
                AppSettings.ForEach(x => _valueDictionary.Add(x.Key, x.Value));
                ConnectionStrings.ForEach(x => _valueDictionary.Add(x.Key, x.Value));
                
                return _valueDictionary;
            }
        }

        private static IEnumerable<KeyValuePair<string, object>> AppSettings
        {
            get
            {
                var collection = ConfigurationManager.AppSettings;
                return collection.Cast<string>().Select(x => new KeyValuePair<string, object>(x, collection[x]));
            }
        }

        private static IEnumerable<KeyValuePair<string, object>> ConnectionStrings
        {
            get
            {
                var connectionStrings = ConfigurationManager.ConnectionStrings ?? new ConnectionStringSettingsCollection();

                return connectionStrings.Cast<ConnectionStringSettings>().ToDictionary<ConnectionStringSettings, string, object>(
                            keySelector: connectionString => connectionString.Name.Trim(), 
                            elementSelector: connectionString => connectionString);
            }
        }
        #endregion

        public IEnumerable<string> Keys { get { return ValueDictionary.Keys; } }

        public TValue Get<TValue>(string key, TValue @default)
        {
            key = key.Trim();
            object value;

            if(!ValueDictionary.TryGetValue(key, out value))
            {
                if (!ReferenceEquals(@default, null) && !@default.Equals(default(TValue)))
                    ValueDictionary.Add(key, @default);

                value = @default;
            }

            if (value == null) 
                throw new KeyNotFoundException(string.Format("the key '{0}' was not found to be Configured", key));

            if (value is string && typeof(TValue) != typeof(string))
            {
                TValue output;
                return ((value as string).TryParse(out output))
                           ? output
                           : default(TValue);
            }

            return (TValue)value;
        }
    }
}