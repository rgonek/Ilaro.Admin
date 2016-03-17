﻿using System;
using System.Linq.Expressions;

namespace Ilaro.Admin.Configuration.Customizers
{
    public class EntityCustomizer<TEntity> : IEntityCustomizer where TEntity : class
    {
        public ICustomizersHolder CustomizersHolder { get; }

        public EntityCustomizer(ICustomizersHolder customizersHolder)
        {
            if (customizersHolder == null)
                throw new ArgumentNullException("customizersHolder");

            CustomizersHolder = customizersHolder;
        }

        /// <summary>
        /// Set SQL table name and schema
        /// </summary>
        public EntityCustomizer<TEntity> Table(string tableName, string schema = null)
        {
            CustomizersHolder.Table(tableName, schema);

            return this;
        }

        /// <summary>
        /// Set primary key for entity
        /// </summary>
        public EntityCustomizer<TEntity> Id(
            params Expression<Func<TEntity, object>>[] idProperties)
        {
            var membersOf = TypeExtensions.DecodeMemberAccessExpressionOf(idProperties);
            CustomizersHolder.Id(membersOf);

            return this;
        }

        /// <summary>
        /// Set which properties are display in entity list
        /// </summary>
        public EntityCustomizer<TEntity> DisplayProperties(
            params Expression<Func<TEntity, object>>[] displayProperties)
        {
            var membersOf = TypeExtensions.DecodeMemberAccessExpressionOf(displayProperties);
            CustomizersHolder.DisplayProperties(membersOf);

            return this;
        }

        /// <summary>
        /// Set on which properties searching should be done
        /// </summary>
        public EntityCustomizer<TEntity> SearchProperties(
            params Expression<Func<TEntity, object>>[] searchProperties)
        {
            var membersOf = TypeExtensions.DecodeMemberAccessExpressionOf(searchProperties);
            CustomizersHolder.SearchProperties(membersOf);

            return this;
        }

        /// <summary>
        /// Set display link
        /// </summary>
        public EntityCustomizer<TEntity> DisplayLink(string displayLink)
        {
            CustomizersHolder.DisplayLink(displayLink);

            return this;
        }

        /// <summary>
        /// Set edit link
        /// </summary>
        public EntityCustomizer<TEntity> EditLink(string editLink)
        {
            CustomizersHolder.EditLink(editLink);

            return this;
        }

        /// <summary>
        /// Set delete link
        /// </summary>
        public EntityCustomizer<TEntity> DeleteLink(string deleteLink)
        {
            CustomizersHolder.DeleteLink(deleteLink);

            return this;
        }

        /// <summary>
        /// Set entity group
        /// </summary>
        public EntityCustomizer<TEntity> Group(string group)
        {
            CustomizersHolder.Group(group);

            return this;
        }

        /// <summary>
        /// Set display format, it used to display entity record name on 
        /// delete or on create or update form
        /// </summary>
        /// <param name="displayFormat">
        /// Display format can hold a property name, eg. "Item of id #{Id}"
        /// </param>
        public EntityCustomizer<TEntity> DisplayFormat(string displayFormat)
        {
            CustomizersHolder.DisplayFormat(displayFormat);

            return this;
        }

        /// <summary>
        /// Set singular and plural display name
        /// </summary>
        public EntityCustomizer<TEntity> Display(string singular, string plural)
        {
            CustomizersHolder.Display(singular, plural);

            return this;
        }

        /// <summary>
        /// Add property group, it used to display create or edit form
        /// </summary>
        public EntityCustomizer<TEntity> PropertiesGroup(
            string groupName,
            params Expression<Func<TEntity, object>>[] properties)
        {
            PropertiesGroup(groupName, false, properties);

            return this;
        }

        /// <summary>
        /// Add property group, it used to display create or edit form
        /// </summary>
        public EntityCustomizer<TEntity> PropertiesGroup(
            string groupName,
            bool isCollapsed,
            params Expression<Func<TEntity, object>>[] properties)
        {
            var membersOf = TypeExtensions.DecodeMemberAccessExpressionOf(properties);
            CustomizersHolder.PropertyGroup(groupName, isCollapsed, membersOf);

            return this;
        }

        /// <summary>
        /// Configure entity property
        /// </summary>
        public EntityCustomizer<TEntity> Property<TProperty>(
            Expression<Func<TEntity, TProperty>> property,
            Action<IPropertyCustomizer> customizer)
        {
            RegisterPropertyCustomizer(property, customizer);

            return this;
        }

        protected virtual void RegisterPropertyCustomizer<TProperty>(
            Expression<Func<TEntity, TProperty>> property,
            Action<IPropertyCustomizer> customizer)
        {
            var memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
            CustomizersHolder.Property(memberOf, customizer);
        }
    }
}