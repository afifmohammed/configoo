﻿namespace Configoo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public sealed class Configured
    {
        readonly IHaveLookupValues _values;

        private Configured() { }

        internal Configured(IHaveLookupValues values)
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
            return typeof(TValue) == typeof(string)
                       ? For(@default as string, default(TValue))
                       : For(typeof(TValue).Name, @default);

        }

        public TValue For<TValue>(Func<string, bool> keySelector)
        {
            var key = _values.Keys.SingleOrDefault(keySelector);

            if (string.IsNullOrEmpty(key))
                throw new KeyNotFoundException(string.Format("the key '{0}' was not found to be Configured", key));

            return For<TValue>(key);
        }

        public TValue For<TValue>(string key, TValue @default = default(TValue))
        {
            return _values.Get(key, @default);
        }
    }
}