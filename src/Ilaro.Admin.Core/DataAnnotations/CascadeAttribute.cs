using System;
using Ilaro.Admin.Core;

namespace Ilaro.Admin.Core.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CascadeAttribute : Attribute
    {
        public CascadeOption DeleteOption { get; set; }

        public CascadeAttribute(CascadeOption deleteOption)
        {
            DeleteOption = deleteOption;
        }
    }
}