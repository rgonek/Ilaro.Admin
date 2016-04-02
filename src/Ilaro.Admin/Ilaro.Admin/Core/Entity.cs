using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Ilaro.Admin.Extensions;
using Ilaro.Admin.Models;

namespace Ilaro.Admin.Core
{
    [DebuggerDisplay("Entity {Name}")]
    public class Entity
    {
        public Type Type { get; private set; }

        public string Name { get; private set; }

        public string Table { get; private set; }

        public Verbose Verbose { get; }

        public IList<Property> Properties { get; private set; }

        public IEnumerable<Property> FilterProperties
        {
            get
            {
                return Properties.Where(x => x.TypeInfo.IsBool);
            }
        }

        public IList<Property> Key
        {
            get
            {
                return Properties.Where(x => x.IsKey).ToList();
            }
        }

        public IEnumerable<Property> ForeignKey
        {
            get
            {
                return Properties.Where(x => x.IsForeignKey);
            }
        }

        public string JoinedKey
        {
            get { return string.Join(Const.KeyColSeparator.ToString(), Key.Select(x => x.Column)); }
        }

        public bool IsChangeEntity
        {
            get { return TypeInfo.IsChangeEntity(Type); }
        }

        public IList<GroupProperties> Groups { get; private set; }
            = new List<GroupProperties>();

        public IEnumerable<Property> DisplayProperties
        {
            get
            {
                return Properties.Where(x => x.IsVisible);
            }
        }

        public IEnumerable<Property> SearchProperties
        {
            get
            {
                return Properties.Where(x => x.IsSearchable);
            }
        }

        public bool IsSearchActive
        {
            get { return SearchProperties.Any(); }
        }

        public bool AllowAdd { get; private set; } = true;
        public bool AllowEdit { get; internal set; } = true;
        public bool AllowDelete { get; internal set; } = true;
        public Links Links { get; } = new Links();

        public bool HasToStringMethod { get; private set; }

        public string RecordDisplayFormat { get; internal set; }
        public bool SoftDeleteEnabled { get; internal set; }

        public Property this[string propertyName]
        {
            get { return Properties.FirstOrDefault(x => x.Name == propertyName); }
        }

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
            if (!schema.IsNullOrEmpty())
            {
                Table = "[" + schema + "].[" + table + "]";
            }
            else
            {
                Table = "[" + table + "]";
            }
        }
    }
}
