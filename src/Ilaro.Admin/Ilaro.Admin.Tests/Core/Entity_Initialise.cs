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

            Assert.Equal("[EntityChanges]", entity.TableName);
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
    }
}
