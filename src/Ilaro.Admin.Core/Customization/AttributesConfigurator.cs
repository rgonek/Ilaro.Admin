﻿using Ilaro.Admin.Core.Customization.Customizers;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Ilaro.Admin.Core.DataAnnotations;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using Ilaro.Admin.Core.Extensions;
using EntityConcurrencyCheckAttribute = Ilaro.Admin.Core.DataAnnotations.ConcurrencyCheckAttribute;
using PropertyConcurrencyCheckAttribute = System.ComponentModel.DataAnnotations.ConcurrencyCheckAttribute;
using System;

namespace Ilaro.Admin.Core.Customization
{
    internal static class AttributesConfigurator
    {
        internal static void Initialise(ICustomizersHolder customizerHolder)
        {
            var attributes = customizerHolder.Type.GetCustomAttributes(false);

            Table(customizerHolder, attributes);
            SearchProperties(customizerHolder, attributes);
            DisplayFormat(customizerHolder, attributes);
            Display(customizerHolder, attributes);
            Links(customizerHolder, attributes);
            Editable(customizerHolder, attributes);
            Deletable(customizerHolder, attributes);
            Columns(customizerHolder, attributes);
            Groups(customizerHolder, attributes);
            SoftDelete(customizerHolder, attributes);
            ConcurrencyCheck(customizerHolder, attributes);

            foreach (var member in customizerHolder.Type.GetProperties())
            {
                attributes = member.GetCustomAttributes(false);

                DataType(member, customizerHolder, attributes);
                FileOptions(member, customizerHolder, attributes);
                ImageSettings(member, customizerHolder, attributes);
                Template(member, customizerHolder, attributes);
                Id(member, customizerHolder, attributes);
                OnCreate(member, customizerHolder, attributes);
                OnUpdate(member, customizerHolder, attributes);
                OnSave(member, customizerHolder, attributes);
                OnDelete(member, customizerHolder, attributes);
                Cascade(member, customizerHolder, attributes);
                Column(member, customizerHolder, attributes);
                Display(member, customizerHolder, attributes);
                DisplayFormat(member, customizerHolder, attributes);
                Required(member, customizerHolder, attributes);
                ForeignKey(member, customizerHolder, attributes);
                Validation(member, customizerHolder, attributes);
                Timestamp(member, customizerHolder, attributes);
                ConcurrencyCheck(member, customizerHolder, attributes);
                DefaultOrder(member, customizerHolder, attributes);
                DefaultFilter(member, customizerHolder, attributes);
                Filterable(member, customizerHolder, attributes);
            }
        }

        private static void Table(
            ICustomizersHolder customizerHolder,
            object[] attributes)
        {
            var attribute = attributes.GetAttribute<TableAttribute>();
            if (attribute != null)
            {
                customizerHolder.Table(attribute.Name, attribute.Schema);
            }
        }

        private static void SearchProperties(
            ICustomizersHolder customizerHolder,
            object[] attributes)
        {
            var attribute = attributes.GetAttribute<SearchAttribute>();
            if (attribute != null)
            {
                var members = customizerHolder.Type.GetProperties()
                    .Where(x => attribute.Columns.Contains(x.Name));
                customizerHolder.SearchProperties(members);
            }
        }

        private static void Display(
            ICustomizersHolder customizerHolder,
            object[] attributes)
        {
            var attribute = attributes.GetAttribute<VerboseAttribute>();
            if (attribute != null)
            {
                customizerHolder.Display(attribute.Singular, attribute.Plural);
                customizerHolder.Group(attribute.GroupName);
            }
        }

        private static void Links(
            ICustomizersHolder customizerHolder,
            object[] attributes)
        {
            var attribute = attributes.GetAttribute<LinksAttribute>();
            if (attribute != null)
            {
                customizerHolder.Link(
                    attribute.DisplayLink,
                    attribute.EditLink,
                    attribute.DeleteLink);
            }
        }

        private static void Editable(
            ICustomizersHolder customizerHolder,
            object[] attributes)
        {
            var attribute = attributes.GetAttribute<EditableAttribute>();
            if (attribute != null)
            {
                customizerHolder.Editable(attribute.AllowEdit);
            }
        }

