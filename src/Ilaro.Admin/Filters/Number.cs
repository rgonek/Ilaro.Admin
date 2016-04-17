using System;
using Ilaro.Admin.Core;

namespace Ilaro.Admin.Filters
{
    public class Number : ITypeGrouper
    {
        public bool Match(Type type)
        {
            return TypeInfo.IsNumber(type);
        }
    }
}