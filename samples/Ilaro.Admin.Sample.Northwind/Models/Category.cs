using System.Collections.Generic;

namespace Ilaro.Admin.Sample.Northwind.Models
{
    public class Category
    {
        public int CategoryID { get; set; }

        public string CategoryName { get; set; }

        public string Description { get; set; }

        //public byte[] Picture { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}