        private static void Deletable(
            ICustomizersHolder customizerHolder,
            object[] attributes)
        {
            var attribute = attributes.GetAttribute<DeletableAttribute>();
            if (attribute != null)
            {
                customizerHolder.Deletable(attribute.AllowDelete);
            }
        }

        private static void SoftDelete(
            ICustomizersHolder customizerHolder,
            object[] attributes)
        {
            var attribute = attributes.GetAttribute<SoftDeleteAttribute>();
            if (attribute != null)
            {
                customizerHolder.SoftDelete();
            }
        }

        private static void DisplayFormat(
            ICustomizersHolder customizerHolder,
            object[] attributes)
        {
            var attribute = attributes.GetAttribute<RecordDisplayAttribute>();
            if (attribute != null)
            {
                customizerHolder.DisplayFormat(attribute.DisplayFormat);
            }
        }

        private static void Columns(
            ICustomizersHolder customizerHolder,
            object[] attributes)
        {
            var attribute = attributes.GetAttribute<ColumnsAttribute>();
            if (attribute != null)
            {
                var members = customizerHolder.Type.GetProperties()
                    .Where(x => attribute.Columns.Contains(x.Name));
                customizerHolder.DisplayProperties(members);
            }
        }

        private static void Groups(
            ICustomizersHolder customizerHolder,
            object[] attributes)
        {
            var attribute = attributes.GetAttribute<GroupsAttribute>();
            if (attribute != null)
            {
                var membersByGroup = customizerHolder.Type.GetProperties()
                    .Where(x => x.GetCustomAttribute<DisplayAttribute>() != null)
                    .GroupBy(x => x.GetCustomAttribute<DisplayAttribute>().GroupName)
                    .ToDictionary(x => x.Key, x => x.ToList());
                foreach (var group in attribute.Groups)
                {
                    var trimmedGroup = group.TrimEnd('*');
                    var properties = Enumerable.Empty<PropertyInfo>();
                    if (membersByGroup.ContainsKey(group))
                        properties = properties.Union(membersByGroup[group]);
                    if (membersByGroup.ContainsKey(trimmedGroup))
                        properties = properties.Union(membersByGroup[trimmedGroup]);
                    customizerHolder.PropertyGroup(trimmedGroup, group.EndsWith("*"), properties);
                }
            }
        }

        private static void DataType(
            MemberInfo member,
            ICustomizersHolder customizerHolder,
            object[] attributes)
        {
            var dataTypeAttribute = attributes.GetAttribute<DataTypeAttribute>();
            if (dataTypeAttribute != null)
            {
                customizerHolder.Property(member, x =>
                {
                    x.Type(dataTypeAttribute.DataType);
                });
                return;
            }

            var enumDataTypeAttribute = attributes.GetAttribute<EnumDataTypeAttribute>();
            if (enumDataTypeAttribute != null)
            {
                customizerHolder.Property(member, x =>
                {
                    x.Enum(enumDataTypeAttribute.EnumType);
                });
                return;
            }

            var imageAttribute = attributes.GetAttribute<ImageSettingsAttribute>();
            if (imageAttribute != null)
            {
                customizerHolder.Property(member, x =>
                {
                    x.Type(Core.DataType.Image);
                });
                return;
            }

            var fileAttribute = attributes.GetAttribute<FileAttribute>();
            if (fileAttribute != null)
            {
                customizerHolder.Property(member, x =>
                {
                    x.Type(fileAttribute.IsImage ?
                        Core.DataType.Image :
                        Core.DataType.File);
                });
                return;
            }
        }

        private static void FileOptions(
            MemberInfo member,
            ICustomizersHolder customizerHolder,
            object[] attributes)
        {
            var fileAttribute = attributes.GetAttribute<FileAttribute>();
            if (fileAttribute != null)
            {
                customizerHolder.Property(member, x =>
                {
                    x.File(
                        fileAttribute.NameCreation,
                        fileAttribute.MaxFileSize,
                        fileAttribute.IsImage,
                        fileAttribute.Path,
                        fileAttribute.AllowedFileExtensions);
                });
            }
        }

