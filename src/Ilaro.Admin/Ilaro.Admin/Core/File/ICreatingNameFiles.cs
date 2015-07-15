using System.Web;

namespace Ilaro.Admin.Core.File
{
    public interface ICreatingNameFiles
    {
        string GetFileName(Property property, HttpPostedFileWrapper file);
    }
}