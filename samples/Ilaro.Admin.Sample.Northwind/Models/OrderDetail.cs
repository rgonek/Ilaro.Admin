﻿namespace Ilaro.Admin.Sample.Northwind.Models
{
    public class OrderDetail
    {
        public int OrderID { get; set; }

        public int ProductID { get; set; }

        public decimal UnitPrice { get; set; }

        public short Quantity { get; set; }

        public float Discount { get; set; }

        public virtual Product Product { get; set; }
    }
}