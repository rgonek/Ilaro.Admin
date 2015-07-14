using System;
using System.Web;
using Ilaro.Admin.DataAnnotations;
using Ilaro.Admin.Extensions;

namespace Ilaro.Admin.Core.File
{
    public class FileNameCreator : ICreatingNameFiles
    {
        public string GetFileName(Property property, HttpPostedFile file)
        {
            switch (property.ImageOptions.NameCreation)
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