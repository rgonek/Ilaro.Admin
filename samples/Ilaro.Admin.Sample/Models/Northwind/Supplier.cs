using Ilaro.Admin.Core;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ilaro.Admin.Core.DataAnnotations;

namespace Ilaro.Admin.Sample.Models.Northwind
{
    [Groups("Main", "Contact", "Address")]
    [Columns("SupplierID", "CompanyName", "ContactName", "ContactTitle", "Country", "City")]
    [Verbose(GroupName = "Supplier")]
    public class Supplier
    {
        public int SupplierID { get; set; }

        [Required, StringLength(40)]
        [Display(GroupName = "Main")]
        public string CompanyName { get; set; }

        [StringLength(30)]
        [Display(GroupName = "Main")]
        public string ContactName { get; set; }

        [StringLength(30)]
        [Display(GroupName = "Main")]
        public string ContactTitle { get; set; }

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
        [Display(GroupName = "Contact")]
        public string Phone { get; set; }

        [StringLength(24)]
        [Display(GroupName = "Contact")]
        public string Fax { get; set; }

        [Display(GroupName = "Contact")]
        public string HomePage { get; set; }

        [Cascade(CascadeOption.Delete)]
        public IList<Product> Products { get; set; }
    }
}