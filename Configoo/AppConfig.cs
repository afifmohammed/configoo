using System;
using System.Collections.Generic;

namespace Configoo
{
    /// <summary>
    /// helper class to retreive appconfig key value pairs and connection strings from app.config or web.config
    /// </summary>
    public sealed class AppConfig : HasLookupValues
    {
        /// <summary>
        /// on construction will get injected a lazy dictionary that is populated with appconfig key value pairs
        /// and connection strings.
        /// </summary>
        /// <remarks>should be used as a singleton</remarks>
        public AppConfig()
            : base(new Lazy<IDictionary<string, object>>(
                () => new Dictionary<string, object>().AppSettingsAndConnStrings()))
        { }
    }
}