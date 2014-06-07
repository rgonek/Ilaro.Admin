using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ilaro.Admin.FileUpload
{
    public enum FileUploadValidationResult
    {
        EmptyFile,
        TooBigFile,
        WrongExtension,
        NotImage,
        Valid
    }
}
