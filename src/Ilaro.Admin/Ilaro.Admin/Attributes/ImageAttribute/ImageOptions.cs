using Ilaro.Admin.Commons.FileUpload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Robert.Admin.Attributes
{
    public class ImageOptions
    {
        public NameCreation NameCreation { get; set; }

        public long MaxFileSize { get; set; }

        public string[] AllowedFileExtensions { get; set; }

        public ImageSettings[] Settings { get; set; }

        public bool IsMultiple { get; set; }
    }
}