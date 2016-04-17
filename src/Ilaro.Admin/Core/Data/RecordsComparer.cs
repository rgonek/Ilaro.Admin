using System.Collections.Generic;
using System.Linq;
using Ilaro.Admin.Extensions;

namespace Ilaro.Admin.Core.Data
{
    public class RecordsComparer : IComparingRecords
    {
        public void SkipNotChangedProperties(
            EntityRecord entityRecord, 
            IDictionary<string, object> existingRecord)
        {
            foreach (var property in entityRecord.Values
                .WhereIsNotSkipped()
                .Where(value => value.Property.IsKey == false))
            {
                if (existingRecord.ContainsKey(property.Property.Column.Undecorate()))
                {
                    var oldValue = existingRecord[property.Property.Column.Undecorate()];
                    var equals = Equals(property.Raw, oldValue);

                    if (equals)
                        property.DataBehavior = DataBehavior.Skip;
                }
            }
        }

        private new bool Equals(object newValue, object oldValue)
        {
            return
                (newValue == null && oldValue == null) ||
                (newValue != null && oldValue != null && newValue.Equals(oldValue));
        }
    }
}