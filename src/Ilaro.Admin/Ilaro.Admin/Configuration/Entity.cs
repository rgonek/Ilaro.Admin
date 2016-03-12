using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Ilaro.Admin.Core;

namespace Ilaro.Admin.Configuration
{
    /// <summary>
    /// Fluent entity configurator
    /// </summary>
    public class Entity<TEntity> where TEntity : class
    {
        private Entity _entity;

        /// <summary>
        /// Add entity to Ilaro.Admin
        /// </summary>
        public static Entity<TEntity> Register()
        {
            return new Entity<TEntity>
            {
                _entity = Admin.RegisterEntity<TEntity>()
            };
        }

        /// <summary>
        /// Set SQL table name and schema
        /// </summary>
        public Entity<TEntity> SetTableName(string table, string schema = null)
        {
            _entity.SetTableName(table, schema);

            return this;
        }

        /// <summary>
        /// Set which properties are display in entity list
        /// </summary>
        /// <param name="expressions"></param>
        /// <returns></returns>
        public Entity<TEntity> SetColumns(
            params Expression<Func<TEntity, object>>[] expressions)
        {
            var members = GetMemberExpressions(expressions);
            var properties = members.Select(x => x.Member.Name);
            _entity.SetColumns(properties);

            return this;
        }

        /// <summary>
        /// Set on which properties searching should be done
        /// </summary>
        public Entity<TEntity> SetSearchProperties(
            params Expression<Func<TEntity, object>>[] expressions)
        {
            var members = GetMemberExpressions(expressions);

            var properties = members.Select(x => x.Member.Name).ToList();
            _entity.SetSearchProperties(properties);

            return this;
        }

        /// <summary>
        /// Add property group, it used to display create or edit form
        /// </summary>
        public Entity<TEntity> AddPropertiesGroup(
            string group,
            params Expression<Func<TEntity, object>>[] properties)
        {
            return AddPropertiesGroup(group, false, properties);
        }

        /// <summary>
        /// Add property group, it used to display create or edit form
        /// </summary>
        public Entity<TEntity> AddPropertiesGroup(
            string group,
            bool isCollapsed,
            params Expression<Func<TEntity, object>>[] properties)
        {
            var members = GetMemberExpressions(properties);
            var propertiesNames = members.Select(x => x.Member.Name);
            _entity.AddGroup(group, isCollapsed, propertiesNames);

            return this;
        }

        /// <summary>
        /// Set display format, it used to display entity record name on 
        /// delete or on create or update form
        /// </summary>
        /// <param name="displayFormat">
        /// Display format can hold a property name, eg. "Item of id #{Id}"
        /// </param>
        /// <returns></returns>
        public Entity<TEntity> SetDisplayFormat(string displayFormat)
        {
            _entity.RecordDisplayFormat = displayFormat;

            return this;
        }

        /// <summary>
        /// Set primary key for entity
        /// </summary>
        public Entity<TEntity> SetKey<TProperty>(
            Expression<Func<TEntity, TProperty>> expression)
        {
            var property = (expression.Body as MemberExpression).Member.Name;
            _entity.SetKey(property);

            return this;
        }

        /// <summary>
        /// Set singular and plural display name
        /// </summary>
        public Entity<TEntity> SetDisplayName(string singular, string plural)
        {
            _entity.Verbose.Singular = singular;
            _entity.Verbose.Plural = plural;

            return this;
        }

        /// <summary>
        /// Set entity group
        /// </summary>
        public Entity<TEntity> SetGroup(string group)
        {
            _entity.Verbose.Group = group;

            return this;
        }


        /// <summary>
        /// Set display link
        /// </summary>
        public Entity<TEntity> SetDisplayLink(string link)
        {
            _entity.Links.Display = link;

            return this;
        }

        /// <summary>
        /// Set edit link
        /// </summary>
        public Entity<TEntity> SetEditLink(string link)
        {
            _entity.Links.Edit = link;
            _entity.Links.HasEdit = true;

            return this;
        }

        /// <summary>
        /// Set delete link
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        public Entity<TEntity> SetDeleteLink(string link)
        {
            _entity.Links.Delete = link;
            _entity.Links.HasDelete = true;

            return this;
        }

        /// <summary>
        /// Configure entity property
        /// </summary>
        public Entity<TEntity> ConfigureProperty(PropertyOf<TEntity> property)
        {
            return this;
        }

        private IEnumerable<MemberExpression> GetMemberExpressions(
            Expression<Func<TEntity, object>>[] expressions)
        {
            var members = expressions
                .Select(x => x.Body)
                .OfType<MemberExpression>();

            members = members.Union(
                expressions
                    .Select(x => x.Body)
                    .OfType<UnaryExpression>()
                    .Select(x => x.Operand)
                    .OfType<MemberExpression>());

            return members;
        }
    }
}