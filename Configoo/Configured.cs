using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Configoo
{
    public static class A<TConfigured> where TConfigured : Configured
    {
        public static TConfigured Value { 
            get 
            { 
                var resolver = Resolver.Get;
                if(resolver == null)
                    throw new InvalidOperationException(
                        string.Format("{0} could not be resolved. Did you forget to Load Configooness in your Ninject Kenrnel.", typeof(TConfigured)));

                return resolver(typeof (TConfigured)) as TConfigured;
            } 
        }
    }

    public class Configured
    {
        private readonly IGetConfigurationValues _values;

        private Configured() {}

        protected Configured(IGetConfigurationValues values)
        {
            _values = values;
        }

        public string For(string key, string @default = null)
        {
            return For<string>(key, @default);
        }

        public TProperty For<TClass, TProperty>(Expression<Func<TClass, TProperty>> propertyAccessor, TProperty @default = default(TProperty))
        {
            var key = propertyAccessor.Name();
            return For(key, @default);
        }

        public TValue For<TValue>(TValue @default = default(TValue)) where TValue : class
        {
            return typeof (TValue) == typeof (string)
                       ? For(@default as string, default(TValue))
                       : For(typeof (TValue).Name, @default);

        }

        public TValue For<TValue>(Func<string, bool> keySelector)
        {
            var key = _values.Keys.SingleOrDefault(keySelector);
            return string.IsNullOrEmpty(key) ? default(TValue) : For<TValue>(key);
        }

        public TValue For<TValue>(string key, TValue @default = default(TValue))
        {
            var thedefault = default(TValue);
            if (!_values.Keys.Contains(key) && (@default == null || @default.Equals(thedefault))) 
                throw new KeyNotFoundException(string.Format("the key '{0}' was not found to be Configured", key));

            return _values.Get(key, @default);
        }
    }
}
