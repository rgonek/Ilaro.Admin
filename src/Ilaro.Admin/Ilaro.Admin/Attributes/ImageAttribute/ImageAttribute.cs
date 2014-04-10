using Ilaro.Admin.Commons;
using Ilaro.Admin.Commons.FileUpload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ilaro.Admin.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ImageAttribute : Attribute
    {
        /// <summary>
        /// Default is OriginalFileName
        /// </summary>
        public NameCreation NameCreation { get; set; }

        /// <summary>
        /// Default is: 2048000 = 2MB
        /// </summary>
        public long MaxFileSize { get; set; }

        /// <summary>
        /// Default is: .jpg, .jpeg, .png, .gif, .bmp
        /// </summary>
        public string[] AllowedFileExtensions { get; set; }

        public bool IsMulti { get; set; }

        public ImageAttribute()
        {
            MaxFileSize = Consts.MaxFileSize;
            AllowedFileExtensions = Consts.AllowedFileExtensions;
            NameCreation = NameCreation.OriginalFileName;
        }

        public ImageAttribute(params string[] allowedFileExtensions)
            : this()
        {
            AllowedFileExtensions = allowedFileExtensions;
        }
    }
}