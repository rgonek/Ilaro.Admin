using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ilaro.Admin.Core;
using Ilaro.Admin.DataAnnotations;

namespace Ilaro.Admin.Sample.Models.Northwind
{
    [Groups("Main", "Others", "Stocks")]
    [Verbose(GroupName = "Product")]
    public class Product
    {
        public int ProductID { get; set; }

        [Required, StringLength(40)]
        [Display(GroupName = "Main")]
        public string ProductName { get; set; }

        [ForeignKey("Supplier")]
        [Display(Name = "Supplier", GroupName = "Others")]
        public int SupplierID { get; set; }

        [ForeignKey("CategoryID")]
        [Display(GroupName = "Main")]
        public Category Category { get; set; }

        [StringLength(20)]
        [Display(GroupName = "Main")]
        public string QuantityPerUnit { get; set; }

        [Display(GroupName = "Main")]
        public decimal UnitPrice { get; set; }

        [Display(GroupName = "Stocks")]
        public short? UnitsInStock { get; set; }

        [Display(GroupName = "Stocks")]
        public short? UnitsOnOrder { get; set; }

        [Display(GroupName = "Others")]
        public short? ReorderLevel { get; set; }

        [Required]
        [Display(GroupName = "Others")]
        public bool Discontinued { get; set; }

        [Cascade(CascadeOption.Delete)]
        public ICollection<OrderDetail> OrderDetails { get; set; }

        public override string ToString()
        {
            return ProductName;
        }
    }
}