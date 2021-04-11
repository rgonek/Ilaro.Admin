using System;

namespace Ilaro.Admin.Core.Filters
{
    public interface ITypeGrouper
    {
        bool Match(Type type);
    }
}