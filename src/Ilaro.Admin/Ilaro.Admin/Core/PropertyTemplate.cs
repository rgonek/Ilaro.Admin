using System.ComponentModel.DataAnnotations;
using System.Linq;
using Ilaro.Admin.DataAnnotations;
using Ilaro.Admin.Extensions;

namespace Ilaro.Admin.Core
{
    public class PropertyTemplate
    {
        public string Editor { get; internal set; }
        public string Display { get; internal set; }

        public PropertyTemplate(object[] attributes, PropertyTypeInfo typeInfo, bool isForeignKey)
        {
            SetTemplatesName(attributes, typeInfo, isForeignKey);
        }

        private void SetTemplatesName(object[] attributes, PropertyTypeInfo typeInfo, bool isForeignKey)
        {
            var uiHintAttribute = attributes
                .FirstOrDefault(x =>
                    x.GetType() == typeof(UIHintAttribute)) as UIHintAttribute;
            var templateAttribute = attributes
                .FirstOrDefault(x =>
                    x.GetType() == typeof(TemplateAttribute)) as TemplateAttribute;
            if (uiHintAttribute != null || templateAttribute != null)
            {
                if (uiHintAttribute != null)
                {
                    Editor = Display = uiHintAttribute.UIHint;
                }
                if (templateAttribute != null)
                {
                    if (!templateAttribute.DisplayTemplate.IsNullOrEmpty())
                    {
                        Display = templateAttribute.DisplayTemplate;
                    }
                    if (!templateAttribute.EditorTemplate.IsNullOrEmpty())
                    {
                        Editor = templateAttribute.EditorTemplate;
                    }
                }
            }
            else
            {
                if (isForeignKey)
                {
                    Editor = typeInfo.IsCollection ?
                        Templates.Editor.DualList :
                        Templates.Editor.DropDownList;
                    Display = Templates.Display.Text;
                }
                else if (typeInfo.SourceDataType != null)
                {
                    switch (typeInfo.SourceDataType)
                    {
                        case System.ComponentModel.DataAnnotations.DataType.DateTime:
                            Editor = Templates.Editor.DateTime;
                            Display = Templates.Display.DateTime;
                            break;
                        case System.ComponentModel.DataAnnotations.DataType.Date:
                            Editor = Templates.Editor.Date;
                            Display = Templates.Display.Date;
                            break;
                        case System.ComponentModel.DataAnnotations.DataType.Time:
                            Editor = Templates.Editor.Time;
                            Display = Templates.Display.Time;
                            break;
                        // TODO: for consideration
                        //case DataAnnotations.DataType.Duration:
                        //break;
                        case System.ComponentModel.DataAnnotations.DataType.Url:
                        case System.ComponentModel.DataAnnotations.DataType.Upload:
                            Editor = Templates.Editor.File;
                            Display = Templates.Display.Link;
                            break;
                        case System.ComponentModel.DataAnnotations.DataType.ImageUrl:
                            Editor = Templates.Editor.File;
                            Display = Templates.Display.Image;
                            break;
                        case System.ComponentModel.DataAnnotations.DataType.Currency:
                            Editor = Templates.Editor.Numeric;
                            Display = Templates.Display.Numeric;
                            break;
                        case System.ComponentModel.DataAnnotations.DataType.Password:
                            Editor = Templates.Editor.Password;
                            Display = Templates.Display.Hash;
                            break;
                        case System.ComponentModel.DataAnnotations.DataType.Html:
                            Editor = Templates.Editor.Html;
                            Display = Templates.Display.Html;
                            break;
                        case System.ComponentModel.DataAnnotations.DataType.MultilineText:
                            Editor = Templates.Editor.TextArea;
                            Display = Templates.Display.Text;
                            break;
                        case System.ComponentModel.DataAnnotations.DataType.PhoneNumber:
                        case System.ComponentModel.DataAnnotations.DataType.Text:
                        case System.ComponentModel.DataAnnotations.DataType.EmailAddress:
                        case System.ComponentModel.DataAnnotations.DataType.CreditCard:
                        case System.ComponentModel.DataAnnotations.DataType.PostalCode:
                            Editor = Templates.Editor.TextBox;
                            Display = Templates.Display.Text;
                            break;
                    }
                }

                if (!Display.IsNullOrEmpty()) return;

                switch (typeInfo.DataType)
                {
                    case DataType.Enum:
                        Editor = Templates.Editor.DropDownList;
                        Display = Templates.Display.Text;
                        break;
                    case DataType.DateTime:
                        Editor = Templates.Editor.DateTime;
                        Display = Templates.Display.DateTime;
                        break;
                    case DataType.Bool:
                        Editor = Templates.Editor.CheckBox;
                        Display = Templates.Display.Bool;
                        break;
                    case DataType.File:
                        Editor = Templates.Editor.File;
                        Display = Templates.Display.Image;
                        break;
                    case DataType.Numeric:
                        Editor = Templates.Editor.Numeric;
                        Display = Templates.Display.Numeric;
                        break;
                    default:
                        Editor = Templates.Editor.TextBox;
                        Display = Templates.Display.Text;
                        break;
                }
            }
        }
    }
}