using System;

namespace Ilaro.Admin.ViewModels
{
    public class PagerInfo
    {
        public string Url { get; set; }

        public int Current { get; set; }

        public int TotalPages { get; set; }

        public int TotalItems { get; set; }

        public int PerPage { get; set; }

        public PagerInfo(string url, int current, int totalPages)
        {
            Url = url;
            Current = current;
            TotalPages = totalPages;
        }

        public PagerInfo(string url, int perPage, int page, int totalItems)
        {
            Url = url;
            PerPage = perPage;
            Current = page;
            TotalItems = totalItems;

            TotalPages = (int)Math.Ceiling(TotalItems / (double)PerPage);
        }
    }
}