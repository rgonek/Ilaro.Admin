using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ilaro.Admin.Core;
using Ilaro.Admin.DataAnnotations;
using DataType = System.ComponentModel.DataAnnotations.DataType;

namespace Ilaro.Admin.Sample.Models.Northwind
{
    public class Employee
    {
        [Key]
        public int EmployeeID { get; set; }

        [Required, StringLength(20)]
        [Compare("FirstName")]
        public string LastName { get; set; }

        [Required, StringLength(10)]
        public string FirstName { get; set; }

        [StringLength(30)]
        public string Title { get; set; }

        [StringLength(25)]
        public string TitleOfCourtesy { get; set; }

        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? HireDate { get; set; }

        [StringLength(60)]
        public string Address { get; set; }

        [StringLength(15)]
        public string City { get; set; }

        [StringLength(15)]
        public string Region { get; set; }

        [StringLength(10)]
        public string PostalCode { get; set; }

        [StringLength(15)]
        public string Country { get; set; }

        [StringLength(24)]
        public string HomePhone { get; set; }

        [StringLength(4)]
        public string Extension { get; set; }

        public byte[] Photo { get; set; }

        public string Notes { get; set; }

        public int? ReportsTo { get; set; }

        [StringLength(255)]
        public string PhotoPath { get; set; }

        [ForeignKey("ReportsTo")]
        public virtual Employee Manager { get; set; }

        [Cascade(CascadeOption.AskUser)]
        public virtual ICollection<Order> Orders { get; set; }

        [Cascade(CascadeOption.AskUser)]
        public virtual ICollection<EmployeeTerritory> EmployeeTerritories { get; set; }
    }
}