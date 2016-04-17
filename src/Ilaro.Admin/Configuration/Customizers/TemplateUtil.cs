using Ilaro.Admin.Core;
using SystemDataType = System.ComponentModel.DataAnnotations.DataType;

namespace Ilaro.Admin.Configuration.Customizers
{
    internal class TemplateUtil
    {
        internal static string GetDisplayTemplate(PropertyTypeInfo typeInfo, bool isForeignKey)
        {
            if (isForeignKey)
            {
                return Templates.Display.Text;
            }

            if (typeInfo.SourceDataType != null)
            {
                switch (typeInfo.SourceDataType)
                {
                    case SystemDataType.DateTime:
                        return Templates.Display.DateTime;
                    case SystemDataType.Date:
                        return Templates.Display.Date;
                    case SystemDataType.Time:
                        return Templates.Display.DateTime;
                    case SystemDataType.Url:
                    case SystemDataType.Upload:
                        return Templates.Display.Url;
                    case SystemDataType.ImageUrl:
                        return Templates.Display.Image;
                    case SystemDataType.Currency:
                        return Templates.Display.Numeric;
                    case SystemDataType.Password:
                        return Templates.Display.Password;
                    case SystemDataType.Html:
                        return Templates.Display.Html;
                    case SystemDataType.MultilineText:
                        return Templates.Display.Text;
                    case SystemDataType.PhoneNumber:
                    case SystemDataType.Text:
                    case SystemDataType.EmailAddress:
                    case SystemDataType.CreditCard:
                    case SystemDataType.PostalCode:
                        return Templates.Display.Text;
                }
            }

            switch (typeInfo.DataType)
            {
                case DataType.Enum:
                    return Templates.Display.Text;
                case DataType.DateTime:
                    return Templates.Display.DateTime;
                case DataType.Bool:
                    return Templates.Display.Bool;
                case DataType.File:
                    return typeInfo.IsFileStoredInDb ?
                        Templates.Display.DbImage :
                        Templates.Display.File;
                case DataType.Image:
                    return typeInfo.IsFileStoredInDb ?
                        Templates.Display.DbImage :
                        Templates.Display.Image;
                case DataType.Numeric:
                    return Templates.Display.Numeric;
                default:
                    return Templates.Display.Text;
            }
        }

        internal static string GetEditorTemplate(PropertyTypeInfo typeInfo, bool isForeignKey)
        {
            if (isForeignKey)
            {
                return typeInfo.IsCollection ?
                    Templates.Editor.DualList :
                    Templates.Editor.DropDownList;
            }
            if (typeInfo.SourceDataType != null)
            {
                switch (typeInfo.SourceDataType)
                {
                    case SystemDataType.DateTime:
                        return Templates.Editor.DateTime;
                    case SystemDataType.Date:
                        return Templates.Editor.Date;
                    case SystemDataType.Time:
                        return Templates.Editor.Time;
                    case SystemDataType.Url:
                    case SystemDataType.Upload:
                        return Templates.Editor.File;
                    case SystemDataType.ImageUrl:
                        return Templates.Editor.File;
                    case SystemDataType.Currency:
                        return Templates.Editor.Numeric;
                    case SystemDataType.Password:
                        return Templates.Editor.Password;
                    case SystemDataType.Html:
                        return Templates.Editor.Html;
                    case SystemDataType.MultilineText:
                        return Templates.Editor.TextArea;
                    case SystemDataType.PhoneNumber:
                    case SystemDataType.Text:
                    case SystemDataType.EmailAddress:
                    case SystemDataType.CreditCard:
                    case SystemDataType.PostalCode:
                        return Templates.Editor.TextBox;
                }
            }

            switch (typeInfo.DataType)
            {
                case DataType.Enum:
                    return Templates.Editor.DropDownList;
                case DataType.DateTime:
                    return Templates.Editor.DateTime;
                case DataType.Bool:
                    return Templates.Editor.Checkbox;
                case DataType.File:
                    return Templates.Editor.File;
                case DataType.Image:
                    return Templates.Editor.File;
                case DataType.Numeric:
                    return Templates.Editor.Numeric;
                default:
                    return Templates.Editor.TextBox;
            }
        }
    }
}