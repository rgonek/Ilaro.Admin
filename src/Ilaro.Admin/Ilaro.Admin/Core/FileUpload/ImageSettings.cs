using System;
using System.Drawing;

namespace Ilaro.Admin.Core.FileUpload
{
    public class ImageSettings
    {
        public string SubPath { get; set; }

        public int? Width { get; set; }

        public int? Height { get; set; }

        public bool IsMiniature { get; set; }

        public bool IsBig { get; set; }

        public ImageSettings(
            string subPath,
            int? width = null,
            int? height = null)
        {
            SubPath = subPath;
            Width = width;
            Height = height;
        }

        public ImageSettings(string subPath, Size size)
            : this(subPath, size.Width, size.Height)
        {
        }

        public ImageSettings(int? width = null, int? height = null)
            : this(String.Empty, width, height)
        {
        }

        public ImageSettings(Size size)
            : this(size.Width, size.Height)
        {
        }
    }
}
