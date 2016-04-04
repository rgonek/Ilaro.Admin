using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ilaro.Admin.Core;
using Ilaro.Admin.DataAnnotations;
using DataType = System.ComponentModel.DataAnnotations.DataType;

namespace Ilaro.Admin.Tests.TestModels.Northwind
{
    public class Employee
    {
        [Key]
        public int EmployeeID { get; set; }

        [Required, MaxLength(20)]
        public string LastName { get; set; }

        [Required, MaxLength(10)]
        public string FirstName { get; set; }

        [MaxLength(30)]
        public string Title { get; set; }

        [MaxLength(25)]
        public string TitleOfCourtesy { get; set; }

        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? HireDate { get; set; }

        [MaxLength(60)]
        public string Address { get; set; }

        [MaxLength(15)]
        public string City { get; set; }

        [MaxLength(15)]
        public string Region { get; set; }

        [MaxLength(10)]
        public string PostalCode { get; set; }

        [MaxLength(15)]
        public string Country { get; set; }

        [MaxLength(24)]
        public string HomePhone { get; set; }

        [MaxLength(4)]
        public string Extension { get; set; }

        public byte[] Photo { get; set; }

        public string Notes { get; set; }

        public int? ReportsTo { get; set; }

        [MaxLength(255)]
        public string PhotoPath { get; set; }

        [ForeignKey("ReportsTo")]
        public virtual Employee Manager { get; set; }

        [Cascade(CascadeOption.AskUser)]
        public virtual ICollection<Order> Orders { get; set; }

        [Cascade(CascadeOption.AskUser)]
        public virtual ICollection<EmployeeTerritory> EmployeeTerritories { get; set; }
    }
}