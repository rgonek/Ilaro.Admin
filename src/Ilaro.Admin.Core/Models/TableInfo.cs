namespace Ilaro.Admin.Core.Models
{
    public class TableInfo
    {
        public int Page { get; set; } = 1;
        public string SearchQuery { get; set; }
        public int PerPage { get; set; } = 20;
        public string Order { get; set; }
        public string OrderDirection { get; set; }
    }
}