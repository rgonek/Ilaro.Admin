using System.Collections.Generic;
namespace Ilaro.Admin.Core.File
{
    public interface IHandlingFiles
    {
        IEnumerable<Property> Upload(Entity entity);
        void Delete(IEnumerable<Property> properties);
        void DeleteUploaded(IEnumerable<Property> properties);
        void ProcessUploaded(IEnumerable<Property> properties, object existingRecord = null);
    }
}