        private static void ImageSettings(
            MemberInfo member,
            ICustomizersHolder customizerHolder,
            object[] attributes)
        {
            var imageSettingsAttributes = attributes
                .GetAttributes<ImageSettingsAttribute>().ToList();
            if (imageSettingsAttributes.IsNullOrEmpty() == false)
            {
                customizerHolder.Property(member, x =>
                {
                    foreach (var imageAttribute in imageSettingsAttributes)
                    {
                        x.Image(
                            imageAttribute.Settings.SubPath,
                            imageAttribute.Settings.Width,
                            imageAttribute.Settings.Height);
                    }
                });
            }
        }

        private static void OnCreate(
            MemberInfo member,
            ICustomizersHolder customizerHolder,
            object[] attributes)
        {
            var attribute = attributes.GetAttribute<OnCreateAttribute>();
            if (attribute != null)
            {
                customizerHolder.Property(member, x =>
                {
                    x.OnCreate(attribute.Value);
                });
            }
        }

        private static void OnUpdate(
            MemberInfo member,
            ICustomizersHolder customizerHolder,
            object[] attributes)
        {
            var attribute = attributes.GetAttribute<OnUpdateAttribute>();
            if (attribute != null)
            {
                customizerHolder.Property(member, x =>
                {
                    x.OnUpdate(attribute.Value);
                });
            }
        }

        private static void OnSave(
            MemberInfo member,
            ICustomizersHolder customizerHolder,
            object[] attributes)
        {
            var attribute = attributes.GetAttribute<OnSaveAttribute>();
            if (attribute != null)
            {
                customizerHolder.Property(member, x =>
                {
                    x.OnSave(attribute.Value);
                });
            }
        }

        private static void OnDelete(
            MemberInfo member,
            ICustomizersHolder customizerHolder,
            object[] attributes)
        {
            var attribute = attributes.GetAttribute<OnDeleteAttribute>();
            if (attribute != null)
            {
                customizerHolder.Property(member, x =>
                {
                    x.OnDelete(attribute.Value);
                });
            }
        }

        private static void Template(
            MemberInfo member,
            ICustomizersHolder customizerHolder,
            object[] attributes)
        {
            var uiHintAttribute = attributes.GetAttribute<UIHintAttribute>();
            var templateAttribute = attributes.GetAttribute<TemplateAttribute>();
            if (uiHintAttribute != null || templateAttribute != null)
            {
                string editor = null, display = null;
                if (uiHintAttribute != null)
                {
                    editor = display = uiHintAttribute.UIHint;
                }
                if (templateAttribute != null)
                {
                    if (!templateAttribute.DisplayTemplate.IsNullOrEmpty())
                    {
                        display = templateAttribute.DisplayTemplate;
                    }
                    if (!templateAttribute.EditorTemplate.IsNullOrEmpty())
                    {
                        editor = templateAttribute.EditorTemplate;
                    }
                }

                customizerHolder.Property(member, x =>
                {
                    x.Template(display, editor);
                });
            }
        }

        private static void Id(
            MemberInfo member,
            ICustomizersHolder customizerHolder,
            object[] attributes)
        {
            var attribute = attributes.GetAttribute<KeyAttribute>();
            if (attribute != null)
            {
                customizerHolder.Property(member, x =>
                {
                    x.Id();
                });
            }
        }

        private static void Cascade(
            MemberInfo member,
            ICustomizersHolder customizerHolder,
            object[] attributes)
        {
            var attribute = attributes.GetAttribute<CascadeAttribute>();
            if (attribute != null)
            {
                customizerHolder.Property(member, x =>
                {
                    x.Cascade(attribute.DeleteOption);
                });
            }
        }

        private static void Column(
            MemberInfo member,
            ICustomizersHolder customizerHolder,
            object[] attributes)
        {
            var attribute = attributes.GetAttribute<ColumnAttribute>();
            if (attribute != null)
            {
                customizerHolder.Property(member, x =>
                {
                    x.Column(attribute.Name);
                });
            }
        }

