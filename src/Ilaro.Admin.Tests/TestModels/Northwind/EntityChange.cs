using System;
using Ilaro.Admin.Core;

namespace Ilaro.Admin.Tests.TestModels.Northwind
{
    public class EntityChange : IEntityChange
    {
        public int EntityChangeId { get; set; }

        public string EntityName { get; set; }

        public string EntityKey { get; set; }

        public EntityChangeType ChangeType { get; set; }

        public string RecordDisplayName { get; set; }

        public string Description { get; set; }

        public DateTime ChangedOn { get; set; }

        public string ChangedBy { get; set; }
    }
}