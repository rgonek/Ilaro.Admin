using Ilaro.Sample.Models.Northwind.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace Ilaro.Sample.Models.Northwind
{
	public class NorthwindContext : DbContext
	{
		public DbSet<Customer> Customers { get; set; }
		public DbSet<Employee> Employees { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderDetail> OrderDetails { get; set; }
		public DbSet<Product> Products { get; set; }

		// Use the constructor to target a specific named connection string
		public NorthwindContext()
			: base("name=NorthwindEntities")
		{
			// Disable proxy creation as this messes up the data service.
			this.Configuration.ProxyCreationEnabled = false;

			// Create Northwind if it doesn't already exist.
			//this.Database.CreateIfNotExists();
		}

		// Disable creation of the EdmMetadata table.
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<IncludeMetadataConvention>();
		}
	}
}