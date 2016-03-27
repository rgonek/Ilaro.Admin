using System;
using Ilaro.Admin.Core;

namespace Ilaro.Admin.Models
{
    public class Column
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public string SortDirection { get; set; }

        public Column(Property property, string order, string orderDirection)
        {
            order = order.ToLower();

            Name = property.Name;
            DisplayName = property.Display;
            Description = property.Description;
            SortDirection = 
                property.Name.ToLower() == order ?
                orderDirection == "asc" ? "up" : "down" : 
                String.Empty;
        }
    }
}
