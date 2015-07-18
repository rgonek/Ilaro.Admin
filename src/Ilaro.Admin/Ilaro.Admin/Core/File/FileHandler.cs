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
                throw new ArgumentNullException("saver");
            if (deleter == null)
                throw new ArgumentNullException("deleter");
            if (resizer == null)
                throw new ArgumentNullException("resizer");
            if (fileNameCreator == null)
                throw new ArgumentNullException("fileNameCreator");
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            _saver = saver;
            _deleter = deleter;
            _resizer = resizer;
            _fileNameCreator = fileNameCreator;
            _configuration = configuration;
        }

        /// <summary>
        /// Upload files to temp location, or save file byte array to property value
        /// </summary>
        public IList<Property> Upload(Entity entity)
        {
            var proccessedProperties = new List<Property>();
            foreach (var property in entity
                .CreateProperties(getForeignCollection: false)
                .Where(x => x.TypeInfo.IsFile))
            {
                if (property.Value.Raw.IsBehavior(DataBehavior.Clear))
                {
                    property.Value.Raw = property.Value.DefaultValue;
                    proccessedProperties.Add(property);
                    continue;
                }
                var file = (HttpPostedFileWrapper)property.Value.Raw;
                if (file == null || file.ContentLength == 0)
                {
                    property.Value.Raw = DataBehavior.Skip;
                    continue;
                }

                if (property.TypeInfo.IsFileStoredInDb)
                {
                    var setting = property.FileOptions.Settings.FirstOrDefault();
                    var fileInputStream = property.TypeInfo.IsImage ?
                        _resizer.Resize(file.InputStream, setting.Width, setting.Height) :
                        file.InputStream;

                    var bytes = _saver.GetFileByteArray(fileInputStream);
                    property.Value.Raw = bytes;
                }
                else
                {
                    var fileName = _fileNameCreator.GetFileName(property, file);
                    property.Value.Raw = fileName;

                    if (property.TypeInfo.IsImage)
                    {
                        foreach (var setting in property.FileOptions.Settings)
                        {
                            var resizedStream = _resizer.Resize(
                                file.InputStream,
                                setting.Width,
                                setting.Height);

                            var subPath =
                                setting.SubPath.TrimEnd('/', '\\') +
                                _configuration.UploadFilesTempFolderSufix;
                            var path = Path.Combine(BasePath, property.FileOptions.Path, subPath, fileName);

                            _saver.SaveFile(resizedStream, path);
                            resizedStream.Dispose();
                        }
                    }
                    else
                    {
                        var subPath =
                            property.FileOptions.Path.TrimEnd('/', '\\') +
                            _configuration.UploadFilesTempFolderSufix;
                        var path = Path.Combine(BasePath, subPath, fileName);

                        _saver.SaveFile(file.InputStream, path);
                    }

                    proccessedProperties.Add(property);
                }

                file.InputStream.Dispose();
            }

            return proccessedProperties;
        }

        /// <summary>
        /// Move uploaded files from temp location, and delete old files.
        /// </summary>
        public void ProcessUploaded(IEnumerable<Property> properties, IDictionary<string, object> existingRecord = null)
        {
            foreach (var property in properties)
            {
                var settings = property.FileOptions.Settings.ToList();
                if (property.TypeInfo.IsImage == false)
                {
                    settings = settings.Take(1).ToList();
                }

                foreach (var setting in settings)
                {
                    DeleteOldFile(property, setting, existingRecord);

                    var fileName = property.Value.AsString;
                    if (fileName.IsNullOrEmpty() == false)
                    {
                        var subPath =
                            setting.SubPath.TrimEnd('/', '\\') +
                            _configuration.UploadFilesTempFolderSufix;
                        var sourcePath = Path.Combine(BasePath, property.FileOptions.Path, subPath, fileName);
                        var targetPath = Path.Combine(BasePath, property.FileOptions.Path, setting.SubPath, fileName);

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
            if (recordDict.ContainsKey(property.ColumnName.Undecorate()))
            {
                var fileName = recordDict[property.ColumnName.Undecorate()].ToStringSafe();
                var path = Path.Combine(BasePath, property.FileOptions.Path, setting.SubPath, fileName);

                _deleter.Delete(path);
            }
        }

        /// <summary>
        /// Delete files uploaded in current request.
        /// </summary>
        public void DeleteUploaded(IEnumerable<Property> properties)
        {
            foreach (var property in properties)
            {
                var settings = property.FileOptions.Settings.ToList();
                if (property.TypeInfo.IsFile)
                {
                    settings = settings.Take(1).ToList();
                }

                foreach (var setting in settings)
                {
                    var fileName = property.Value.AsString;
                    var subPath =
                        setting.SubPath.TrimEnd('/', '\\') +
                        _configuration.UploadFilesTempFolderSufix;
                    var path = Path.Combine(BasePath, property.FileOptions.Path, subPath, fileName);

                    _deleter.Delete(path);
                }
            }
        }

        /// <summary>
        /// Delete files uploaded in current request.
        /// </summary>
        public void Delete(IEnumerable<Property> properties)
        {
            foreach (var property in properties)
            {
                var settings = property.FileOptions.Settings.ToList();
                if (property.TypeInfo.IsFile)
                {
                    settings = settings.Take(1).ToList();
                }

                foreach (var setting in settings)
                {
                    var fileName = property.Value.AsString;
                    var path = Path.Combine(BasePath, property.FileOptions.Path, setting.SubPath, fileName);

                    _deleter.Delete(path);
                }
            }
        }
    }
}