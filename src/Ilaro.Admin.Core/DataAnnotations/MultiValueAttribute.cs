using System;

namespace Ilaro.Admin.Core.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MultiValueAttribute : Attribute
    {
        public string Separator { get; set; }

        public MultiValueAttribute(string separator = ";")
        {
            Separator = separator;
        }
    }
}
