using Ilaro.Admin.Attributes;
using Ilaro.Admin.Commons;
using Ilaro.Admin.Commons.FileUpload;
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
using Resources;
using System.Globalization;

namespace Ilaro.Admin.ViewModels
{
	[DebuggerDisplay("Property {Name}")]
	public class Property
	{
		#region Fields

		private IList<Type> realNumbersTypes = new List<Type>
        {
            typeof(sbyte), typeof(sbyte?),
            typeof(byte), typeof(byte?),
            typeof(short), typeof(short?),
            typeof(ushort), typeof(ushort?),
            typeof(int), typeof(int?),
            typeof(uint), typeof(uint?),
            typeof(long), typeof(long?),
            typeof(ulong), typeof(ulong?)
        };

		private IList<Type> floatingPointNumbersTypes = new List<Type>
        {
            typeof(float), typeof(float?),
            typeof(double), typeof(double?),
            typeof(decimal), typeof(decimal?)
        };

		#endregion

		public Entity Entity { get; set; }

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

		public string ColumnName { get; set; }

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

		public Entity ForeignEntity { get; set; }

		public string ForeignEntityName { get; set; }

		public Property ReferenceProperty { get; set; }

		public string ReferencePropertyName { get; set; }

		public bool IsRequired { get; set; }
		public string RequiredErrorMessage { get; set; }

		public DataType DataType { get; set; }

		public DeleteOption DeleteOption { get; set; }

		public Type EnumType { get; set; }

		public ImageOptions ImageOptions { get; set; }

		public string EditorTemplateName { get; set; }
		public string DisplayTemplateName { get; set; }

		public object Value { get; set; }

		public IList<object> Values { get; set; }

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

