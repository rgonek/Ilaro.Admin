using System.Collections.Generic;
using System.Linq;
using Ilaro.Admin.Extensions;
using System.Text;

namespace Ilaro.Admin.Core.Data
{
    public class ChangesDescriber : IDescribingChanges
    {
        public string UpdateChanges(Entity entity, IDictionary<string, object> existingRecord)
        {
            var updateProperties = entity.GetDefaultCreateProperties(getForeignCollection: false)
                .Where(x => x.IsKey == false)
                .WhereIsNotSkipped().ToList();

            if (updateProperties.Any() == false)
                return "No changes";

            var changeBuilder = new StringBuilder();
            foreach (var property in updateProperties)
            {
                var columnName = property.ColumnName.Undecorate();
                if (existingRecord.ContainsKey(columnName))
                {
                    var oldValue = existingRecord[columnName];
                    changeBuilder.AppendFormat("{0} ({1} => {2})", property.Name, oldValue.ToStringSafe(), property.Value.AsString);
                    changeBuilder.AppendLine();
                }
            }

            return changeBuilder.ToString();
        }

        public string CreateChanges(Entity entity)
        {
            var display = entity.ToString();
            var joinedKeyValue = "#" + entity.JoinedKeyValue;
            if (display != joinedKeyValue)
            {
                display += " ({0})".Fill(joinedKeyValue);
            }
            return "Created " + display;
        }

        public string DeleteChanges(Entity entity, IDictionary<string, object> existingRecord)
        {
            var display = entity.ToString();
            var joinedKeyValue = "#" + entity.JoinedKeyValue;
            if (display != joinedKeyValue)
            {
                display += " ({0})".Fill(joinedKeyValue);
            }
            return "Deleted " + display;
        }
    }
}