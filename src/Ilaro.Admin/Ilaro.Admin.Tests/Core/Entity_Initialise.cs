using Ilaro.Admin.Tests.TestModels.Northwind;
using Xunit;
using SystemDataType = System.ComponentModel.DataAnnotations.DataType;

namespace Ilaro.Admin.Tests.Core
{
    public class Entity_Initialise : TestBase
    {
        [Fact]
        public void default_table_name_should_be_created()
        {
            _admin.RegisterEntity<EntityChange>();
            _admin.Initialise();
            var entity = _admin.GetEntity<EntityChange>();

            Assert.Equal("[EntityChanges]", entity.Table);
        }

        [Fact]
        public void when_property_has_set_data_type__display_template_should_be_autmatic_set_to_proper_value()
        {
            _admin.RegisterEntity<Category>()
                .Property(x => x.CategoryName, x => x.Type(SystemDataType.Html));
            _admin.Initialise();
            var entity = _admin.GetEntity<Category>();

            Assert.Equal(Templates.Display.Html, entity["CategoryName"].Template.Display);
        }

        [Fact]
        public void when_property_has_set_data_type__editor_template_should_be_autmatic_set_to_proper_value()
        {
            _admin.RegisterEntity<Category>()
                .Property(x => x.CategoryName, x => x.Type(SystemDataType.Html));
            _admin.Initialise();
            var entity = _admin.GetEntity<Category>();

            Assert.Equal(Templates.Editor.Html, entity["CategoryName"].Template.Editor);
        }

        [Fact]
        public void when_property_has_set_data_type_and_display_template__display_template_should_not_be_autmatic_set()
        {
            _admin.RegisterEntity<Category>()
                .Property(x => x.CategoryName, x =>
                {
                    x.Type(SystemDataType.Html);
                    x.Template(display: Templates.Display.Url);
                });
            _admin.Initialise();
            var entity = _admin.GetEntity<Category>();

            Assert.Equal(Templates.Display.Url, entity["CategoryName"].Template.Display);
        }

        [Fact]
        public void when_property_has_set_data_type_and_editor_template__editor_template_should_not_be_autmatic_set()
        {
            _admin.RegisterEntity<Category>()
                .Property(x => x.CategoryName, x =>
                {
                    x.Type(SystemDataType.Html);
                    x.Template(editor: Templates.Editor.Markdown);
                });
            _admin.Initialise();
            var entity = _admin.GetEntity<Category>();

            Assert.Equal(Templates.Editor.Markdown, entity["CategoryName"].Template.Editor);
        }

        [Fact]
        public void when_property_has_set_editor_template_and_data_type__editor_template_should_not_be_autmatic_set()
        {
            _admin.RegisterEntity<Category>()
                .Property(x => x.CategoryName, x =>
                {
                    x.Template(editor: Templates.Editor.Markdown);
                    x.Type(SystemDataType.Html);
                });
            _admin.Initialise();
            var entity = _admin.GetEntity<Category>();

            Assert.Equal(Templates.Editor.Markdown, entity["CategoryName"].Template.Editor);
        }

        [Fact]
        public void when_property_is_byte_array_type__display_template_should_set_to_db_image()
        {
            _admin.RegisterEntity<Employee>();
            _admin.Initialise();
            var entity = _admin.GetEntity<Employee>();

            Assert.Equal(Templates.Display.DbImage, entity["Photo"].Template.Display);
        }

        [Fact]
        public void when_property_is_not_directly_set_as_foreign_key__it_should_be_determined_as_foreign_key()
        {
            _admin.RegisterEntity<Employee>()
                .Property(x => x.Manager, x =>
                {
                    x.ForeignKey("ReportsTo");
                });
            _admin.Initialise();
            var entity = _admin.GetEntity<Employee>();

            Assert.True(entity["ReportsTo"].IsForeignKey);
        }

        [Fact]
        public void when_property_is_not_directly_set_as_foreign_key__editor_template_should_be_set_to_drop_down_list()
        {
            _admin.RegisterEntity<Employee>()
                .Property(x => x.Manager, x =>
                {
                    x.ForeignKey("ReportsTo");
                });
            _admin.Initialise();
            var entity = _admin.GetEntity<Employee>();

            Assert.Equal(Templates.Editor.DropDownList, entity["ReportsTo"].Template.Editor);
        }

        [Fact]
        public void when_property_is_foreign_key_it_should_has_proper_column_name()
        {
            _admin.RegisterEntity<Product>()
                .Property(x => x.Category, x =>
                {
                    x.ForeignKey("CategoryID");
                });
            _admin.Initialise();
            var entity = _admin.GetEntity<Product>();

            var property = entity["Category"];
            Assert.Equal("[CategoryID]", property.Column);
        }

        [Fact]
        public void when_property_is_one_to_many__display_template_should_set_to_db_image()
        {
            _admin.RegisterEntity<Customer>();
            _admin.Initialise();
            var entity = _admin.GetEntity<Customer>();

            Assert.NotNull(entity["Orders"].Template.Display);
        }

        [Fact]
        public void when_property_is_one_to_many__editor_template_should_set_to_db_image()
        {
            _admin.RegisterEntity<Customer>();
            _admin.Initialise();
            var entity = _admin.GetEntity<Customer>();

            Assert.NotNull(entity["Orders"].Template.Editor);
        }
    }
}
