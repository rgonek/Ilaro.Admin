using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Ilaro.Admin.Core.Extensions;
using Ilaro.Admin.Core.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Ilaro.Admin.Core
{
    [DebuggerDisplay("Entity {Name}")]
    [ValidateNever]
    public class Entity
    {
        public Type Type { get; private set; }

        public string Name { get; private set; }

        public string Table { get; private set; }

        public Verbose Verbose { get; }

        public IList<Property> Properties { get; private set; }

        public IEnumerable<Property> FilterProperties => Properties.Where(x => x.TypeInfo.IsBool);

        public Id Id => Properties.Where(x => x.IsKey).ToList();

        public IEnumerable<Property> ForeignKeys => Properties.Where(x => x.IsForeignKey);

        public bool IsChangeEntity => TypeInfo.IsChangeEntity(Type);

        public IList<GroupProperties> Groups { get; private set; } = new List<GroupProperties>();

        public IEnumerable<Property> DisplayProperties => Properties.Where(x => x.IsVisible);

        public IEnumerable<Property> SearchProperties => Properties.Where(x => x.IsSearchable);

        public IEnumerable<string> SelectableColumns => DisplayProperties
                .Union(Id.Keys)
                .Where(x => !x.IsForeignKey
                    || (!x.TypeInfo.IsCollection && x.IsForeignKey))
                .Select(x => Table + "." + x.Column)
                .Distinct();

        public bool IsSearchActive => SearchProperties.Any();

        public bool AllowAdd { get; private set; } = true;
        public bool AllowEdit { get; internal set; } = true;
        public bool AllowDelete { get; internal set; } = true;
        public Links Links { get; } = new Links();

        public bool HasToStringMethod { get; private set; }

        public string RecordDisplayFormat { get; internal set; }
        public bool SoftDeleteEnabled { get; internal set; }
        public bool ConcurrencyCheckEnabled { get; internal set; }

        public Property this[string propertyName] => Properties.FirstOrDefault(x => x.Name == propertyName);

        public Entity(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            Type = type;
            Name = Type.Name;
            SetTableName(Name.Pluralize());
            Verbose = new Verbose(Type);

            Properties = type.GetProperties()
                .Select(x => new Property(this, x))
                .ToList();

            if (IsChangeEntity)
            {
                AllowAdd = AllowEdit = AllowDelete = false;
            }

            // check if has ToString() method
            HasToStringMethod =
                Type.GetMethod("ToString")
                .DeclaringType.Name != "Object";
        }

        internal void SetTableName(string table, string schema = null)
        {
            Table = schema.HasValue()
                ? schema + "." + table
                : table;
        }
    }
}
