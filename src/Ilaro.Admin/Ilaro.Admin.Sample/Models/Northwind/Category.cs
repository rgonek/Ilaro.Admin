using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ilaro.Admin.Core;
using Ilaro.Admin.DataAnnotations;

namespace Ilaro.Admin.Sample.Models.Northwind
{
    public class Category
    {
        public int CategoryID { get; set; }

        [StringLength(15)]
        public string CategoryName { get; set; }

        public string Description { get; set; }

        //public byte[] Picture { get; set; }

        [Cascade(CascadeOption.AskUser)]
        public ICollection<Product> Products { get; set; }
    }
}