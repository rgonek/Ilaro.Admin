using Ilaro.Admin.Core;

namespace Ilaro.Admin.Models
{
    public class PropertyDeleteOption
    {
        public string EntityName { get; set; }
        public string HierarchyName { get; set; }
        public CascadeOption DeleteOption { get; set; }
        public bool ShowOptions { get; set; }
        public bool Collapsed { get; set; }
        public int Level { get; set; }
        public bool Visible { get; internal set; }
    }
}
