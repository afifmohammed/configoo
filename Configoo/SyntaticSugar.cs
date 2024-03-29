﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Configoo
{
    public interface IConfigurationSugar
    {
        /// <summary>
        /// Retreives the value as a <see cref="string"/> for the provided <param name="key"/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="default"></param>
        /// <returns></returns>
        string For(string key, string @default = null);

        TProperty For<TClass, TProperty>(Expression<Func<TClass, TProperty>> propertyAccessor, TProperty @default = default(TProperty));
        TValue For<TValue>(TValue @default = default(TValue)) where TValue : class;
        TValue For<TValue>(Func<string, bool> keySelector);
        TValue For<TValue>(string key, TValue @default = default(TValue));
    }

    /// <summary>
    /// helper class that provides strongly typed key resolution on top of the <see cref="IHaveLookupValues"/> interface
    /// </summary>
    internal sealed class SyntaticSugar : IConfigurationSugar
    {
        readonly IHaveLookupValues _values;

        private SyntaticSugar() { }

        internal SyntaticSugar(IHaveLookupValues values)
        {
            _values = values;
        }

        /// <summary>
        /// Retreives the value as a <see cref="string"/> for the provided <param name="key"/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="default"></param>
        /// <returns></returns>
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
                throw new KeyNotFoundException(string.Format("the key '{0}' was not found to be SyntaticSugar", key));

            return For<TValue>(key);
        }

        public TValue For<TValue>(string key, TValue @default = default(TValue))
        {
            return _values.Get(key, @default);
        }
    }
}