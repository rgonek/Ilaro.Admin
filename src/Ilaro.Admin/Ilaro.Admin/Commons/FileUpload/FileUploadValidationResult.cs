using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ilaro.Admin.Commons.FileUpload
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
