using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Ilaro.Admin.DataAnnotations;
using Ilaro.Admin.Extensions;
using Ilaro.Admin.Core.Data;

namespace Ilaro.Admin.Core.File
{
    public class FileHandler : IHandlingFiles
    {
        private static readonly string BasePath = HttpContext.Current.Server.MapPath("~/");

        private readonly ISavingFiles _saver;
        private readonly IDeletingFiles _deleter;
        private readonly IResizingImages _resizer;
        private readonly ICreatingNameFiles _fileNameCreator;
        private readonly IConfiguration _configuration;

        public FileHandler(
            ISavingFiles saver,
            IDeletingFiles deleter,
            IResizingImages resizer,
            ICreatingNameFiles fileNameCreator,
            IConfiguration configuration)
        {
            if (saver == null)
                throw new ArgumentNullException(nameof(saver));
            if (deleter == null)
                throw new ArgumentNullException(nameof(deleter));
            if (resizer == null)
                throw new ArgumentNullException(nameof(resizer));
            if (fileNameCreator == null)
                throw new ArgumentNullException(nameof(fileNameCreator));
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            _saver = saver;
            _deleter = deleter;
            _resizer = resizer;
            _fileNameCreator = fileNameCreator;
            _configuration = configuration;
        }

        public IEnumerable<PropertyValue> Upload(
            EntityRecord entityRecord,
            Func<Property, object> defaultValueResolver)
        {
            var proccessedProperties = new List<PropertyValue>();
            foreach (var propertyValue in entityRecord.Values
                .Where(value => value.Property.TypeInfo.IsFile))
            {
                if (propertyValue.DataBehavior == DataBehavior.Clear)
                {
                    propertyValue.Raw = defaultValueResolver(propertyValue.Property);
                    proccessedProperties.Add(propertyValue);
                    continue;
                }
                var file = (HttpPostedFileWrapper)propertyValue.Raw;
                if (file == null || file.ContentLength == 0)
                {
                    propertyValue.DataBehavior = DataBehavior.Skip;
                    continue;
                }

                if (propertyValue.Property.TypeInfo.IsFileStoredInDb)
                {
                    var setting = propertyValue.Property.FileOptions.Settings.FirstOrDefault();
                    var fileInputStream = propertyValue.Property.TypeInfo.IsImage ?
                        _resizer.Resize(file.InputStream, setting.Width, setting.Height) :
                        file.InputStream;

                    var bytes = _saver.GetFileByteArray(fileInputStream);
                    propertyValue.Raw = bytes;
                }
                else
                {
                    var fileName = _fileNameCreator.GetFileName(propertyValue.Property, file);
                    propertyValue.Raw = fileName;

                    if (propertyValue.Property.TypeInfo.IsImage)
                    {
                        foreach (var setting in propertyValue.Property.FileOptions.Settings)
                        {
                            var resizedStream = _resizer.Resize(
                                file.InputStream,
                                setting.Width,
                                setting.Height);

                            var subPath =
                                setting.SubPath.TrimEnd('/', '\\') +
                                _configuration.UploadFilesTempFolderSufix;
                            var path = Path.Combine(
                                BasePath,
                                propertyValue.Property.FileOptions.Path,
                                subPath,
                                fileName);

                            _saver.SaveFile(resizedStream, path);
                            resizedStream.Dispose();
                        }
                    }
                    else
                    {
                        var subPath =
                            propertyValue.Property.FileOptions.Path.TrimEnd('/', '\\') +
                            _configuration.UploadFilesTempFolderSufix;
                        var path = Path.Combine(BasePath, subPath, fileName);

                        _saver.SaveFile(file.InputStream, path);
                    }

                    proccessedProperties.Add(propertyValue);
                }

                file.InputStream.Dispose();
            }

            return proccessedProperties;
        }

        public void ProcessUploaded(
            IEnumerable<PropertyValue> propertiesValues,
            IDictionary<string, object> existingRecord = null)
        {
            foreach (var propertyValue in propertiesValues)
            {
                var settings = propertyValue.Property.FileOptions.Settings.ToList();
                if (propertyValue.Property.TypeInfo.IsImage == false)
                {
                    settings = settings.Take(1).ToList();
                }

                foreach (var setting in settings)
                {
                    DeleteOldFile(propertyValue.Property, setting, existingRecord);

                    var fileName = propertyValue.AsString;
                    if (fileName.IsNullOrEmpty() == false)
                    {
                        var subPath =
                            setting.SubPath.TrimEnd('/', '\\') +
                            _configuration.UploadFilesTempFolderSufix;
                        var sourcePath = Path.Combine(BasePath, propertyValue.Property.FileOptions.Path, subPath, fileName);
                        var targetPath = Path.Combine(BasePath, propertyValue.Property.FileOptions.Path, setting.SubPath, fileName);

                        System.IO.File.Move(sourcePath, targetPath);
                    }
                }
            }
        }

        private void DeleteOldFile(
            Property property,
            ImageSettings setting,
            IDictionary<string, object> recordDict)
        {
            if (recordDict.ContainsKey(property.Column.Undecorate()))
            {
                var fileName = recordDict[property.Column.Undecorate()].ToStringSafe();
                var path = Path.Combine(BasePath, property.FileOptions.Path, setting.SubPath, fileName);

                _deleter.Delete(path);
            }
        }

        public void DeleteUploaded(IEnumerable<PropertyValue> propertiesValues)
        {
            foreach (var propertyValue in propertiesValues)
            {
                var settings = propertyValue.Property.FileOptions.Settings.ToList();
                if (propertyValue.Property.TypeInfo.IsFile)
                {
                    settings = settings.Take(1).ToList();
                }

                foreach (var setting in settings)
                {
                    var fileName = propertyValue.AsString;
                    var subPath =
                        setting.SubPath.TrimEnd('/', '\\') +
                        _configuration.UploadFilesTempFolderSufix;
                    var path = Path.Combine(BasePath, propertyValue.Property.FileOptions.Path, subPath, fileName);

                    _deleter.Delete(path);
                }
            }
        }

        public void Delete(IEnumerable<PropertyValue> propertiesValues)
        {
            foreach (var propertyValue in propertiesValues)
            {
                var settings = propertyValue.Property.FileOptions.Settings.ToList();
                if (propertyValue.Property.TypeInfo.IsFile)
                {
                    settings = settings.Take(1).ToList();
                }

                foreach (var setting in settings)
                {
                    var fileName = propertyValue.AsString;
                    var path = Path.Combine(BasePath, propertyValue.Property.FileOptions.Path, setting.SubPath, fileName);

                    _deleter.Delete(path);
                }
            }
        }
    }
}