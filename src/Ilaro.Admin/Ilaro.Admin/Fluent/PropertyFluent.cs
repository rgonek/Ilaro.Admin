using Ilaro.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Ilaro.Admin.Fluent
{
	public class Property<TEntity>
	{
		private Property _property;

		public static Property<TEntity> Configure<TProperty>(Expression<Func<TEntity, TProperty>> expression)
		{
			var entityName = typeof(TEntity).Name;
			var propertyName = (expression.Body as MemberExpression).Member.Name;

			return new Property<TEntity>
			{
				_property = AdminInitialise.EntitiesTypes.FirstOrDefault(x => x.Name == entityName)[propertyName]
			};
		}

		public Property<TEntity> SetColumnName(string columnName)
		{
			_property.ColumnName = columnName;

			return this;
		}

		public Property<TEntity> OnDelete(DeleteOption deleteOption)
		{
			_property.DeleteOption = deleteOption;

			return this;
		}

		public Property<TEntity> IsKey()
		{
			_property.IsKey = true;

			return this;
		}

		public Property<TEntity> IsLinkKey()
		{
			_property.IsLinkKey = true;

			return this;
		}

		public Property<TEntity> SetDisplayName(string name, string description = "")
		{
			_property.DisplayName = name;
			_property.Description = description;

			return this;
		}

		public Property<TEntity> SetDisplayTemplate(string template)
		{
			_property.DisplayTemplateName = template;

			return this;
		}

		public Property<TEntity> SetEditorTemplate(string template)
		{
			_property.EditorTemplateName = template;

			return this;
		}

		public Property<TEntity> SetDataType(Ilaro.Admin.ViewModels.DataType dataType)
		{
			_property.DataType = dataType;

			return this;
		}
	}
}