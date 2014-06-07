using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Ilaro.Admin.Attributes;
using Ilaro.Admin.Extensions;
using System.Diagnostics;
using Ilaro.Admin.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ilaro.Admin.ViewModels
{
	[DebuggerDisplay("Entity {Name}")]
	public class Entity
	{
		public Type Type { get; set; }

		public string Name { get; set; }

		public string TableName { get; set; }

		public string Singular { get; set; }

		public string Plural { get; set; }

		public IList<Property> Properties { get; set; }

		public IEnumerable<Property> FilterProperties
		{
			get
			{
				return Properties.Where(x => x.PropertyType == typeof(bool) || x.PropertyType == typeof(bool?));
			}
		}

		public Property Key
		{
			get
			{
				return Properties.FirstOrDefault(x => x.IsKey);
			}
		}

		public Property LinkKey
		{
			get
			{
				return Properties.FirstOrDefault(x => x.IsLinkKey);
			}
		}

		public bool IsChangeEntity { get; private set; }

		public string GroupName { get; set; }

		public IList<GroupPropertiesViewModel> Groups { get; set; }

		public IList<Property> DisplayProperties { get; set; }

		public IEnumerable<Property> SearchProperties { get; set; }

		#region Links

		public string DisplayLink { get; set; }

		public string EditLink { get; set; }

		public string DeleteLink { get; set; }

		public bool HasEditLink { get; set; }

		public bool HasDeleteLink { get; set; }

		public int LinksCount
		{
			get
			{
				var count = 0;

				if (HasEditLink)
				{
					count++;
				}

				if (HasDeleteLink)
				{
					count++;
				}

				if (!DisplayLink.IsNullOrEmpty())
				{
					count++;
				}

				return count;
			}
		}

		#endregion

		public bool CanAdd { get; set; }

		public bool HasToStringMethod { get; set; }

		public string RecordDisplayFormat { get; set; }

		public object[] Attributes { get; set; }

		public Property this[string propertyName]
		{
			get { return Properties.FirstOrDefault(x => x.Name == propertyName); }
		}

		public Entity(Type type)
		{
			this.Type = type;
			Name = Type.Name;

			IsChangeEntity = typeof(IEntityChange).IsAssignableFrom(Type);

			var verbose = (type.GetCustomAttributes(typeof(VerboseAttribute), false) as VerboseAttribute[]).FirstOrDefault();
			if (verbose != null)
			{
				this.Singular = verbose.Singular ?? type.Name.SplitCamelCase();
				this.Plural = verbose.Plural ?? this.Singular.Pluralize().SplitCamelCase();
				this.GroupName = verbose.GroupName ?? Resources.IlaroAdminResources.Others;
			}
			else
			{
				this.Singular = type.Name.SplitCamelCase();
				this.Plural = this.Singular.Pluralize().SplitCamelCase();
				this.GroupName = Resources.IlaroAdminResources.Others;
			}

			Properties = type.GetProperties().Select(x => new Property(this, x)).ToList();

			Attributes = type.GetCustomAttributes(false);

			SetTableName(Attributes);
			//SetColumns(attributes);
			SetLinks(Attributes);
			SetLinkKey();

			SetSearchProperties(Attributes);

			CanAdd = true;
			if (IsChangeEntity)
			{
				CanAdd = HasDeleteLink = HasEditLink = false;
			}

			// check if has ToString() method
			HasToStringMethod = Type.GetMethod("ToString").DeclaringType.Name != "Object";

			var recordDisplay = Attributes.OfType<RecordDisplayAttribute>().FirstOrDefault();
			if (recordDisplay != null)
			{
				RecordDisplayFormat = recordDisplay.DisplayFormat;
			}
		}

		private void SetTableName(object[] attributes)
		{
			var tableAttribute = attributes.OfType<TableAttribute>().FirstOrDefault();
			if (tableAttribute != null)
			{
				SetTableName(tableAttribute.Name, tableAttribute.Schema);
			}
			else
			{
				TableName = "[" + Name.Pluralize() + "]";
			}
		}

		internal void SetTableName(string table, string schema = null)
		{
			if (!schema.IsNullOrEmpty())
			{
				TableName = "[" + schema + "].[" + table + "]";
			}
			else
			{
				TableName = "[" + table + "]";
			}
		}

		public void SetLinkKey()
		{
			if (LinkKey == null && Key != null)
			{
				Key.IsLinkKey = true;
			}
		}

		public void SetColumns()
		{
			SetColumns(Attributes);
		}

		private void SetColumns(object[] attributes)
		{
			// if there any display properties that mean it was setted by fluent configuration 
			// and we don't want replace them
			if (DisplayProperties.IsNullOrEmpty())
			{
				var columnsAttribute = attributes.OfType<ColumnsAttribute>().FirstOrDefault();
				if (columnsAttribute != null)
				{
					SetColumns(columnsAttribute.Columns);
				}
				else
				{
					DisplayProperties = GetDisplayProperties().ToList();
				}
			}
		}

		internal void SetColumns(IEnumerable<string> properties)
		{
			DisplayProperties = new List<Property>();
			foreach (var column in properties)
			{
				DisplayProperties.Add(Properties.FirstOrDefault(x => x.Name == column));
			}
		}

		private void SetSearchProperties(object[] attributes)
		{
			var searchAttribute = attributes.OfType<SearchAttribute>().FirstOrDefault();
			if (searchAttribute != null)
			{
				SetSearchProperties(searchAttribute.Columns);
			}
			else
			{
				// TODO: Move types to other class
				SearchProperties = Properties.Where(x => !x.IsForeignKey && x.PropertyType.In(typeof(string), typeof(int), typeof(short), typeof(long), typeof(double), typeof(decimal), typeof(int?), typeof(short?), typeof(long?), typeof(double?), typeof(decimal?)));
			}
		}

		internal void SetSearchProperties(IEnumerable<string> properties)
		{
			SearchProperties = Properties.Where(x => properties.Contains(x.Name));
		}

		public void PrepareGroups()
		{
			if(!Groups.IsNullOrEmpty())
			{
				return;
			}

			SetGroups(Attributes);
		}

		private void SetGroups(object[] attributes)
		{
			var groupsAttribute = attributes.OfType<GroupsAttribute>().FirstOrDefault();
			if (groupsAttribute != null)
			{
				PrepareGroups(groupsAttribute.Groups);
			}
			else
			{
				PrepareGroups(new List<string>());
			}
		}

		private void PrepareGroups(IList<string> groupsNames)
		{
			var groupsDict = CreateProperties().GroupBy(x => x.GroupName).ToDictionary(x => x.Key);

			Groups = new List<GroupPropertiesViewModel>();
			if (groupsNames.IsNullOrEmpty())
			{
				foreach (var group in groupsDict)
				{
					Groups.Add(new GroupPropertiesViewModel
					{
						GroupName = group.Key,
						Properties = group.Value.ToList()
					});
				}
			}
			else
			{
				foreach (var groupName in groupsNames)
				{
					var trimedGroupName = groupName.TrimEnd('*');
					if (groupsDict.ContainsKey(trimedGroupName ?? Resources.IlaroAdminResources.Others))
					{
						var group = groupsDict[trimedGroupName];

						Groups.Add(new GroupPropertiesViewModel
						{
							GroupName = group.Key,
							Properties = group.ToList(),
							IsCollapsed = groupName.EndsWith("*")
						});
					}
				}
			}
		}

		internal void AddGroup(string group, bool isCollapsed, IEnumerable<string> propertiesNames)
		{
			if (Groups == null)
			{
				Groups = new List<GroupPropertiesViewModel>();
			}

			Groups.Add(new GroupPropertiesViewModel
			{
				GroupName = group,
				Properties = Properties.Where(x => propertiesNames.Contains(x.Name)),
				IsCollapsed = isCollapsed
			});
		}

		private void SetLinks(object[] attributes)
		{
			var linksAttribute = attributes.OfType<LinksAttribute>().FirstOrDefault();
			if (linksAttribute != null)
			{
				DisplayLink = linksAttribute.DisplayLink;
				EditLink = linksAttribute.EditLink;
				DeleteLink = linksAttribute.DeleteLink;
				HasEditLink = linksAttribute.HasEditLink;
				HasDeleteLink = linksAttribute.HasDeleteLink;
			}
			else
			{
				HasEditLink = true;
				HasDeleteLink = true;
			}
		}

		public IEnumerable<Property> CreateProperties(bool getKey = true, bool getForeignCollection = true)
		{
			foreach (var property in Properties)
			{
				// Get all properties which is not a key and foreign key
				if (!property.IsKey && !property.IsForeignKey)
				{
					yield return property;
				}
				// If property is key, and I want get a key (getKey == true) && data type is string
				else if (property.IsKey && property.DataType == DataType.Text && getKey)
				{
					yield return property;
				}
				else if (property.IsForeignKey)
				{
					// If is foreign key and not have reference property
					if (property.ReferenceProperty == null && (getForeignCollection || (!getForeignCollection && !property.IsCollection)))
					{
						yield return property;
					}
					// If is foreign key and have foreign key, that means, we have two properties for one database column, 
					// so I want only that one who is a system type
					else if (property.ReferenceProperty != null && property.IsSystemType && (getForeignCollection || (!getForeignCollection && !property.IsCollection)))
					{
						yield return property;
					}
				}
			}
		}

		private IEnumerable<Property> GetDisplayProperties()
		{
			foreach (var property in Properties)
			{
				// Get all properties which is not a key and foreign key
				if (!property.IsForeignKey)
				{
					yield return property;
				}

				else if (property.IsForeignKey)
				{
					// If is foreign key and not have reference property
					if (property.ReferenceProperty == null && !property.IsCollection)
					{
						yield return property;
					}
					// If is foreign key and have foreign key, that means, we have two properties for one database column, 
					// so I want only that one who is a system type
					else if (property.ReferenceProperty != null && property.IsSystemType && !property.IsCollection)
					{
						yield return property;
					}
				}
			}
		}

		internal void SetKey(string propertyName)
		{
			var property = Properties.FirstOrDefault(x => x.Name == propertyName);

			property.IsKey = true;
		}

		internal void SetLinkKey(string propertyName)
		{
			var property = Properties.FirstOrDefault(x => x.Name == propertyName);

			property.IsLinkKey = true;
		}
	}
}
