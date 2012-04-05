using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Configoo
{
    internal static class DictionaryExtensions
    {
        public static void PopulateConfigurationValues(this IDictionary<string, object> dictionary)
        {
            var collection = ConfigurationManager.AppSettings;

            collection.Cast<string>()
                .Select(x => new KeyValuePair<string, object>(x, collection[x]))
                .ForEach(x => dictionary.Add(x.Key, x.Value));

            var connectionStrings = ConfigurationManager.ConnectionStrings ?? new ConnectionStringSettingsCollection();

            connectionStrings.Cast<ConnectionStringSettings>().ToDictionary<ConnectionStringSettings, string, object>(
                keySelector: connectionString => connectionString.Name.Trim(),
                elementSelector: connectionString => connectionString)
                .ForEach(x => dictionary.Add(x.Key, x.Value));
        }
    }
}