using System;
using System.Collections.Generic;
using Ninject;
using Ninject.Extensions.Conventions;
using Ninject.Modules;

namespace Configoo
{
    public class Configooness : NinjectModule
    {
        private readonly Action<AssemblyScanner>[] _customActions;

        public Configooness() : this(s => { })
        {}

        internal Configooness(params Action<AssemblyScanner>[] customActions)
        {
            _customActions = customActions;
        }

        public override void Load()
        {
            Resolver.Get = t => Kernel.Get(t);

            Bind<IDictionary<string, object>>().ToMethod(c =>
                                                       {
                                                           var d = new Dictionary<string, object>();
                                                           d.PopulateConfigurationValues();
                                                           return d;
                                                       }).WhenInjectedInto<LookupValues>().InSingletonScope();
            Kernel.Scan(scanner =>
                            {
                                var path = AppDomain.CurrentDomain.ExecutingAssmeblyPath();
                                scanner.FromAssembliesInPath(path); 
                                scanner.WhereTypeInheritsFrom<ILookupValues>();
                                scanner.Where(t => !t.IsAbstract && !t.IsInterface && t.IsClass);
                                scanner.BindWith<OverridableBindingGenerator<ILookupValues>>();
                                scanner.InSingletonScope();
                                _customActions.ForEach(a => a(scanner));
                            });

            Kernel.Scan(scanner =>
            {
                var path = AppDomain.CurrentDomain.ExecutingAssmeblyPath();
                scanner.FromAssembliesInPath(path);
                scanner.WhereTypeInheritsFrom<Configured>();
                scanner.Where(t => !t.IsAbstract && !t.IsInterface && t.IsClass);
                scanner.BindWithDefaultConventions();
                scanner.InSingletonScope();
                _customActions.ForEach(a => a(scanner));
            });

            Kernel.Settings.InjectNonPublic = true;
        }
    }
}