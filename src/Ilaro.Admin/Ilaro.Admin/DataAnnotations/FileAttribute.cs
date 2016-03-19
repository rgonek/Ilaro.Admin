using System;

namespace Ilaro.Admin.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FileAttribute : Attribute
    {
        public NameCreation NameCreation { get; set; }
        public long MaxFileSize { get; set; }
        public string[] AllowedFileExtensions { get; set; }
        public string Path { get; set; }
        public bool IsImage { get; set; }

        public FileAttribute()
        {
            NameCreation = NameCreation.Guid;
        }

        public FileAttribute(params string[] allowedFileExtensions)
            : this()
        {
            AllowedFileExtensions = allowedFileExtensions;
        }
    }
}