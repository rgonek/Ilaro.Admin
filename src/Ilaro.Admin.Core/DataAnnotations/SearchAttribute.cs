using System;
using System.Collections.Generic;

namespace Ilaro.Admin.Core.DataAnnotations
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