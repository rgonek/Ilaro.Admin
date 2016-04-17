using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ilaro.Admin.Tests.TestModels.Northwind
{
    [Table("Order Details")]
    public class OrderDetail
    {
        [Key]
        [ForeignKey("Order")]
        public int OrderID { get; set; }

        [Key]
        public int ProductID { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        [Required]
        public short Quantity { get; set; }

        [Required]
        public float Discount { get; set; }

        [ForeignKey("OrderID")]
        public Order Order { get; set; }

        [ForeignKey("ProductID")]
        public virtual Product Product { get; set; }
    }
}