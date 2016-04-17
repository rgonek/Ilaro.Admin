using System.IO;

namespace Ilaro.Admin.Core.File
{
    public interface IResizingImages
    {
        Stream Resize(
            Stream sourceStream,
            int? width = null,
            int? height = null);
    }
}