﻿using Ilaro.Admin.Core.Extensions;
using System;
using System.Linq;

namespace Ilaro.Admin.Core.Models
{
    public class ChangeRow
    {
        public EntityRecord Record { get; set; }

        public int EntityChangeId
        {
            get
            {
                return (int)this[nameof(IEntityChange.EntityChangeId)].AsObject;
            }
        }

        public string EntityName
        {
            get
            {
                return this[nameof(IEntityChange.EntityName)].AsString;
            }
        }

        public string EntityKey
        {
            get
            {
                return this[nameof(IEntityChange.EntityKey)].AsString;
            }
        }

        public EntityChangeType ChangeType
        {
            get
            {
                return (EntityChangeType)this[nameof(IEntityChange.ChangeType)].AsObject;
            }
        }

        public string ChangeTypeString
        {
            get
            {
                return ChangeType.ToString().SplitCamelCase();
            }
        }

        public string RecordDisplayName
        {
            get
            {
                return this[nameof(IEntityChange.RecordDisplayName)].AsString;
            }
        }

        public string Description
        {
            get
            {
                return this[nameof(IEntityChange.Description)].AsString;
            }
        }

        public DateTime ChangedOn
        {
            get
            {
                return (DateTime)this[nameof(IEntityChange.ChangedOn)].AsObject;
            }
        }

        public string ChangedOnString
        {
            get
            {
                return this[nameof(IEntityChange.ChangedOn)].AsString;
            }
        }

        public string ChangedBy
        {
            get
            {
                return this[nameof(IEntityChange.ChangedBy)].AsString;
            }
        }

        public PropertyValue this[string propertyName]
        {
            get { return Record.Values.FirstOrDefault(x => x.Property.Name == propertyName); }
        }

        public ChangeRow(EntityRecord record)
        {
            Record = record;
        }
    }
}