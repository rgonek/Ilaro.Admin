using System.Collections.Generic;

namespace Ilaro.Admin.Core.File
{
    public interface IHandlingFiles
    {
        /// <summary>
        /// Upload files to temp location, or save file byte array to property value
        /// </summary>
        IList<Property> Upload(Entity entity);

        /// <summary>
        /// Move uploaded files from temp location, and delete old files.
        /// </summary>
        void Delete(IEnumerable<Property> properties);

        /// <summary>
        /// Delete files uploaded in current request.
        /// </summary>
        void DeleteUploaded(IEnumerable<Property> properties);

        /// <summary>
        /// Delete files uploaded in current request.
        /// </summary>
        void ProcessUploaded(IEnumerable<Property> properties, object existingRecord = null);
    }
}