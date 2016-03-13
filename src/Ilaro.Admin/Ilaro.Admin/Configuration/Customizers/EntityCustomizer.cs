using System;
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

        public void Table(string tableName)
        {
            CustomizersHolder.Table(tableName);
        }

        public void Id(params Expression<Func<TEntity, object>>[] idProperties)
        {
            var membersOf = TypeExtensions.DecodeMemberAccessExpressionOf(idProperties);
            CustomizersHolder.Id(membersOf);
        }

        public void DisplayProperties(params Expression<Func<TEntity, object>>[] displayProperties)
        {
            var membersOf = TypeExtensions.DecodeMemberAccessExpressionOf(displayProperties);
            CustomizersHolder.DisplayProperties(membersOf);
        }

        public void SearchProperties(params Expression<Func<TEntity, object>>[] searchProperties)
        {
            var membersOf = TypeExtensions.DecodeMemberAccessExpressionOf(searchProperties);
            CustomizersHolder.SearchProperties(membersOf);
        }

        public void DisplayLink(string displayLink)
        {
            CustomizersHolder.DisplayLink(displayLink);
        }

        public void EditLink(string editLink)
        {
            CustomizersHolder.EditLink(editLink);
        }

        public void DeleteLink(string deleteLink)
        {
            CustomizersHolder.DeleteLink(deleteLink);
        }

        public void Group(string group)
        {
            CustomizersHolder.Group(group);
        }

        public void DisplayFormat(string displayFormat)
        {
            CustomizersHolder.DisplayFormat(displayFormat);
        }

        public void Display(string singular, string plural)
        {
            CustomizersHolder.Display(singular, plural);
        }

        public void PropertiesGroup(string groupName, params Expression<Func<TEntity, object>>[] properties)
        {
            PropertiesGroup(groupName, false, properties);
        }

        public void PropertiesGroup(string groupName, bool isCollapsed, params Expression<Func<TEntity, object>>[] properties)
        {
            var membersOf = TypeExtensions.DecodeMemberAccessExpressionOf(properties);
            CustomizersHolder.PropertyGroup(groupName, isCollapsed, membersOf);
        }

        public void Property<TProperty>(Expression<Func<TEntity, TProperty>> property, Action<IPropertyCustomizer> customizer)
        {
            RegisterPropertyCustomizer(property, customizer);
        }

        protected virtual void RegisterPropertyCustomizer<TProperty>(Expression<Func<TEntity, TProperty>> property, Action<IPropertyCustomizer> customizer)
        {
            var memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
            CustomizersHolder.Property(memberOf, customizer);
        }
    }
}