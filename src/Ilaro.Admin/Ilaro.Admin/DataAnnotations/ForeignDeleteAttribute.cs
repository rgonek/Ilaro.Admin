using System;
using Ilaro.Admin.Core;

namespace Ilaro.Admin.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ForeignDeleteAttribute : Attribute
    {
        public CascadeOption DeleteOption { get; set; }

        public ForeignDeleteAttribute(CascadeOption deleteOption)
        {
            DeleteOption = deleteOption;
        }
    }
}