using Ninject;
using Ninject.Modules;

namespace Configoo
{
    public class Configooness : NinjectModule
    {
        public override void Load()
        {
            Bind<Configured>().ToSelf().InSingletonScope();

            Bind<IGetConfigurationValues>().To<GetConfigurationValues>().InSingletonScope();

            Configured.Instance = () => Kernel.Get<Configured>();
            Kernel.Settings.InjectNonPublic = true;
        }
    }
}