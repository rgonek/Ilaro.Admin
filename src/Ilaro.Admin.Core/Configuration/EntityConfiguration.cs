using Ilaro.Admin.Core.Configuration.Configurators;

namespace Ilaro.Admin.Core.Configuration
{
    public class EntityConfiguration<TEntity> : EntityConfigurator<TEntity> where TEntity : class
    {
        public EntityConfiguration() : base(new ConfiguratorsHolder(typeof(TEntity))) { }
    }
}