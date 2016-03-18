using System.Collections.Generic;

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
    }
}