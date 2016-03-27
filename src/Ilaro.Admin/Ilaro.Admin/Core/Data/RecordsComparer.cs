using System.Collections.Generic;
using System.Linq;
using Ilaro.Admin.Extensions;

namespace Ilaro.Admin.Core.Data
{
    public class RecordsComparer : IComparingRecords
    {
        public void SkipNotChangedProperties(Entity entity, IDictionary<string, object> existingRecord)
        {
            foreach (var property in entity
                .GetDefaultCreateProperties(getForeignCollection: false)
                .Where(x => x.IsKey == false)
                .WhereIsNotSkipped())
            {
                if (existingRecord.ContainsKey(property.Column.Undecorate()))
                {
                    var oldValue = existingRecord[property.Column.Undecorate()];
                    var equals = Equals(property.Value.Raw, oldValue);

                    if (equals)
                        property.Value.Raw = DataBehavior.Skip;
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