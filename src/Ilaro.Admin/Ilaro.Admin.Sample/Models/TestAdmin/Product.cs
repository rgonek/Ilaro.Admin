using System;
using System.ComponentModel.DataAnnotations;
using Ilaro.Admin.Attributes;
using Ilaro.Admin.FileUpload;

namespace Ilaro.Admin.Sample.Models.TestAdmin
{
    //[Columns("Image", "Category", "Name")]
    [Search("Number")]
    [Groups("Podstawowe", "Daty*", "Status*", "Pozostałe*")]
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        [MaxLength(20)]
        [Display(Name = "Nazwa", Description = "To jest nazwa produktu. Maksymalnie 20 znaków.", GroupName = "Podstawowe")]
        public string Name { get; set; }

        [Image(NameCreation = NameCreation.Timestamp), ImageSettings("content/product/big", 800, 600, IsBig = true), ImageSettings("content/product/min", 50, 50, IsMiniature = true)]
        public string Image { get; set; }

        [MaxLength(100)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Wstęp", Description = "To jest krótki opis produktu. Maksymalnie 100 znaków.", GroupName = "Podstawowe")]
        public string Lead { get; set; }

        [Required]
        [DataType(DataType.Html)]
        [Display(Name = "Opis", Description = "To jest opis produktu. Maksymalnie 100 znaków.", GroupName = "Podstawowe")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Kategoria", GroupName = "Podstawowe")]
        [EnumDataType(typeof(ProductCategory))]
        public int Category { get; set; }
        // oldest ef dont support enums
        //public ProductCategory Category { get; set; }

        public int Number { get; set; }

        [Display(GroupName = "Status")]
        public bool IsDeleted { get; set; }

        [Display(GroupName = "Status")]
        public bool IsSold { get; set; }

        [Required]
        [Display(GroupName = "Daty")]
        public DateTime DateAdd { get; set; }

        [Display(GroupName = "Daty")]
        public DateTime? DateSold { get; set; }
    }
}