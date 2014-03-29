using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace Ilaro.Sample.Models.TestAdmin
{
    public class TestAdminContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
    }
}