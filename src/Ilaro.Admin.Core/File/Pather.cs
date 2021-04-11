using Ilaro.Admin.Core.Extensions;
using System.IO;
using System.Linq;

namespace Ilaro.Admin.Core.File
{
    public static class Pather
    {
        public static string Combine(params string[] paths)
        {
            return Path.Combine(paths.Where(x => x.HasValue()).Select(x => Normalize(x)).ToArray());
        }

        public static string Join(string path1, string path2)
        {
            return Normalize(path1) + Normalize(path2);
        }

        private static string Normalize(string path)
        {
            return path.Trim('/', '\\').Replace("/", "\\");
        }
    }
}