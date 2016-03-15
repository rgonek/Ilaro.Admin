using System.Collections.Generic;
using Ilaro.Admin.Core;
using Ilaro.Admin.DataAnnotations;

namespace Ilaro.Admin.Configuration
{
    /// <summary>
    /// Fluent property configurator
    /// </summary>
    public class PropertyOf<TEntity> where TEntity : class
    {
        private Property _property;

        public PropertyOf<TEntity> SetFileOptions(
            NameCreation nameCreation,
            long maxFileSize,
            bool isImage,
            string path,
            params string[] allowedFileExtensions)
        {
            _property.TypeInfo.DataType = DataType.File;

            _property.FileOptions = new FileOptions
            {
                NameCreation = nameCreation,
                MaxFileSize = maxFileSize,
                AllowedFileExtensions = allowedFileExtensions,
                IsImage = isImage,
                Path = path,
                Settings = new List<ImageSettings>()
            };
            return this;
        }

        public PropertyOf<TEntity> SetImageSettings(
            string path,
            int? width,
            int? height)
        {
            _property.TypeInfo.DataType = DataType.File;

            if (_property.FileOptions == null)
            {
                _property.FileOptions = new FileOptions
                {
                    NameCreation = NameCreation.OriginalFileName,
                    Settings = new List<ImageSettings>()
                };
            }
            _property.FileOptions.IsImage = true;
            _property.TypeInfo.DataType = DataType.Image;

            _property.FileOptions.Settings.Add(new ImageSettings
            {
                SubPath = path,
                Width = width,
                Height = height
            });

            return this;
        }
    }
}