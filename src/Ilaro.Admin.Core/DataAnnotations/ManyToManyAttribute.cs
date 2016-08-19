using System;

namespace Ilaro.Admin.Core.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ManyToManyAttribute : Attribute
    {
        public ManyToManyAttribute()
        {
        }
    }
}