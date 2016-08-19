using Ilaro.Admin.Core;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ilaro.Admin.Core.DataAnnotations;

namespace Ilaro.Admin.Sample.Models.Northwind
{
    [Table("Region")]
    public class Region
    {
        public int RegionID { get; set; }

        [Required, StringLength(50)]
        [Template(EditorTemplate = Templates.Editor.TextArea)]
        public string RegionDescription { get; set; }

        [Cascade(CascadeOption.Delete)]
        public ICollection<Territory> Territories { get; set; }
    }
}