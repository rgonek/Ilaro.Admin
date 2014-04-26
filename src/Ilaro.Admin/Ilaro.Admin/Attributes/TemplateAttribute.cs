using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ilaro.Admin.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class TemplateAttribute : Attribute
    {
        public string DisplayTemplate { get; set; }

        public string EditorTemplate { get; set; }
    }
}