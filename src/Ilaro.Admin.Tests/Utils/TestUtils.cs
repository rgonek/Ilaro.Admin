using System;
using System.IO;
using System.Reflection;

namespace Ilaro.Admin.Tests.Utils
{
    public static class TestUtils
    {
        public static string GetDatabaseScript(string fileName)
        {
            var currentDirectory = Path.GetDirectoryName(
                new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath);

            return Path.Combine(currentDirectory, "Database", fileName);
        }
    }
}
