using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ilaro.Admin.Sample.Models.Northwind
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
        [DisplayFormat(DataFormatString = "C")]
        public decimal UnitPrice { get; set; }

        [Required]
        public short Quantity { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "0%")]
        public float Discount { get; set; }

        [ForeignKey("ProductID")]
        public virtual Product Product { get; set; }
    }
}