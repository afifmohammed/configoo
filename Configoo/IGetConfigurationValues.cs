using System.Collections.Generic;

namespace Configoo
{
    public interface IGetConfigurationValues
    {
        IEnumerable<string> Keys { get; }
        TValue Get<TValue>(string key, TValue @default);
    }
}