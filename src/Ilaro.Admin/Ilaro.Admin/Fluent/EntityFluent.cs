using Ilaro.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Ilaro.Admin.Fluent
{
	public class Entity<TEntity>
	{
		private Entity _entity;

		public static Entity<TEntity> Add()
		{
			return new Entity<TEntity>
			{
				_entity = AdminInitialise.AddEntity<TEntity>()
			};
		}

		public Entity<TEntity> SetTableName(string table, string schema = null)
		{
			_entity.SetTableName(table, schema);

			return this;
		}

		public Entity<TEntity> SetDisplayProperties(params Expression<Func<TEntity, object>>[] expressions)
		{
			var members = expressions.Select(x => x.Body).OfType<MemberExpression>();
			var properties = members.Select(x => x.Member.Name);
			_entity.SetDisplayProperties(properties);

			return this;
		}

		public Entity<TEntity> SetSearchProperties(params Expression<Func<TEntity, object>>[] expressions)
		{
			var members = expressions.Select(x => x.Body).OfType<MemberExpression>();
			var properties = members.Select(x => x.Member.Name);
			_entity.SetSearchProperties(properties);

			return this;
		}

		public Entity<TEntity> AddPropertiesGroup(string group, params Expression<Func<TEntity, object>>[] expressions)
		{
			return AddPropertiesGroup(group, false, expressions);
		}

		public Entity<TEntity> AddPropertiesGroup(string group, bool isCollapsed, params Expression<Func<TEntity, object>>[] expressions)
		{
			var members = expressions.Select(x => x.Body).OfType<MemberExpression>();
			var properties = members.Select(x => x.Member.Name);
			_entity.AddGroup(group, isCollapsed, properties);

			return this;
		}

		public Entity<TEntity> SetDisplayFormat(string displayFormat)
		{
			_entity.RecordDisplayFormat = displayFormat;

			return this;
		}

		public Entity<TEntity> SetKey<TProperty>(Expression<Func<TEntity, TProperty>> expression)
		{
			var property = (expression.Body as MemberExpression).Member.Name;
			_entity.SetKey(property);

			return this;
		}

		public Entity<TEntity> SetLinkKey<TProperty>(Expression<Func<TEntity, TProperty>> expression)
		{
			var property = (expression.Body as MemberExpression).Member.Name;
			_entity.SetLinkKey(property);

			return this;
		}

		public Entity<TEntity> SetDisplayName(string singular, string plural)
		{
			_entity.Singular = singular;
			_entity.Plural = plural;

			return this;
		}

		public Entity<TEntity> SetGroup(string group)
		{
			_entity.GroupName = group;

			return this;
		}

		public Entity<TEntity> SetDisplayLink(string link)
		{
			_entity.DisplayLink = link;

			return this;
		}

		public Entity<TEntity> SetEditLink(string link)
		{
			_entity.EditLink = link;
			_entity.HasEditLink = true;

			return this;
		}

		public Entity<TEntity> SetDeleteLink(string link)
		{
			_entity.DeleteLink = link;
			_entity.HasDeleteLink = true;

			return this;
		}

		public Entity<TEntity> ConfigureProperty(Property<TEntity> prop)
		{
			return this;
		}
	}
}