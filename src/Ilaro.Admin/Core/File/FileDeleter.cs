using System;

namespace Ilaro.Admin.Core.File
{
    public class FileDeleter : IDeletingFiles
    {
        private static readonly IInternalLogger _log = LoggerProvider.LoggerFor(typeof(FileDeleter));

        public void Delete(string path)
        {
            if (System.IO.File.Exists(path))
            {
                try
                {
                    System.IO.File.Delete(path);
                }
                catch (Exception ex)
                {
                    _log.Error(ex);
                }
            }
        }
    }
}