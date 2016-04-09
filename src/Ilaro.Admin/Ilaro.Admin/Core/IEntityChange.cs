using System;

namespace Ilaro.Admin.Core
{
    /// <summary>
    /// Interface for entity wchich contains info 
    /// about every change make in Ilaro.Admin
    /// </summary>
    public interface IEntityChange
    {
        int EntityChangeId { get; set; }

        string EntityName { get; set; }

        string EntityKey { get; set; }

        EntityChangeType ChangeType { get; set; }

        string RecordDisplayName { get; set; }

        /// <summary>
        /// Used only for update type. 
        /// Information about what was updated.
        /// </summary>
        string Description { get; set; }

        DateTime ChangedOn { get; set; }

        string ChangedBy { get; set; }
    }
}