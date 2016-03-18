using Ilaro.Admin.Configuration.Customizers;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Ilaro.Admin.DataAnnotations;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using Ilaro.Admin.Core;
using Ilaro.Admin.Extensions;
using System.ComponentModel;
using System;

namespace Ilaro.Admin.Configuration
{
    internal static class AttributesConfigurator
    {
        internal static void Initialise(CustomizersHolder customizerHolder)
        {
            var attributes = customizerHolder.Type.GetCustomAttributes(false);

            Table(customizerHolder, attributes);
            SearchProperties(customizerHolder, attributes);
            DisplayFormat(customizerHolder, attributes);

            foreach (var member in customizerHolder.Type.GetMembers())
            {
                attributes = member.GetCustomAttributes(false);

                DataType(member, customizerHolder, attributes);
                FileOptions(member, customizerHolder, attributes);
                ImageSettings(member, customizerHolder, attributes);
                Template(member, customizerHolder, attributes);
                Id(member, customizerHolder, attributes);
                OnDelete(member, customizerHolder, attributes);
                Column(member, customizerHolder, attributes);
                Display(member, customizerHolder, attributes);
                PropertyDisplayFormat(member, customizerHolder, attributes);
                Required(member, customizerHolder, attributes);
                ForeignKey(member, customizerHolder, attributes);
            }
        }

        private static void Table(
            CustomizersHolder customizerHolder,
            object[] attributes)
        {
            var attribute = attributes.GetAttribute<TableAttribute>();
            if (attribute != null)
            {
                customizerHolder.Table(attribute.Name, attribute.Schema);
            }
        }

        private static void SearchProperties(
            CustomizersHolder customizerHolder,
            object[] attributes)
        {
            var attribute = attributes.GetAttribute<SearchAttribute>();
            if (attribute != null)
            {
                var members = customizerHolder.Type.GetMembers()
                    .Where(x => attribute.Columns.Contains(x.Name));
                customizerHolder.SearchProperties(members);
            }
        }

        private static void DisplayFormat(
            CustomizersHolder customizerHolder,
            object[] attributes)
        {
            var attribute = attributes.GetAttribute<RecordDisplayAttribute>();
            if (attribute != null)
            {
                customizerHolder.DisplayFormat(attribute.DisplayFormat);
            }
        }

        private static void SetColumns(
            CustomizersHolder customizerHolder,
            object[] attributes)
        {
            var attribute = attributes.GetAttribute<ColumnsAttribute>();
            if (attribute != null)
            {
                var members = customizerHolder.Type.GetMembers()
                    .Where(x => attribute.Columns.Contains(x.Name));
                customizerHolder.DisplayProperties(members);
            }
        }

        private static void SetGroups(
            CustomizersHolder customizerHolder,
            object[] attributes)
        {
            var attribute = attributes.GetAttribute<GroupsAttribute>();
            if (attribute != null)
            {
                var membersByGroup = customizerHolder.Type.GetMembers()
                    .GroupBy(x => x.GetCustomAttribute<DisplayAttribute>()?.GroupName)
                    .ToDictionary(x => x.Key, x => x.ToList());
                foreach (var group in attribute.Groups)
                {
                    customizerHolder.PropertyGroup(group.TrimEnd('*'), group.EndsWith("*"), membersByGroup[group]);
                }
            }
        }

        private static void DataType(
            MemberInfo member,
            CustomizersHolder customizerHolder,
            object[] attributes)
        {
            var dataTypeAttribute = attributes.GetAttribute<DataTypeAttribute>();
            if (dataTypeAttribute != null)
            {
                customizerHolder.Property(member, x =>
                {
                    //SourceDataType = dataTypeAttribute.DataType;
                    x.Type(DataTypeConverter.Convert(dataTypeAttribute.DataType));
                });
                return;
            }

            var enumDataTypeAttribute = attributes.GetAttribute<EnumDataTypeAttribute>();
            if (enumDataTypeAttribute != null)
            {
                customizerHolder.Property(member, x =>
                {
                    //EnumType = enumDataTypeAttribute.EnumType;
                    x.Type(Core.DataType.Enum);
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
            CustomizersHolder customizerHolder,
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
            CustomizersHolder customizerHolder,
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

        private static void DefaultValue(
            MemberInfo member,
            CustomizersHolder customizerHolder,
            object[] attributes)
        {
            var attribute = attributes.GetAttribute<DefaultValueAttribute>();
            if (attribute != null)
            {
                customizerHolder.Property(member, x =>
                {
                    x.DefaultValue(attribute.Value);
                });
            }
        }

        private static void Template(
            MemberInfo member,
            CustomizersHolder customizerHolder,
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
            CustomizersHolder customizerHolder,
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

        private static void OnDelete(
            MemberInfo member,
            CustomizersHolder customizerHolder,
            object[] attributes)
        {
            var attribute = attributes.GetAttribute<OnDeleteAttribute>();
            if (attribute != null)
            {
                customizerHolder.Property(member, x =>
                {
                    x.OnDelete(attribute.DeleteOption);
                });
            }
        }

        private static void Column(
            MemberInfo member,
            CustomizersHolder customizerHolder,
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
            CustomizersHolder customizerHolder,
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

        private static void PropertyDisplayFormat(
            MemberInfo member,
            CustomizersHolder customizerHolder,
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
            CustomizersHolder customizerHolder,
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
            CustomizersHolder customizerHolder,
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
    }
}