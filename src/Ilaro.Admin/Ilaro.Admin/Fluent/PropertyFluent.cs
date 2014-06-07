using Ilaro.Admin.Attributes;
using Ilaro.Admin.Commons;
using Ilaro.Admin.Commons.FileUpload;
using Ilaro.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Ilaro.Admin.Fluent
{
	/// <summary>
	/// Fluent property configurator
	/// </summary>
	public class PropertyOf<TEntity>
	{
		private Property _property;

		/// <summary>
		/// Initialize configurator
		/// </summary>
		public static PropertyOf<TEntity> Configure<TProperty>(Expression<Func<TEntity, TProperty>> expression)
		{
			var entityName = typeof(TEntity).Name;
			var propertyName = (expression.Body as MemberExpression).Member.Name;

			return new PropertyOf<TEntity>
			{
				_property = AdminInitialise.EntitiesTypes.FirstOrDefault(x => x.Name == entityName)[propertyName]
			};
		}

		/// <summary>
		/// Set SQL column name
		/// </summary>
		public PropertyOf<TEntity> SetColumnName(string columnName)
		{
			_property.ColumnName = columnName;

			return this;
		}

		/// <summary>
		/// Set delete behavior of foreign property on deleting
		/// </summary>
		public PropertyOf<TEntity> OnDelete(DeleteOption deleteOption)
		{
			_property.DeleteOption = deleteOption;

			return this;
		}

		/// <summary>
		/// Mark property as primary key
		/// </summary>
		public PropertyOf<TEntity> IsKey()
		{
			_property.IsKey = true;

			return this;
		}


		/// <summary>
		/// Mark property as primary link key
		/// </summary>
		public PropertyOf<TEntity> IsLinkKey()
		{
			_property.IsLinkKey = true;

			return this;
		}

		/// <summary>
		/// Set property displa name and description
		/// </summary>
		public PropertyOf<TEntity> SetDisplayName(string name, string description = "")
		{
			_property.DisplayName = name;
			_property.Description = description;

			return this;
		}

		/// <summary>
		/// Set display template
		/// </summary>
		public PropertyOf<TEntity> SetDisplayTemplate(string template)
		{
			_property.DisplayTemplateName = template;

			return this;
		}

		/// <summary>
		/// Set editor template
		/// </summary>
		public PropertyOf<TEntity> SetEditorTemplate(string template)
		{
			_property.EditorTemplateName = template;

			return this;
		}

		/// <summary>
		/// Set data type
		/// </summary>
		public PropertyOf<TEntity> SetDataType(Ilaro.Admin.ViewModels.DataType dataType)
		{
			_property.DataType = dataType;

			return this;
		}

		/// <summary>
		/// Set image options
		/// </summary>
		public PropertyOf<TEntity> SetImageOptions(NameCreation nameCreation, long maxFileSize, bool isMultiple, params string[] allowedFileExtensions)
		{
			_property.ImageOptions = new Attributes.ImageOptions
			{
				NameCreation = nameCreation,
				MaxFileSize = maxFileSize,
				IsMultiple = isMultiple,
				AllowedFileExtensions = allowedFileExtensions,
				Settings = new List<ImageSettings>()
			};
			return this;
		}

		/// <summary>
		/// Set image options
		/// </summary>
		public PropertyOf<TEntity> SetImageSettings(string path, int? width, int? height, bool isMiniature, bool isBig)
		{
			if (_property.ImageOptions == null)
			{
				_property.ImageOptions = new ImageOptions
				{
					AllowedFileExtensions = Consts.AllowedFileExtensions,
					MaxFileSize = Consts.MaxFileSize,
					NameCreation = NameCreation.OriginalFileName,
					Settings = new List<ImageSettings>()
				};
			}
			_property.ImageOptions.Settings.Add(new ImageSettings
			{
				SubPath = path,
				Width = width,
				Height = height,
				IsMiniature = isMiniature,
				IsBig = isBig
			});

			return this;
		}
	}
}