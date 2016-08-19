using System;

namespace Ilaro.Admin.Core.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Class)]
    public class VerboseAttribute : Attribute
    {
        public string Singular { get; set; }

        public string Plural { get; set; }

        public string GroupName { get; set; }
    }
}
