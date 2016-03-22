using Ilaro.Admin.Configuration.Customizers;

namespace Ilaro.Admin.Configuration
{
    /// <summary>
    /// Helper class for fluent entity configurator
    /// </summary>
    public class Entity<TEntity> where TEntity : class
    {
        /// <summary>
        /// Add entity to Ilaro.Admin
        /// </summary>
        public static EntityCustomizer<TEntity> Register()
        {
            var customizerHolder = new CustomizersHolder(typeof(TEntity));
            Admin.AddCustomizer(customizerHolder);
            return new EntityCustomizer<TEntity>(customizerHolder);
        }
    }
}