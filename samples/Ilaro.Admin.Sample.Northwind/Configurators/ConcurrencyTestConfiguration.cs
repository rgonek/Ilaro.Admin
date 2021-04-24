using Ilaro.Admin.Core.Configuration;
using Ilaro.Admin.Sample.Northwind.Models;

namespace Ilaro.Admin.Sample.Northwind.Configurators
{
    public class ConcurrencyTestConfiguration : EntityConfiguration<ConcurrencyTest>
    {
        public ConcurrencyTestConfiguration()
        {
            Property(x => x.LastModification, x => x.OnSave(Core.DataAccess.ValueBehavior.UtcNow).IsConcurrencyCheck());
        }
    }
}
