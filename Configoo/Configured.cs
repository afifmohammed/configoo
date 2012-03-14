using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Ninject;

namespace Configoo
{
    public class Configured
    {
        internal static Configured _value;
        internal static Func<Configured> Instance { get; set; }
        
        public static Configured Value
        {
            get { return _value ?? (_value = Instance()); }
        }

        private readonly Lazy<IDictionary<string, object>> _values;

        [Inject] private IGetConfigurationValues Values { get; set; }

        public Configured()
        {
            _values = new Lazy<IDictionary<string, object>>(() => Values.List);
        }

        static Configured()
        {
            Instance = () => { throw new InvalidOperationException("Oops! You need to load Configooness in your ninject kernel."); };
        }

        public string For(string key, string @default = null)
        {
            return GetValue(key, @default).ToString();
        }

        public TValue For<TValue>(string key, TValue @default = default(TValue))
        {
            return (TValue)GetValue(key, @default);
        }

        public object For<TClass>(Expression<Func<TClass, object>> propertyAccessor, object @default = null)
        {
            var key = propertyAccessor.Name();
            return GetValue(key, @default);
        }

        public TValue For<TValue>(Func<string, bool> keySelector)
        {
            var key = _values.Value.Keys.SingleOrDefault(keySelector);
            return string.IsNullOrEmpty(key) ? default(TValue) : (TValue)GetValue(key);
        }

        private object GetValue(string key, object @default = null)
        {
            var values = _values.Value;
            
            key = key.Trim().ToLower();

            if (!values.ContainsKey(key) && @default != null)
                values.Add(key, @default);

            return values[key];
        }
    }
}
