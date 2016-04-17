using System;

namespace Ilaro.Admin.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ConcurrencyCheckAttribute : Attribute
    {
    }
}