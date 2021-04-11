using Ilaro.Admin.Core.Configuration.Configurators;

namespace Ilaro.Admin.Core.Configuration
{
    /// <summary>
    /// Helper class for fluent entity configurator
    /// </summary>
    public class Entity<TEntity> where TEntity : class
    {
        /// <summary>
        /// Add entity to Ilaro.Admin
        /// </summary>
        public static EntityConfigurator<TEntity> Register()
        {
            var customizerHolder = new ConfiguratorsHolder(typeof(TEntity));
            Admin.AddConfigurator(customizerHolder);
            return new EntityConfigurator<TEntity>(customizerHolder);
        }
    }
}