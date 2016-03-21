using Ilaro.Admin.Configuration.Customizers;

namespace Ilaro.Admin.Configuration
{
    public class EntityConfiguration<TEntity> : EntityCustomizer<TEntity> where TEntity : class
    {
        public EntityConfiguration() : base(new CustomizersHolder(typeof(TEntity))) { }
    }
}