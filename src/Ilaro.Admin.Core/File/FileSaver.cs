using System.IO;
using Ilaro.Admin.Core.Extensions;

namespace Ilaro.Admin.Core.File
{
    public class FileSaver : ISavingFiles
    {
        private void CreateDirectory(string path)
        {
            if (Directory.Exists(path))
                return;

            var directoryPath = Path.GetDirectoryName(path);
            if (directoryPath.IsNullOrWhiteSpace())
                return;

            Directory.CreateDirectory(directoryPath);
        }

        public void SaveFile(
            Stream inputFileStream,
            string fileName)
        {
            CreateDirectory(fileName);

            using (var fileStream = System.IO.File.Create(fileName))
            {
                inputFileStream.Seek(0, SeekOrigin.Begin);
                inputFileStream.CopyTo(fileStream);
            }
        }

        public byte[] GetFileByteArray(Stream inputFileStream)
        {
            using (var memory = new MemoryStream())
            {
                inputFileStream.Seek(0, SeekOrigin.Begin);
                inputFileStream.CopyTo(memory);

                return memory.ToArray();
            }
        }
    }
}