        private static void Display(
            MemberInfo member,
            ICustomizersHolder customizerHolder,
            object[] attributes)
        {
            var attribute = attributes.GetAttribute<DisplayAttribute>();
            if (attribute != null)
            {
                customizerHolder.Property(member, x =>
                {
                    x.Display(attribute.Name, attribute.Description);
                });
            }
        }

        private static void DisplayFormat(
            MemberInfo member,
            ICustomizersHolder customizerHolder,
            object[] attributes)
        {
            var attribute = attributes.GetAttribute<DisplayFormatAttribute>();
            if (attribute != null)
            {
                customizerHolder.Property(member, x =>
                {
                    x.Format(attribute.DataFormatString);
                });
            }
        }

        private static void Required(
            MemberInfo member,
            ICustomizersHolder customizerHolder,
            object[] attributes)
        {
            var attribute = attributes.GetAttribute<RequiredAttribute>();
            if (attribute != null)
            {
                customizerHolder.Property(member, x =>
                {
                    x.Required(attribute.ErrorMessage);
                });
            }
        }

        private static void ForeignKey(
            MemberInfo member,
            ICustomizersHolder customizerHolder,
            object[] attributes)
        {
            var attribute = attributes.GetAttribute<ForeignKeyAttribute>();
            if (attribute != null)
            {
                customizerHolder.Property(member, x =>
                {
                    x.ForeignKey(attribute.Name);
                });
            }
        }

        private static void Validation(
            PropertyInfo member,
            ICustomizersHolder customizerHolder,
            object[] attributes)
        {
            var validators = attributes.GetAttributes<ValidationAttribute>();
            if (validators.IsNullOrEmpty() == false)
            {
                customizerHolder.Property(member, x =>
                {
                    x.Validators(validators);
                });
            }
        }

        private static void Timestamp(
            PropertyInfo member,
            ICustomizersHolder customizerHolder,
            object[] attributes)
        {
            var attribute = attributes.GetAttribute<TimestampAttribute>();
            if (attribute != null)
            {
                customizerHolder.Property(member, x =>
                {
                    x.IsTimestamp();
                });
            }
        }

        private static void ConcurrencyCheck(
            PropertyInfo member,
            ICustomizersHolder customizerHolder,
            object[] attributes)
        {
            var attribute = attributes.GetAttribute<PropertyConcurrencyCheckAttribute>();
            if (attribute != null)
            {
                customizerHolder.Property(member, x =>
                {
                    x.IsConcurrencyCheck();
                });
            }
        }

        private static void ConcurrencyCheck(
            ICustomizersHolder customizerHolder,
            object[] attributes)
        {
            var attribute = attributes.GetAttribute<EntityConcurrencyCheckAttribute>();
            if (attribute != null)
            {
                customizerHolder.ConcurrencyCheck();
            }
        }

        private static void DefaultOrder(
            PropertyInfo member,
            ICustomizersHolder customizerHolder,
            object[] attributes)
        {
            var attribute = attributes.GetAttribute<DefaultOrderAttribute>();
            if (attribute != null)
            {
                customizerHolder.Property(member, x =>
                {
                    x.DefaultOrder(attribute.OrderType);
                });
            }
        }

        private static void DefaultFilter(
            PropertyInfo member,
            ICustomizersHolder customizerHolder,
            object[] attributes)
        {
            var attribute = attributes.GetAttribute<DefaultFilterAttribute>();
            if (attribute != null)
            {
                customizerHolder.Property(member, x =>
                {
                    x.DefaultFilter(attribute.Value);
                });
            }
        }

        private static void Filterable(
            PropertyInfo member,
            ICustomizersHolder customizerHolder,
            object[] attributes)
        {
            var attribute = attributes.GetAttribute<FilterableAttribute>();
            if (attribute != null)
            {
                customizerHolder.Property(member, x =>
                {
                    x.Filterable();
                });
            }
        }

        private static void MultiValue(
            MemberInfo member,
            ICustomizersHolder customizerHolder,
            object[] attributes)
        {
            var attribute = attributes.GetAttribute<MultiValueAttribute>();
            if (attribute != null)
            {
                customizerHolder.Property(member, x =>
                {
                    x.MultiValue(attribute.Separator);
                });
            }
        }
    }
}