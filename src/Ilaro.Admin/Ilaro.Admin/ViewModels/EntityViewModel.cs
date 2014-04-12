using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Ilaro.Admin.Attributes;
using Ilaro.Admin.Extensions;
using System.Diagnostics;
using Ilaro.Admin.Model;

namespace Ilaro.Admin.ViewModels
{
	[DebuggerDisplay("Entity {Name}")]
	public class EntityViewModel
	{
		public Type Type { get; set; }

		public string Name { get; set; }

		public string TableName { get; set; }

		public string Singular { get; set; }

		public string Plural { get; set; }

		public IList<PropertyViewModel> Properties { get; set; }

		public IEnumerable<PropertyViewModel> FilterProperties
		{
			get
			{
				return Properties.Where(x => x.PropertyType == typeof(bool) || x.PropertyType == typeof(bool?));
			}
		}

		public PropertyViewModel Key
		{
			get
			{
				return Properties.FirstOrDefault(x => x.IsKey);
			}
		}

		public PropertyViewModel LinkKey
		{
			get
			{
				return Properties.FirstOrDefault(x => x.IsLinkKey);
			}
		}

		public bool IsChangeEntity { get; private set; }

		public string GroupName { get; set; }

		public IList<string> Groups { get; set; }

		public IList<PropertyViewModel> DisplayColumns { get; set; }

		public IEnumerable<PropertyViewModel> SearchProperties { get; set; }

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

		public EntityViewModel(Type type)
		{
			this.Type = type;
			Name = Type.Name;

			IsChangeEntity = typeof(IEntityChange).IsAssignableFrom(Type);

			// TODO: better determine table name
			TableName = Name.Pluralize();

			var verbose = (type.GetCustomAttributes(typeof(VerboseAttribute), false) as VerboseAttribute[]).FirstOrDefault();
			if (verbose != null)
			{
				this.Singular = verbose.Singular;
				this.Plural = verbose.Plural ?? this.Singular.Pluralize();
				this.GroupName = verbose.GroupName ?? Resources.IlaroAdminResources.Others;
			}
			else
			{
				this.Singular = type.Name.SplitCamelCase();
				this.Plural = this.Singular.Pluralize().SplitCamelCase();
				this.GroupName = Resources.IlaroAdminResources.Others;
			}

			Properties = type.GetProperties().Select(x => new PropertyViewModel(this, x)).ToList();

			var attributes = type.GetCustomAttributes(false);

			SetColumns(attributes);
			SetLinks(attributes);
			SetLinkKey();

			SetSearchProperties(attributes);

			SetGroups(attributes);

			CanAdd = true;
			if (IsChangeEntity)
			{
				CanAdd = HasDeleteLink = HasEditLink = false;
			}
		}

		private void SetLinkKey()
		{
			if (LinkKey == null && Key != null)
			{
				Key.IsLinkKey = true;
			}
		}

		private void SetColumns(object[] attributes)
		{
			var columnsAttribute = attributes.OfType<ColumnsAttribute>().FirstOrDefault();
			if (columnsAttribute != null)
			{
				DisplayColumns = new List<PropertyViewModel>();
				foreach (var column in columnsAttribute.Columns)
				{
					DisplayColumns.Add(Properties.FirstOrDefault(x => x.Name == column));
				}
				//DisplayColumns = Properties.Where(x => columnsAttribute.Columns.Contains(x.Name));
			}
			else
			{
				DisplayColumns = Properties.Where(x => !x.IsForeignKey).ToList();
			}
		}

		private void SetSearchProperties(object[] attributes)
		{
			var searchAttribute = attributes.OfType<SearchAttribute>().FirstOrDefault();
			if (searchAttribute != null)
			{
				SearchProperties = Properties.Where(x => searchAttribute.Columns.Contains(x.Name));
			}
			else
			{
				SearchProperties = Properties.Where(x => !x.IsForeignKey && x.PropertyType.In(typeof(string), typeof(int), typeof(short), typeof(long), typeof(double), typeof(decimal), typeof(int?), typeof(short?), typeof(long?), typeof(double?), typeof(decimal?)));
			}
		}

		private void SetGroups(object[] attributes)
		{
			var groupsAttribute = attributes.OfType<GroupsAttribute>().FirstOrDefault();
			if (groupsAttribute != null)
			{
				Groups = groupsAttribute.Groups.ToList();
			}
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

		public IEnumerable<PropertyViewModel> CreateProperties(bool getKey = true)
		{
			return Properties.Where(x =>
				!x.IsForeignKey &&
				(
					!x.IsKey ||
						(
							x.IsKey &&
							x.PropertyType == typeof(string) &&
							getKey
						)
				)
			);
		}
	}
}
