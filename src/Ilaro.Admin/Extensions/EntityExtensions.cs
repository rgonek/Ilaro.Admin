using Ilaro.Admin.Core;

namespace Ilaro.Admin.Extensions
{
    public static class EntityExtensions
    {
        public static int LinksCount(this Entity entity)
        {
            var count = 0;
            if (entity.AllowEdit)
                count++;
            if (entity.AllowDelete)
                count++;

            if (entity.Links.Display.HasValue())
                count++;

            return count;
        }
    }
}