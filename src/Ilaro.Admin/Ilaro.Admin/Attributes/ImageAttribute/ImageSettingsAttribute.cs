using Ilaro.Admin.FileUpload;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace Ilaro.Admin.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ImageSettingsAttribute : Attribute
    {
        public ImageSettings Settings { get; set; }

        public bool IsMiniature { get; set; }

        public bool IsBig { get; set; }

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