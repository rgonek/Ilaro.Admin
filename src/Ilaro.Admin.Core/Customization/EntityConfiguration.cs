using Ilaro.Admin.Core.Customization.Customizers;

namespace Ilaro.Admin.Core.Customization
{
    public class EntityConfiguration<TEntity> : EntityCustomizer<TEntity> where TEntity : class
    {
        public EntityConfiguration() : base(new CustomizersHolder(typeof(TEntity))) { }
    }
}