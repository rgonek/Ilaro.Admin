using System.Collections.Generic;

namespace Ilaro.Admin.Models
{
    public class GroupIndexModel
    {
        public IList<GroupModel> Groups { get; set; }
        public bool ChangeEnabled { get; set; }
        public IList<ChangeRow> Changes { get; set; }
    }
}
