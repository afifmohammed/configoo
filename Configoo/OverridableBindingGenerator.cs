using System;
using System.Linq;
using Ninject;
using Ninject.Activation;
using Ninject.Extensions.Conventions;
using Ninject.Planning.Bindings;

namespace Configoo
{
    internal class OverridableBindingGenerator<TService> : IBindingGenerator
    {
        private readonly string _serviceAssembly = typeof(TService).Assembly.FullName;

        public void Process(Type type, Func<IContext, object> scopeCallback, IKernel kernel)
        {
            var service = typeof(TService);

            if (NoBindingsIn(kernel))
            {
                kernel.Rebind(service).To(type).InScope(scopeCallback).WithMetadata("assembly", type.Assembly.FullName);
                return;
            }

            if (DefaultBindingIn(kernel) && type.Assembly.FullName != _serviceAssembly)
            {
                kernel.Rebind(service).To(type).InScope(scopeCallback).WithMetadata("assembly", type.Assembly.FullName);
                return;
            }

            kernel.Bind(service).To(type).InScope(scopeCallback).WithMetadata("assembly", type.Assembly.FullName);
        }

        private bool NoBindingsIn(IKernel kernel)
        {
            return !kernel.GetBindings(typeof(TService)).Any(HasAssemblyKey);
        }

        private bool DefaultBindingIn(IKernel kernel)
        {
            return kernel.GetBindings(typeof (TService)).Any(IsServiceAsemblyBinding);

        }

        private bool HasAssemblyKey(IBinding b)
        {
            var satisifies = b.Metadata.Has("assembly");
            return satisifies;
        }

        private bool IsServiceAsemblyBinding(IBinding b)
        {
            var haskey = HasAssemblyKey(b);
            if (!haskey) return false;
            var satisfies = b.Metadata.Get<string>("assembly") == _serviceAssembly;
            return satisfies;
        }
    }
}