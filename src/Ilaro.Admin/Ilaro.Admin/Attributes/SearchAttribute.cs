using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ilaro.Admin.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SearchAttribute : Attribute
    {
        public IList<string> Columns { get; set; }

        public SearchAttribute(params string[] columns)
        {
            Columns = columns;
        }
    }
}