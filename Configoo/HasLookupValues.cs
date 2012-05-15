using System;
using System.Collections.Generic;

namespace Configoo
{
    public abstract class HasLookupValues : IHaveLookupValues
    {
        private readonly Lazy<IDictionary<string, object>> _valueDictionary;

        protected HasLookupValues()
            : this(new Lazy<IDictionary<string, object>>(() => new Dictionary<string, object>()))
        { }

        protected HasLookupValues(Lazy<IDictionary<string, object>> valueDictionary)
        {
            _valueDictionary = valueDictionary;
        }

        public IEnumerable<string> Keys { get { return _valueDictionary.Value.Keys; } }

        protected virtual bool IsNotNull<TValue>(TValue value)
        {
            return !ReferenceEquals(value, null) && !value.Equals(default(TValue));
        }

        public virtual TValue Get<TValue>(string key, TValue @default)
        {
            key = key.Trim();
            object value;

            if (!_valueDictionary.Value.TryGetValue(key, out value))
            {
                if (IsNotNull(@default))
                    _valueDictionary.Value.Add(key, @default);

                value = @default;
            }

            if (value == null)
                throw new KeyNotFoundException(string.Format("the key '{0}' was not found to be SyntaticSugar", key));

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