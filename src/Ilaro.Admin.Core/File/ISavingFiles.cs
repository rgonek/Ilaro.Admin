using System.IO;

namespace Ilaro.Admin.Core.File
{
    public interface ISavingFiles
    {
        void SaveFile(
            Stream inputFileStream,
            string fileName);

        byte[] GetFileByteArray(Stream inputFileStream);
    }
}