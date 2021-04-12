using Ilaro.Admin.Core;
using Ilaro.Admin.Core.Configuration;
using Ilaro.Admin.Sample.Northwind.Models;

namespace Ilaro.Admin.Sample.Northwind.Configurators
{
    public class CategoryConfiguration : EntityConfiguration<Category>
    {
        public CategoryConfiguration()
        {
            Group("Product");

            //Property(x => x.CategoryName, x => x.StringLength(15));
            Property(x => x.Description, x => x.Template(editor: Templates.Editor.Markdown));
            Property(x => x.Products, x => x.Cascade(CascadeOption.Delete));
        }
    }
}