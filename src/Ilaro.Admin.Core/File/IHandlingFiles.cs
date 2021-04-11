using System;
using System.Collections.Generic;

namespace Ilaro.Admin.Core.File
{
    public interface IHandlingFiles
    {
        /// <summary>
        /// Upload files to temp location, or save file byte array to property value
        /// </summary>
        IEnumerable<PropertyValue> Upload(
            EntityRecord entityRecord,
            Func<Property, object> defaultValueResolver);

        /// <summary>
        /// Delete files for existing record.
        /// </summary>
        void Delete(IEnumerable<PropertyValue> propertiesValues);

        /// <summary>
        /// Delete files uploaded in current request.
        /// </summary>
        void DeleteUploaded(IEnumerable<PropertyValue> propertiesValues);

        /// <summary>
        /// Move uploaded files from temp location, and delete old files.
        /// </summary>
        void ProcessUploaded(
            IEnumerable<PropertyValue> propertiesValues, 
            IDictionary<string, object> existingRecord = null);
    }
}