using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Ninject;
using Ninject.Modules;

namespace Configoo
{
    public class Configooness : NinjectModule
    {
        public override void Load()
        {
            Bind<Configured>().ToSelf().InSingletonScope();

            Bind<GetConfigurationValues>()
                .ToConstant(() =>
                {
                    var values = new Dictionary<string, object>();
                    AppSettings.ForEach(x => values.Add(x.Key, x.Value));
                    ConnectionStrings.ForEach(x => values.Add(x.Key, x.Value));
                    return values;
                }).InSingletonScope();

            Configured.Instance = () => Kernel.Get<Configured>();
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