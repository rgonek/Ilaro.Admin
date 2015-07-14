using System.Collections.Generic;
using System.Linq;

namespace Ilaro.Admin.DataAnnotations
{
    public class FileOptions
    {
        public NameCreation NameCreation { get; internal set; }
        public long? MaxFileSize { get; internal set; }
        public string[] AllowedFileExtensions { get; internal set; }
        public string Path { get; internal set; }
        public bool IsImage { get; internal set; }
        public IList<ImageSettings> Settings { get; internal set; }

        public FileOptions()
        {
        }

        public FileOptions(object[] attributes)
        {
            var fileAttribute =
                attributes.OfType<FileAttribute>().FirstOrDefault();
            var imageSettingsAttributes =
                attributes.OfType<ImageSettingsAttribute>().ToList();

            if (fileAttribute != null)
            {
                AllowedFileExtensions = fileAttribute.AllowedFileExtensions;
                MaxFileSize = fileAttribute.MaxFileSize;
                NameCreation = fileAttribute.NameCreation;
                Path = fileAttribute.Path;
                IsImage = fileAttribute.IsImage;
                Settings = new List<ImageSettings>();
            }
            else
            {
                NameCreation = NameCreation.Guid;
                Settings = new List<ImageSettings>();
            }

            if (imageSettingsAttributes.Any())
            {
                Settings = imageSettingsAttributes.Select(x => x.Settings).ToList();
            }
            else
            {
                Settings.Add(new ImageSettings());
            }
        }
    }
}