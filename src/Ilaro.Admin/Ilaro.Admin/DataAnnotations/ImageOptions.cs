using System.Collections.Generic;
using System.Linq;
using Ilaro.Admin.Core.File;

namespace Ilaro.Admin.DataAnnotations
{
    public class ImageOptions
    {
        public NameCreation NameCreation { get; internal set; }
        public long? MaxFileSize { get; internal set; }
        public string[] AllowedFileExtensions { get; internal set; }
        public IList<ImageSettings> Settings { get; internal set; }
        public bool IsMultiple { get; internal set; }

        public ImageOptions()
        {
        }

        public ImageOptions(object[] attributes, string entityName)
        {
            var imageAttribute =
                attributes.OfType<ImageAttribute>().FirstOrDefault();
            var imageSettingsAttributes =
                attributes.OfType<ImageSettingsAttribute>().ToList();

            if (imageAttribute != null)
            {
                AllowedFileExtensions = imageAttribute.AllowedFileExtensions;
                MaxFileSize = imageAttribute.MaxFileSize;
                NameCreation = imageAttribute.NameCreation;
                IsMultiple = imageAttribute.IsMulti;
                Settings = new List<ImageSettings>();
            }
            else
            {
                NameCreation = NameCreation.OriginalFileName;
                Settings = new List<ImageSettings>();
            }

            if (imageSettingsAttributes.Any())
            {
                var length = imageSettingsAttributes.Count;
                Settings = new ImageSettings[length];

                for (var i = 0; i < length; i++)
                {
                    var settings = imageSettingsAttributes[i].Settings;
                    settings.IsBig = imageSettingsAttributes[i].IsBig;
                    settings.IsMiniature = imageSettingsAttributes[i].IsMiniature;
                    Settings[i] = settings;
                }
            }
            else
            {
                Settings = new[]
                {
                    new ImageSettings("Content/" + entityName)
                };
            }
        }
    }
}