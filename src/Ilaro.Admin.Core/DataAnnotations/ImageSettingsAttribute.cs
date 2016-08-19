using System;
using System.Drawing;

namespace Ilaro.Admin.Core.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ImageSettingsAttribute : Attribute
    {
        public ImageSettings Settings { get; set; }

        public ImageSettingsAttribute(string subPath)
        {
            Settings = new ImageSettings(subPath, null, null);
        }

        public ImageSettingsAttribute(string subPath, int width, int height)
        {
            Settings = new ImageSettings(subPath, width, height);
        }

        public ImageSettingsAttribute(string subPath, Size size)
            : this(subPath, size.Width, size.Height)
        {
        }

        public ImageSettingsAttribute(int width, int height)
            : this(String.Empty, width, height)
        {
        }

        public ImageSettingsAttribute(Size size)
            : this(size.Width, size.Height)
        {
        }
    }
}