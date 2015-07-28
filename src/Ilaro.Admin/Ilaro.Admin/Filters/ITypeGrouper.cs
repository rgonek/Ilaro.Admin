using System;

namespace Ilaro.Admin.Filters
{
    public interface ITypeGrouper
    {
        bool Match(Type type);
    }
}