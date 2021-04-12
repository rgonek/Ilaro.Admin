﻿using System.Collections.Generic;

namespace Ilaro.Admin.Sample.Northwind.Models
{
    public class Product
    {
        public int ProductID { get; set; }

        public string ProductName { get; set; }

        public int SupplierID { get; set; }

        public Category Category { get; set; }

        public string QuantityPerUnit { get; set; }

        public decimal UnitPrice { get; set; }

        public short? UnitsInStock { get; set; }

        public short? UnitsOnOrder { get; set; }

        public short? ReorderLevel { get; set; }

        public bool Discontinued { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }

        public override string ToString()
        {
            return ProductName;
        }
    }
}