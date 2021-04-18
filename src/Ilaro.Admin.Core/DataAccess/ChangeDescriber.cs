﻿using System.Collections.Generic;
using System.Linq;
using Ilaro.Admin.Core.Extensions;
using System.Text;

namespace Ilaro.Admin.Core.DataAccess
{
    public class ChangeDescriber : IChangeDescriber
    {
        public string UpdateChanges(EntityRecord entityRecord, IDictionary<string, object> existingRecord)
        {
            var updateProperties = entityRecord.Values
                .WhereIsNotSkipped()
                .Where(x => x.Raw is ValueBehavior == false)
                .Where(value => value.Property.IsKey == false)
                .ToList();

            if (updateProperties.Any() == false)
                return "No changes";

            var changeBuilder = new StringBuilder();
            foreach (var propertyValue in updateProperties)
            {
                var columnName = propertyValue.Property.Column.Undecorate();
                if (existingRecord.ContainsKey(columnName))
                {
                    var oldValue = existingRecord[columnName];
                    changeBuilder.AppendFormat(
                        "{0} ({1} => {2})",
                        propertyValue.Property.Name,
                        oldValue.ToStringSafe(),
                        propertyValue.AsString);
                    changeBuilder.AppendLine();
                }
            }

            return changeBuilder.ToString();
        }

        public string CreateChanges(EntityRecord entityRecord)
        {
            var display = entityRecord.ToString();
            var joinedKeyValue = "#" + entityRecord.Id.ToString();
            if (display != joinedKeyValue)
            {
                display += " ({0})".Fill(joinedKeyValue);
            }
            return "Created " + display;
        }

        public string DeleteChanges(EntityRecord entityRecord, IDictionary<string, object> existingRecord)
        {
            var display = entityRecord.ToString();
            var joinedKeyValue = "#" + entityRecord.Id.ToString();
            if (display != joinedKeyValue)
            {
                display += " ({0})".Fill(joinedKeyValue);
            }
            return "Deleted " + display;
        }
    }
}