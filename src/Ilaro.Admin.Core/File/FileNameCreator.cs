using System;
using Ilaro.Admin.Core.Extensions;
using Microsoft.AspNetCore.Http;

namespace Ilaro.Admin.Core.File
{
    public class FileNameCreator : IFileNameCreator
    {
        public string GetFileName(Property property, IFormFile file)
        {
            switch (property.FileOptions.NameCreation)
            {
                default:
                case NameCreation.OriginalFileName:
                    return file.FileName;
                case NameCreation.Guid:
                    return "{0}.jpg".Fill(Guid.NewGuid());
                case NameCreation.Timestamp:
                    return "{0}.jpg".Fill(DateTime.Now.ToString("ddMMyyhhmmss"));
                case NameCreation.UserInput:
                    return "test.jpg";
            }
        }
    }
}