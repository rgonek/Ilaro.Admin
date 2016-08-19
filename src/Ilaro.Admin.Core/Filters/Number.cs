using System;

namespace Ilaro.Admin.Core.Filters
{
    public class Number : ITypeGrouper
    {
        public bool Match(Type type)
        {
            return TypeInfo.IsNumber(type);
        }
    }
}