using Microsoft.Extensions.DependencyInjection;

namespace Ilaro.Admin.Extensions.Microsoft.DependencyInjection
{
    public class IlaroAdminServiceConfiguration
    {
        public ServiceLifetime Lifetime { get; private set; }

        public IlaroAdminServiceConfiguration()
            => Lifetime = ServiceLifetime.Transient;

        public IlaroAdminServiceConfiguration AsSingleton()
        {
            Lifetime = ServiceLifetime.Singleton;
            return this;
        }

        public IlaroAdminServiceConfiguration AsScoped()
        {
            Lifetime = ServiceLifetime.Scoped;
            return this;
        }

        public IlaroAdminServiceConfiguration AsTransient()
        {
            Lifetime = ServiceLifetime.Transient;
            return this;
        }
    }
}
