using System;

namespace Ilaro.Admin.Models
{
    public class PagerInfo
    {
        public int Current { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }

        public PagerInfo(int current, int totalPages)
        {
            Current = current;
            TotalPages = totalPages;
        }

        public PagerInfo(int perPage, int page, int totalItems)
        {
            Current = page;
            TotalItems = totalItems;

            TotalPages = (int)Math.Ceiling(TotalItems / (double)perPage);
        }
    }
}
