using Ilaro.Admin.Attributes;
using Ilaro.Admin.Commons;
using Ilaro.Admin.Commons.FileUpload;
using Robert.Admin.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using Ilaro.Admin.Extensions;
using System.Diagnostics;
using System.ComponentModel;

namespace Ilaro.Admin.ViewModels
{
	[DebuggerDisplay("Property {Name}")]
	public class PropertyViewModel
	{
		public EntityViewModel Entity { get; set; }

		public Type PropertyType { get; set; }

		/// <summary>
		/// Is a property type (or sub type if property is collection) 
		/// is a system type (namespace starts with "System") or not.
		/// </summary>
		public bool IsSystemType { get; set; }

		/// <summary>
		/// Is property is a collection and has sub type.
		/// </summary>
		public bool IsCollection { get; set; }

		public string Name { get; set; }

		public string DisplayName { get; set; }

		public string GroupName { get; set; }

		public string Description { get; set; }

		public string Prompt { get; set; }

		/// <summary>
		/// Is property is a entity key.
		/// </summary>
		public bool IsKey { get; set; }

		/// <summary>
		/// Is property is a link key.
		/// If you provide custom links to view a entity in your app, that property is used to identify a entity.
		/// For example you have Product with slug, so slug is used to display product not id.
		/// </summary>
		public bool IsLinkKey { get; set; }

		/// <summary>
		/// Is property is a foreign key.
		/// </summary>
		public bool IsForeignKey { get; set; }

		public EntityViewModel ForeignEntity { get; set; }

		public string ForeignEntityName { get; set; }

		public PropertyViewModel ReferenceProperty { get; set; }

		public string ReferencePropertyName { get; set; }

		public bool IsRequired { get; set; }
		public string RequiredErrorMessage { get; set; }

		public DataType DataType { get; set; }

		public Type EnumType { get; set; }

		public ImageOptions ImageOptions { get; set; }

		public string EditorTemplateName { get; set; }
		public string DisplayTemplateName { get; set; }

		[Required]
		[StringLength(20)]
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

				return Value.ToStringSafe();
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

		public IList<ValidationAttribute> ValidationAttributes { get; set; }

		public PropertyViewModel(EntityViewModel entity, PropertyInfo property)
		{
			Entity = entity;
			Name = property.Name;

			PropertyType = property.PropertyType;
			DeterminePropertyInfo();

			var attributes = property.GetCustomAttributes(false);
			ValidationAttributes = attributes.OfType<ValidationAttribute>().ToList();

			SetForeignKey(attributes);

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

		private void DeterminePropertyInfo()
		{
			IsSystemType = PropertyType.Namespace.StartsWith("System");
			// for example for string PropertyType.GetInterface("IEnumerable`1") is not null, so we must check if type has sub type 
			IsCollection = PropertyType.GetInterface("IEnumerable`1") != null && PropertyType.GetGenericArguments().Any();

			if (IsCollection)
			{
				var subType = PropertyType.GetGenericArguments().Single();
				IsSystemType = subType.Namespace.StartsWith("System");
				PropertyType = subType;
			}
		}

		private void SetForeignKey(object[] attributes)
		{
			// move to other class, thanks that I can make a nice tests for this

			var foreignKeyAttribute = attributes.OfType<ForeignKeyAttribute>().FirstOrDefault();
			if (foreignKeyAttribute != null)
			{
				IsForeignKey = true;

				if (IsSystemType)
				{
					ForeignEntityName = foreignKeyAttribute.Name;
				}
				else
				{
					ReferencePropertyName = foreignKeyAttribute.Name;
					ForeignEntityName = PropertyType.Name;
				}
			}
			else
			{
				if (IsSystemType)
				{
					IsForeignKey = false;
				}
				else
				{
					IsForeignKey = true;
					ForeignEntityName = PropertyType.Name;
				}
			}
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
			// TODO: localize string
			var options = EnumType.GetOptions(String.Empty, "--Choose--");

			if (Value != null && Value.GetType().IsEnum)
			{
				return new SelectList(options, "Key", "Value", Convert.ToInt32(Value));
			}

			return new SelectList(options, "Key", "Value", Value);
		}
	}
}
