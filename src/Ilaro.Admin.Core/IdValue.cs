using Dawn;
using Ilaro.Admin.Core.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ilaro.Admin.Core
{
    public class IdValue : IEnumerable<PropertyValue>
    {
        public IList<PropertyValue> KeyValues { get; }

        public int Count => KeyValues.Count;

        public bool IsComposite => Count > 1;

        public bool HasValue => KeyValues.Any(x => x.Raw != null);

        public IdValue(IList<PropertyValue> keyValues)
        {
            Guard.Argument(keyValues, nameof(keyValues)).NotNull().NotEmpty();

            KeyValues = keyValues;
        }

        public PropertyValue this[int index] => KeyValues[index];

        public static implicit operator IdValue(List<PropertyValue> keyValues) => new(keyValues);

        public static implicit operator List<PropertyValue>(IdValue idValue) => idValue.KeyValues.ToList();

        public static implicit operator string(IdValue id) => id.ToStringSafe();

        public override string ToString() => string.Join(Id.ColumnSeparator.ToString(), KeyValues.Select(x => x.AsString));

        public IEnumerator<PropertyValue> GetEnumerator() => KeyValues.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
