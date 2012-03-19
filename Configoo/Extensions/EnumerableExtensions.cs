using System;
using System.Collections.Generic;

namespace Configoo
{
    internal static class EnumerableExtensions
    {
        /// <summary>
        /// enumerates each item on the <paramref name="items"/> collection and will apply the <paramref name="action"/> on it.
        /// </summary>
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }
    }
}