using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ilaro.Admin.Model
{

	/// <summary>
	/// Interface for entity wchich contains info about every change make in Ilaro.Admin
	/// </summary>
	public interface IEntityChange
	{
		int EntityChangeId { get; set; }

		string EntityName { get; set; }

		string EntityKey { get; set; }

		EntityChangeType ChangeType { get; set; }

		/// <summary>
		/// Used only for update type. Concrete information about what was updated.
		/// </summary>
		string Description { get; set; }

		DateTime ChangedOn { get; set; }

		// TODO: smarter way to get user identificator
		string ChangedBy { get; set; }
	}
}