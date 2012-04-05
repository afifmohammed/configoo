using System.Collections.Generic;

namespace Configoo
{
    public class LookupValues : ILookupValues
    {
        private readonly IDictionary<string, object> _valueDictionary;

        public LookupValues() : this(new Dictionary<string, object>())
        {}

        public LookupValues(IDictionary<string, object> valueDictionary)
        {
            _valueDictionary = valueDictionary;
        }
        
        public IEnumerable<string> Keys { get { return _valueDictionary.Keys; } }

        protected virtual bool IsNotNull<TValue>(TValue value)
        {
            return !ReferenceEquals(value, null) && !value.Equals(default(TValue));
        }

        public TValue Get<TValue>(string key, TValue @default)
        {
            key = key.Trim();
            object value;

            if (!_valueDictionary.TryGetValue(key, out value))
            {
                if (IsNotNull(@default))
                    _valueDictionary.Add(key, @default);

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