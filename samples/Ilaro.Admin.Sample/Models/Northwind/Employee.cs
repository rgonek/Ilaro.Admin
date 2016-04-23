using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ilaro.Admin.Core;
using Ilaro.Admin.DataAnnotations;
using DataType = System.ComponentModel.DataAnnotations.DataType;

namespace Ilaro.Admin.Sample.Models.Northwind
{
    [Columns("FirstName", "LastName", "Title", "TitleOfCourtesy", "BirthDate",
        "HireDate", "Address", "City", "Region", "PostalCode", "Country",
        "HomePhone", "Extension")]
    [Groups("Main", "Address", "Notes*")]
    [Verbose(GroupName = "Employee")]
    public class Employee
    {
        public int EmployeeID { get; set; }

        [Required, StringLength(20)]
        [Display(GroupName = "Main")]
        public string LastName { get; set; }

        [Required, StringLength(10)]
        [Display(GroupName = "Main")]
        public string FirstName { get; set; }

        [StringLength(30)]
        [Display(GroupName = "Main")]
        public string Title { get; set; }

        [StringLength(25)]
        [Display(GroupName = "Main")]
        public string TitleOfCourtesy { get; set; }

        [DataType(DataType.Date)]
        [Display(GroupName = "Main")]
        public DateTime? BirthDate { get; set; }

        [DisplayFormat(DataFormatString = "dd.MM.yyyy")]
        [Display(GroupName = "Main")]
        public DateTime? HireDate { get; set; }

        [StringLength(60)]
        [Display(GroupName = "Address")]
        public string Address { get; set; }

        [StringLength(15)]
        [Display(GroupName = "Address")]
        public string City { get; set; }

        [StringLength(15)]
        [Display(GroupName = "Address")]
        public string Region { get; set; }

        [StringLength(10)]
        [Display(GroupName = "Address")]
        public string PostalCode { get; set; }

        [StringLength(15)]
        [Display(GroupName = "Address")]
        public string Country { get; set; }

        [StringLength(24)]
        [Display(GroupName = "Main")]
        public string HomePhone { get; set; }

        [StringLength(4)]
        [Display(GroupName = "Main")]
        public string Extension { get; set; }

        public byte[] Photo { get; set; }

        [Template(EditorTemplate = Templates.Editor.Html)]
        [Display(GroupName = "Notes")]
        public string Notes { get; set; }

        public int? ReportsTo { get; set; }

        [StringLength(255)]
        public string PhotoPath { get; set; }

        [ForeignKey("ReportsTo")]
        public virtual Employee Manager { get; set; }

        [Cascade(CascadeOption.Delete)]
        public virtual ICollection<Order> Orders { get; set; }

        [Cascade(CascadeOption.Delete)]
        public virtual ICollection<EmployeeTerritory> EmployeeTerritories { get; set; }
    }
}