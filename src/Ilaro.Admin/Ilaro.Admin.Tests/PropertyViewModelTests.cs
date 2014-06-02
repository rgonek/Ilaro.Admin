using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Ilaro.Admin.ViewModels;
using Ilaro.Admin;
using System.Linq;

namespace Ilaro.Admin.Tests
{
	[TestClass]
	public class PropertyViewModelTests
	{
		[TestMethod]
		public void SetForeigns_SimpleTypes_IsNotForeignKey()
		{
			var entityType = typeof(TestEntity);
			var property = new Property(null, entityType.GetProperty("Id"));
			Assert.IsFalse(property.IsForeignKey);

			property = new Property(null, entityType.GetProperty("Name"));
			Assert.IsFalse(property.IsForeignKey);

			property = new Property(null, entityType.GetProperty("IsSpecial"));
			Assert.IsFalse(property.IsForeignKey);

			property = new Property(null, entityType.GetProperty("DateAdd"));
			Assert.IsFalse(property.IsForeignKey);

			property = new Property(null, entityType.GetProperty("Price"));
			Assert.IsFalse(property.IsForeignKey);

			property = new Property(null, entityType.GetProperty("Percent"));
			Assert.IsFalse(property.IsForeignKey);
		}

		[TestMethod]
		public void SetForeigns_Collections_IsNotForeignKey()
		{
			var entityType = typeof(TestEntity);
			var property = new Property(null, entityType.GetProperty("Tags"));
			Assert.IsFalse(property.IsForeignKey);

			property = new Property(null, entityType.GetProperty("Dimensions"));
			Assert.IsFalse(property.IsForeignKey);
		}

		[TestMethod]
		public void SetForeigns_Collections_IsForeignKey()
		{
			var entityType = typeof(TestEntity);
			var property = new Property(null, entityType.GetProperty("Siblings"));
			Assert.IsTrue(property.IsForeignKey);
		}

		[TestMethod]
		public void SetForeigns_Enums_IsNotForeignKey()
		{
			var entityType = typeof(TestEntity);
			var property = new Property(null, entityType.GetProperty("Option"));
			Assert.IsFalse(property.IsForeignKey);

			property = new Property(null, entityType.GetProperty("SplitOption"));
			Assert.IsFalse(property.IsForeignKey);
		}

		[TestMethod]
		public void SetForeigns_ComplexTypes_IsForeignKey()
		{
			var entityType = typeof(TestEntity);
			var property = new Property(null, entityType.GetProperty("Parent"));
			Assert.IsTrue(property.IsForeignKey);

			property = new Property(null, entityType.GetProperty("Child"));
			Assert.IsTrue(property.IsForeignKey);
		}

		[TestMethod]
		public void SetForeigns_SimpleTypForeignAttribute_IsForeignKey()
		{
			var entityType = typeof(TestEntity);
			var property = new Property(null, entityType.GetProperty("RoleId"));
			Assert.IsTrue(property.IsForeignKey);
		}

		[TestMethod]
		public void SetForeigns_ForeignAttribute_SetForeignKeysReferences()
		{
			AdminInitialise.AddEntity<TestEntity>();
			AdminInitialise.SetForeignKeysReferences();

			var testEntity = AdminInitialise.EntitiesTypes.FirstOrDefault();
			var property = testEntity.Properties.FirstOrDefault(x => x.Name == "Parent");
			Assert.IsTrue(property.IsForeignKey);

			property = testEntity.Properties.FirstOrDefault(x => x.Name == "ParentId");
			Assert.IsFalse(property.IsForeignKey);

			property = testEntity.Properties.FirstOrDefault(x => x.Name == "Child");
			Assert.IsTrue(property.IsForeignKey);

			property = testEntity.Properties.FirstOrDefault(x => x.Name == "ChildId");
			Assert.IsTrue(property.IsForeignKey);

			property = testEntity.Properties.FirstOrDefault(x => x.Name == "RoleId");
			Assert.IsTrue(property.IsForeignKey);
		}
	}
}
