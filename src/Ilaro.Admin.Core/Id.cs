using Dawn;
using Ilaro.Admin.Core.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ilaro.Admin.Core
{
    public class Id : IEnumerable<Property>
    {
        public const char ColumnSeparator = '_';

        public IList<Property> Keys { get; }

        public int Count => Keys.Count;

        public Id(IList<Property> keys)
        {
            Guard.Argument(keys, nameof(keys)).NotNull().NotEmpty();

            Keys = keys;
        }

        public IdValue Fill(string value)
        {
            Guard.Argument(value, nameof(value)).NotNull().NotEmpty();

            var values = value.Split(ColumnSeparator)
                .Select(x => x.Trim())
                .ToArray();

            return Fill(values);
        }

        public IdValue Fill(params object[] values)
        {
            Guard.Argument(values, nameof(values)).Count(Keys.Count, (values, count) => "Values for all keys need to be provided. Not more, not less.");

            var keyValues = new List<PropertyValue>(values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                keyValues.Add(new PropertyValue(Keys[i], values[i]));
            }

            return keyValues;
        }

        public Property this[int index] => Keys[index];

        public static implicit operator Id(List<Property> keys) => new(keys);

        public static implicit operator List<Property>(Id id) => id.Keys.ToList();

        public static implicit operator string(Id id) => id.ToStringSafe();

        public override string ToString() => string.Join(ColumnSeparator.ToString(), Keys.Select(x => x.Column));

        public IEnumerator<Property> GetEnumerator() => Keys.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
