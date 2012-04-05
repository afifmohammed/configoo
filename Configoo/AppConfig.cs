using System.Collections.Generic;

namespace Configoo
{
    public class AppConfig : LookupValues
    {
        private static IDictionary<string, object> Values
        {
            get { var d = new Dictionary<string, object>();
                d.PopulateConfigurationValues();
                return d;
            }
        }

        public AppConfig() : base(Values)
        {}
    }
}