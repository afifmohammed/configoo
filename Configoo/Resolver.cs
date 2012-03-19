using System;

namespace Configoo
{
    internal static class Resolver
    {
        public static Func<Type, object> Get { get; set; } 
    }
}