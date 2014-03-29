using Ilaro.Admin.Attributes;
using Ilaro.Admin.Commons;
using Ilaro.Admin.Commons.FileUpload;
using Robert.Admin.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using Ilaro.Admin.Extensions;

namespace Ilaro.Admin.ViewModels
{
    public class PropertyViewModel
    {
        public EntityViewModel Entity { get; set; }

        public Type PropertyType { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string GroupName { get; set; }

        public string Description { get; set; }

        public bool IsKey { get; set; }

        public bool IsLinkKey { get; set; }

        public bool IsForeignKey { get; set; }

        public bool IsRequired { get; set; }
        public string RequiredErrorMessage { get; set; }

        public DataType DataType { get; set; }

        public Type EnumType { get; set; }

        public ImageOptions ImageOptions { get; set; }

        public string EditorTemplateName { get; set; }
        public string DisplayTemplateName { get; set; }

        public object Value { get; set; }

        // thats lame, should be in extension method
        public bool? BoolValue
        {
            get
            {
                if (Value == null)
                {
                    return null;
                }

                if (Value is bool || Value is bool?)
                {
                    return (bool?)Value;
                }
                else if (Value is string)
                {
                    return bool.Parse(Value.ToString());
                }

                return null;
            }
        }

        public string StringValue
        {
            get
            {
                if (Value == null)
                {
                    return String.Empty;
                }

                return Value.ToString();
            }
        }
        public object ObjectValue
        {
            get
            {
                if (DataType == ViewModels.DataType.Enum)
                {
                    return Convert.ChangeType(Value, EnumType);
                }

                return Convert.ChangeType(Value, PropertyType);
            }
        }

        public PropertyViewModel(EntityViewModel entity, PropertyInfo property)
        {
            Entity = entity;
            Name = property.Name;

            PropertyType = property.PropertyType;

            IsForeignKey = !(PropertyType.IsPrimitive || PropertyType.IsEnum || PropertyType.Equals(typeof(string)) || PropertyType.Equals(typeof(DateTime)) || PropertyType.Equals(typeof(DateTime?)));

            var attributes = property.GetCustomAttributes(false);
            SetDataType(attributes);

            IsKey = attributes.OfType<KeyAttribute>().Any();
            IsLinkKey = attributes.OfType<LinkKeyAttribute>().Any();

            var requiredAttribute = attributes.OfType<RequiredAttribute>().FirstOrDefault();
            if (requiredAttribute != null)
            {
                IsRequired = true;
                RequiredErrorMessage = requiredAttribute.ErrorMessage;
            }

            var displayAttribute = attributes.OfType<DisplayAttribute>().FirstOrDefault();
            if (displayAttribute != null)
            {
                DisplayName = displayAttribute.Name ?? Name.SplitCamelCase();
                Description = displayAttribute.Description;
                GroupName = displayAttribute.GroupName ?? "Others";
            }
            else
            {
                DisplayName = Name.SplitCamelCase();
                GroupName = "Others";
            }

            SetTemplatesName(attributes);
        }

        private void SetDataType(object[] attributes)
        {
            var enumDataTypeAttribute = attributes.OfType<EnumDataTypeAttribute>().FirstOrDefault();

            if (enumDataTypeAttribute != null)
            {
                DataType = ViewModels.DataType.Enum;
                EnumType = enumDataTypeAttribute.EnumType;
            }
            else if (PropertyType.IsEnum)
            {
                DataType = ViewModels.DataType.Enum;
                EnumType = PropertyType;
            }
            else if (PropertyType.In(typeof(int), typeof(short), typeof(long), typeof(double), typeof(decimal), typeof(float), typeof(int?), typeof(short?), typeof(long?), typeof(double?), typeof(decimal?), typeof(float?)))
            {
                DataType = ViewModels.DataType.Numeric;
            }
            else if (PropertyType.In(typeof(DateTime), typeof(DateTime?)))
            {
                DataType = ViewModels.DataType.DateTime;
            }
            else if (PropertyType.In(typeof(bool), typeof(bool?)))
            {
                DataType = ViewModels.DataType.Bool;
            }
            else if (PropertyType == typeof(byte[]))
            {
                DataType = ViewModels.DataType.File;
            }
            else
            {
                DataType = ViewModels.DataType.String;
            }

            var dataTypeAttribute = attributes.OfType<DataTypeAttribute>().FirstOrDefault();
            if (dataTypeAttribute != null && dataTypeAttribute.DataType == System.ComponentModel.DataAnnotations.DataType.ImageUrl)
            {
                DataType = ViewModels.DataType.File;
            }

            var imageAttribute = attributes.OfType<ImageAttribute>().FirstOrDefault();
            var imageSettingsAttributes = attributes.OfType<ImageSettingsAttribute>().ToList();
            if (imageAttribute != null || imageSettingsAttributes.Any() || DataType == ViewModels.DataType.File)
            {
                DataType = ViewModels.DataType.File;

                if (imageAttribute != null)
                {
                    ImageOptions = new ImageOptions
                    {
                        AllowedFileExtensions = imageAttribute.AllowedFileExtensions,
                        MaxFileSize = imageAttribute.MaxFileSize,
                        NameCreation = imageAttribute.NameCreation,
                        IsMultiple = imageAttribute.IsMulti
                    };
                }
                else
                {
                    ImageOptions = new ImageOptions
                    {
                        AllowedFileExtensions = Consts.AllowedFileExtensions,
                        MaxFileSize = Consts.MaxFileSize,
                        NameCreation = NameCreation.OriginalFileName
                    };
                }

                if (imageSettingsAttributes.Any())
                {
                    var length = imageSettingsAttributes.Count;
                    ImageOptions.Settings = new ImageSettings[length];

                    for (int i = 0; i < length; i++)
                    {
                        var settings = imageSettingsAttributes[i].Settings;
                        settings.IsBig = imageSettingsAttributes[i].IsBig;
                        settings.IsMiniature = imageSettingsAttributes[i].IsMiniature;
                        ImageOptions.Settings[i] = settings;
                    }
                }
                else
                {
                    ImageOptions.Settings = new ImageSettings[] { new ImageSettings("Content/" + Entity.Name) };
                }
            }
        }

        private void SetTemplatesName(object[] attributes)
        {
            var dataTypeAttribute = attributes.OfType<DataTypeAttribute>().FirstOrDefault();
            if (dataTypeAttribute != null)
            {
                switch (dataTypeAttribute.DataType)
                {
                    case System.ComponentModel.DataAnnotations.DataType.Date:
                        EditorTemplateName = DisplayTemplateName = "DatePartial";
                        break;
                    case System.ComponentModel.DataAnnotations.DataType.DateTime:
                        EditorTemplateName = DisplayTemplateName = "DateTimePartial";
                        break;
                    case System.ComponentModel.DataAnnotations.DataType.Text:
                        EditorTemplateName = "TextBoxPartial";
                        DisplayTemplateName = "TextPartial";
                        break;
                    case System.ComponentModel.DataAnnotations.DataType.MultilineText:
                        EditorTemplateName = "TextAreaPartial";
                        DisplayTemplateName = "TextPartial";
                        break;
                    case System.ComponentModel.DataAnnotations.DataType.Html:
                        EditorTemplateName = DisplayTemplateName = "HtmlPartial";
                        break;
                    case System.ComponentModel.DataAnnotations.DataType.ImageUrl:
                        EditorTemplateName = "FilePartial";
                        DisplayTemplateName = "ImagePartial";
                        break;
                }
            }

            if (DisplayTemplateName.IsNullOrEmpty())
            {
                switch (DataType)
                {
                    case ViewModels.DataType.Enum:
                        EditorTemplateName = "DropDownListPartial";
                        DisplayTemplateName = "EnumPartial";
                        break;
                    case ViewModels.DataType.DateTime:
                        EditorTemplateName = DisplayTemplateName = "DateTimePartial";
                        break;
                    case ViewModels.DataType.Bool:
                        EditorTemplateName = "CheckBoxPartial";
                        DisplayTemplateName = "BoolPartial";
                        break;
                    case ViewModels.DataType.File:
                        EditorTemplateName = "FilePartial";
                        DisplayTemplateName = "ImagePartial";
                        break;
                    case ViewModels.DataType.Numeric:
                        EditorTemplateName = DisplayTemplateName = "NumericPartial";
                        break;
                    default:
                    case ViewModels.DataType.String:
                        EditorTemplateName = "TextBoxPartial";
                        DisplayTemplateName = "TextPartial";
                        break;
                }
            }

            var uiHintAttribute = attributes.OfType<UIHintAttribute>().FirstOrDefault();
            if (uiHintAttribute != null)
            {
                EditorTemplateName = uiHintAttribute.UIHint;
            }
        }

        public SelectList GetPossibleValues()
        {
            var options = EnumType.GetOptions(String.Empty, "--Wybierz--");

            if (Value != null && Value.GetType().IsEnum)
            {
                return new SelectList(options, "Key", "Value", Convert.ToInt32(Value));
            }

            return new SelectList(options, "Key", "Value", Value);
        }
    }
}
