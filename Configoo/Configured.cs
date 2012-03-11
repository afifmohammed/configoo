using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Ninject;

namespace Configoo
{
    public class Configured
    {
        private static Configured _value;
        internal static Func<Configured> Instance { get; set; }
        
        public static Configured Value
        {
            get { return _value ?? (_value = Instance()); }
        }

        private readonly Lazy<IDictionary<string, object>> _values;

        [Inject] private GetConfigurationValues ConfigurationValues { get; set; }

        public Configured()
        {
            _values = new Lazy<IDictionary<string, object>>(() => ConfigurationValues());
        }

        public string For(string key, string @default = null)
        {
            return GetValue(key, @default).ToString();
        }

        public TValue For<TValue>(string key, TValue @default = default(TValue)) where TValue : class
        {
            return GetValue(key, @default) as TValue;
        }

        public object For<TClass>(Expression<Func<TClass, object>> propertyAccessor, object @default = null)
        {
            var key = propertyAccessor.Name();
            return GetValue(key, @default);
        }

        private object GetValue(string key, object @default)
        {
            var values = _values.Value;
            if (!values.ContainsKey(key) && @default != null)
                values.Add(key, @default);

            return values[key];
        }
    }
}
