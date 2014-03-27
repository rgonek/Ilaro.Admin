using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ilaro.Admin.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class VerboseAttribute : Attribute
    {
        public string Singular { get; set; }

        public string Plural { get; set; }

        public string GroupName { get; set; }
    }
}
