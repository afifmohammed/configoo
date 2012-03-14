using System;
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
            Bind<Configured>().ToSelf().InSingletonScope();
            
            Kernel.Scan(scanner =>
                            {
                                var path = AppDomain.CurrentDomain.ExecutingAssmeblyPath();
                                scanner.FromAssembliesInPath(path); 
                                scanner.WhereTypeInheritsFrom<IGetConfigurationValues>();
                                scanner.Where(t => !t.IsAbstract && !t.IsInterface && t.IsClass);
                                scanner.BindWith<OverridableBindingGenerator<IGetConfigurationValues>>();
                                scanner.InSingletonScope();
                                _customActions.ForEach(a => a(scanner));
                            });

            Configured.Instance = () => Kernel.Get<Configured>();
            Kernel.Settings.InjectNonPublic = true;
        }
    }
}