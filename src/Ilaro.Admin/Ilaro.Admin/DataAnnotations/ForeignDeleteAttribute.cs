using System;
using Ilaro.Admin.Core;

namespace Ilaro.Admin.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ForeignDeleteAttribute : Attribute
    {
        public DeleteOption DeleteOption { get; set; }

        public ForeignDeleteAttribute(DeleteOption deleteOption)
        {
            DeleteOption = deleteOption;
        }
    }
}