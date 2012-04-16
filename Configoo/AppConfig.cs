using System.Collections.Generic;

namespace Configoo
{
    public sealed class AppConfig : HaveLookupValues
    {
        public AppConfig()
            : base(new Dictionary<string, object>().AppSettingsAndConnStrings())
        { }
    }
}