				if (DataType == DataType.Numeric && floatingPointNumbersTypes.Contains(PropertyType))
				{
					try
					{
						return Convert.ToDecimal(Value).ToString(CultureInfo.InvariantCulture);
					}
					catch { }
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

		/// <summary>
		/// Possible values for foreign entity
		/// </summary>
		public IDictionary<string, string> PossibleValues { get; set; }

		public object[] Attributes { get; set; }

		public IList<ValidationAttribute> ValidationAttributes { get; set; }

		internal System.ComponentModel.DataAnnotations.DataType? SourceDataType { get; set; }

		public IDictionary<string, object> ControlsAttributes { get; set; }

		public Property(Entity entity, PropertyInfo property)
		{
			ControlsAttributes = new Dictionary<string, object>();
			Entity = entity;
			// TODO: determine ColumnName
			ColumnName = Name = property.Name;

			PropertyType = property.PropertyType;
			DeterminePropertyInfo();

			Attributes = property.GetCustomAttributes(false);
			ValidationAttributes = Attributes.OfType<ValidationAttribute>().ToList();

			SetForeignKey(Attributes);

			SetDataType(Attributes);

			SetDeleteOption(Attributes);

			IsKey = Attributes.OfType<KeyAttribute>().Any();
			IsLinkKey = Attributes.OfType<LinkKeyAttribute>().Any();

			var columnAttribute = Attributes.OfType<ColumnAttribute>().FirstOrDefault();
			if (columnAttribute != null)
			{
				ColumnName = columnAttribute.Name;
			}

			var requiredAttribute = Attributes.OfType<RequiredAttribute>().FirstOrDefault();
			if (requiredAttribute != null)
			{
				IsRequired = true;
				RequiredErrorMessage = requiredAttribute.ErrorMessage;
			}

			var displayAttribute = Attributes.OfType<DisplayAttribute>().FirstOrDefault();
			if (displayAttribute != null)
			{
				DisplayName = displayAttribute.Name ?? Name.SplitCamelCase();
				Description = displayAttribute.Description;
				GroupName = displayAttribute.GroupName ?? Resources.IlaroAdminResources.Others;
			}
			else
			{
				DisplayName = Name.SplitCamelCase();
				GroupName = Resources.IlaroAdminResources.Others;
			}
		}

		private void SetDeleteOption(object[] attributes)
		{
			var onDeleteAttribute = attributes.OfType<OnDeleteAttribute>().FirstOrDefault();
			if (onDeleteAttribute != null)
			{
				DeleteOption = onDeleteAttribute.DeleteOption;
			}
			else
			{
				DeleteOption = DeleteOption.Nothing;
			}
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
					ReferencePropertyName = ColumnName = foreignKeyAttribute.Name;
					ForeignEntityName = PropertyType.Name;
				}
			}
			else
			{
				if (IsSystemType || PropertyType.IsEnum)
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
			var dataTypeAttribute = attributes.OfType<DataTypeAttribute>().FirstOrDefault();
			if (dataTypeAttribute != null)
			{
				SourceDataType = dataTypeAttribute.DataType;
				DataType = ConvertDataType(dataTypeAttribute.DataType);

				return;
			}

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
			else if (realNumbersTypes.Contains(PropertyType) || floatingPointNumbersTypes.Contains(PropertyType))
			{
				DataType = ViewModels.DataType.Numeric;
				if (floatingPointNumbersTypes.Contains(PropertyType))
				{
					ControlsAttributes.Add("data-number-number-of-decimals", "4");
				}
				// if its nullable
				if (Nullable.GetUnderlyingType(PropertyType) != null)
				{
					ControlsAttributes.Add("data-number-value", "");
				}
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
				DataType = ViewModels.DataType.Text;
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
						IsMultiple = imageAttribute.IsMulti,
						Settings = new List<ImageSettings>()
					};
				}
				else
				{
					ImageOptions = new ImageOptions
					{
						AllowedFileExtensions = Consts.AllowedFileExtensions,
						MaxFileSize = Consts.MaxFileSize,
						NameCreation = NameCreation.OriginalFileName,
						Settings = new List<ImageSettings>()
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

		private DataType ConvertDataType(System.ComponentModel.DataAnnotations.DataType sourceDataType)
		{
			switch (sourceDataType)
			{
				case System.ComponentModel.DataAnnotations.DataType.DateTime:
				case System.ComponentModel.DataAnnotations.DataType.Date:
				case System.ComponentModel.DataAnnotations.DataType.Time:
				case System.ComponentModel.DataAnnotations.DataType.Duration:
					return DataType.DateTime;
				case System.ComponentModel.DataAnnotations.DataType.Url:
				case System.ComponentModel.DataAnnotations.DataType.ImageUrl:
				case System.ComponentModel.DataAnnotations.DataType.Upload:
					return DataType.File;
				case System.ComponentModel.DataAnnotations.DataType.Currency:
					return DataType.Numeric;
				case System.ComponentModel.DataAnnotations.DataType.Password:
					return DataType.Password;
				default:
				case System.ComponentModel.DataAnnotations.DataType.Custom:
				case System.ComponentModel.DataAnnotations.DataType.PhoneNumber:
				case System.ComponentModel.DataAnnotations.DataType.Text:
				case System.ComponentModel.DataAnnotations.DataType.Html:
				case System.ComponentModel.DataAnnotations.DataType.MultilineText:
				case System.ComponentModel.DataAnnotations.DataType.EmailAddress:
				case System.ComponentModel.DataAnnotations.DataType.CreditCard:
				case System.ComponentModel.DataAnnotations.DataType.PostalCode:
					return DataType.Text;
			}
		}

		public void SetTemplatesName()
		{
			var uiHintAttribute = Attributes.FirstOrDefault(x => x.GetType() == typeof(UIHintAttribute)) as UIHintAttribute;
			var templateAttribute = Attributes.FirstOrDefault(x => x.GetType() == typeof(TemplateAttribute)) as TemplateAttribute;
			if (uiHintAttribute != null || templateAttribute != null)
			{
				if (uiHintAttribute != null)
				{
					EditorTemplateName = DisplayTemplateName = uiHintAttribute.UIHint;
				}
				if (templateAttribute != null)
				{
					if (!templateAttribute.DisplayTemplate.IsNullOrEmpty())
					{
						DisplayTemplateName = templateAttribute.DisplayTemplate;
					}
					if (!templateAttribute.EditorTemplate.IsNullOrEmpty())
					{
						EditorTemplateName = templateAttribute.EditorTemplate;
					}
				}
			}
			else
			{
				if (IsForeignKey)
				{
					if (IsCollection)
					{
						EditorTemplateName = Templates.Editor.DualList;
					}
					else
					{
						EditorTemplateName = Templates.Editor.DropDownList;
					}
					DisplayTemplateName = Templates.Display.Text;
				}
				else if (SourceDataType != null)
				{
					switch (SourceDataType)
					{
						case System.ComponentModel.DataAnnotations.DataType.DateTime:
							EditorTemplateName = Templates.Editor.DateTime;
							DisplayTemplateName = Templates.Display.DateTime;
							break;
						case System.ComponentModel.DataAnnotations.DataType.Date:
							EditorTemplateName = Templates.Editor.Date;
							DisplayTemplateName = Templates.Display.Date;
							break;
						case System.ComponentModel.DataAnnotations.DataType.Time:
							EditorTemplateName = Templates.Editor.Time;
							DisplayTemplateName = Templates.Display.Time;
							break;
						// TODO: for consideration
						//case System.ComponentModel.DataAnnotations.DataType.Duration:
						//break;
						case System.ComponentModel.DataAnnotations.DataType.Url:
						case System.ComponentModel.DataAnnotations.DataType.Upload:
							EditorTemplateName = Templates.Editor.File;
							DisplayTemplateName = Templates.Display.Link;
							break;
						case System.ComponentModel.DataAnnotations.DataType.ImageUrl:
							EditorTemplateName = Templates.Editor.File;
							DisplayTemplateName = Templates.Display.Image;
							break;
						case System.ComponentModel.DataAnnotations.DataType.Currency:
							EditorTemplateName = Templates.Editor.Numeric;
							DisplayTemplateName = Templates.Display.Numeric;
							break;
						case System.ComponentModel.DataAnnotations.DataType.Password:
							EditorTemplateName = Templates.Editor.Password;
							DisplayTemplateName = Templates.Display.Hash;
							break;
						case System.ComponentModel.DataAnnotations.DataType.Html:
							EditorTemplateName = Templates.Editor.Html;
							DisplayTemplateName = Templates.Display.Html;
							break;
						case System.ComponentModel.DataAnnotations.DataType.MultilineText:
							EditorTemplateName = Templates.Editor.TextArea;
							DisplayTemplateName = Templates.Display.Text;
							break;
						case System.ComponentModel.DataAnnotations.DataType.PhoneNumber:
						case System.ComponentModel.DataAnnotations.DataType.Text:
						case System.ComponentModel.DataAnnotations.DataType.EmailAddress:
						case System.ComponentModel.DataAnnotations.DataType.CreditCard:
						case System.ComponentModel.DataAnnotations.DataType.PostalCode:
							EditorTemplateName = Templates.Editor.TextBox;
							DisplayTemplateName = Templates.Display.Text;
							break;
					}
				}

				if (DisplayTemplateName.IsNullOrEmpty())
				{
					switch (DataType)
					{
						case ViewModels.DataType.Enum:
							EditorTemplateName = Templates.Editor.DropDownList;
							DisplayTemplateName = Templates.Display.Text;
							break;
						case ViewModels.DataType.DateTime:
							EditorTemplateName = Templates.Editor.DateTime;
							DisplayTemplateName = Templates.Display.DateTime;
							break;
						case ViewModels.DataType.Bool:
							EditorTemplateName = Templates.Editor.CheckBox;
							DisplayTemplateName = Templates.Display.Bool;
							break;
						case ViewModels.DataType.File:
							EditorTemplateName = Templates.Editor.File;
							DisplayTemplateName = Templates.Display.Image;
							break;
						case ViewModels.DataType.Numeric:
							EditorTemplateName = Templates.Editor.Numeric;
							DisplayTemplateName = Templates.Display.Numeric;
							break;
						default:
						case ViewModels.DataType.Text:
							EditorTemplateName = Templates.Editor.TextBox;
							DisplayTemplateName = Templates.Display.Text;
							break;
					}
				}
			}
		}

		public MultiSelectList GetPossibleValues(bool addChooseItem = true)
		{
			if (IsForeignKey)
			{
				var options = new Dictionary<string, string>();

				if (addChooseItem)
				{
					options.Add(String.Empty, IlaroAdminResources.Choose);
				}
				options = options.Union(PossibleValues).ToDictionary(x => x.Key, x => x.Value);

				if (IsCollection)
				{
					return new MultiSelectList(options, "Key", "Value", Values);
				}
				else
				{
					return new SelectList(options, "Key", "Value", Value);
				}
			}
			else
			{
				var options = addChooseItem ? EnumType.GetOptions(String.Empty, IlaroAdminResources.Choose) : EnumType.GetOptions();

				if (Value != null && Value.GetType().IsEnum)
				{
					return new SelectList(options, "Key", "Value", Convert.ToInt32(Value));
				}

				return new SelectList(options, "Key", "Value", Value);
			}
		}
	}
}
