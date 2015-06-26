using System.Collections.Generic;

namespace Ilaro.Admin.ViewModels
{
    public class DataRowViewModel
    {
        public string KeyValue { get; set; }

        public string LinkKeyValue { get; set; }

        public string DisplayName { get; set; }

        public IList<CellValueViewModel> Values { get; set; }

        public DataRowViewModel()
        {
            Values = new List<CellValueViewModel>();
        }
    }
}