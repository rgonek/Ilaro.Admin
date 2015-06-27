using System.Linq;
using Ilaro.Admin.DataAnnotations;
using Ilaro.Admin.Extensions;

namespace Ilaro.Admin.Core
{
    public class Links
    {
        public string Display { get; internal set; }
        public string Edit { get; internal set; }
        public string Delete { get; internal set; }

        public bool HasEdit { get; internal set; }
        public bool HasDelete { get; internal set; }

        public int Count
        {
            get
            {
                var count = 0;

                if (HasEdit)
                {
                    count++;
                }

                if (HasDelete)
                {
                    count++;
                }

                if (!Display.IsNullOrEmpty())
                {
                    count++;
                }

                return count;
            }
        }

        public Links(object[] attributes)
        {
            var linksAttribute =
                attributes.OfType<LinksAttribute>().FirstOrDefault();
            if (linksAttribute != null)
            {
                Display = linksAttribute.DisplayLink;
                Edit = linksAttribute.EditLink;
                Delete = linksAttribute.DeleteLink;
                HasEdit = linksAttribute.HasEditLink;
                HasDelete = linksAttribute.HasDeleteLink;
            }
            else
            {
                HasEdit = true;
                HasDelete = true;
            }
        }
    }
}