using Microsoft.AspNetCore.Http;

namespace Ilaro.Admin.Core.File
{
    public interface IFileNameCreator
    {
        string GetFileName(Property property, IFormFile file);
    }
}