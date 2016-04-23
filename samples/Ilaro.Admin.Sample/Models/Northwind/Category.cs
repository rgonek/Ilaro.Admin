using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ilaro.Admin.Core;
using Ilaro.Admin.DataAnnotations;

namespace Ilaro.Admin.Sample.Models.Northwind
{
    [Verbose(GroupName = "Product")]
    public class Category
    {
        public int CategoryID { get; set; }

        [StringLength(15)]
        public string CategoryName { get; set; }

        [Template(EditorTemplate = Templates.Editor.Markdown)]
        public string Description { get; set; }

        //public byte[] Picture { get; set; }

        [Cascade(CascadeOption.Delete)]
        public ICollection<Product> Products { get; set; }
    }
}