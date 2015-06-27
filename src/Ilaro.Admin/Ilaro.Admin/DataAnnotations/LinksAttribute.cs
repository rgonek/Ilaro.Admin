using System;

namespace Ilaro.Admin.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Class)]
    public class LinksAttribute : Attribute
    {
        public string DisplayLink { get; set; }

        public string EditLink { get; set; }

        public string DeleteLink { get; set; }

        public bool HasEditLink { get; set; }

        public bool HasDeleteLink { get; set; }

        public LinksAttribute()
        {
            HasEditLink = true;
            HasDeleteLink = true;
        }
    }
}