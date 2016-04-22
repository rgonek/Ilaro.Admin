using Ilaro.Admin.Configuration;
using Ilaro.Admin.Core;
using Ilaro.Admin.Sample.Models.Northwind;

namespace Ilaro.Admin.Sample.Configurators
{
    public class CategoryConfiguration : EntityConfiguration<Category>
    {
        public CategoryConfiguration()
        {
            Property(x => x.CategoryName, x => x.StringLength(15));
            Property(x => x.Description, x => x.Template(editor: Templates.Editor.Markdown));
            Property(x => x.Products, x => x.Cascade(CascadeOption.Delete));
        }
    }
}