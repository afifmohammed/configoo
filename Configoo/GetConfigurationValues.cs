using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Configoo
{
    public interface IGetConfigurationValues
    {
        IDictionary<string, object> List { get; }
    }

    internal class GetConfigurationValues : IGetConfigurationValues
    {
        public IDictionary<string, object> List
        {
            get
            {
                var values = new Dictionary<string, object>();
                AppSettings.ForEach(x => values.Add(x.Key, x.Value));
                ConnectionStrings.ForEach(x => values.Add(x.Key, x.Value));
                return values;
            }
        }

        private static IEnumerable<KeyValuePair<string, object>> AppSettings
        {
            get
            {
                var valueCollection = new Dictionary<string, object>();
                var appSettingsKeyValuePairs = ConfigurationManager.AppSettings;
                appSettingsKeyValuePairs.AllKeys.ForEach(k => valueCollection.Add(k, appSettingsKeyValuePairs[k]));
                return valueCollection;
            }
        }

        private static IEnumerable<KeyValuePair<string, object>> ConnectionStrings
        {
            get
            {
                var connectionStrings = ConfigurationManager.ConnectionStrings ?? new ConnectionStringSettingsCollection();

                return connectionStrings.Cast<ConnectionStringSettings>()
                    .ToDictionary<ConnectionStringSettings, string, object>(connectionString => connectionString.Name, connectionString => connectionString);
            }
        }
    }
}