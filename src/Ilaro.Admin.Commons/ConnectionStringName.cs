using Dawn;
using System.Collections.Generic;

namespace Ilaro.Admin.Common
{
    public class ConnectionStringName : ValueObject
    {
        public string Value { get; }

        private ConnectionStringName()
        {
        }

        public ConnectionStringName(string value)
        {
            Guard.Argument(value, nameof(value)).NotNull().NotEmpty();

            Value = value;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }


        public static implicit operator ConnectionStringName(string connectionStringName)
        {
            return new ConnectionStringName(connectionStringName);
        }

        public static implicit operator string(ConnectionStringName connectionStringName)
        {
            return connectionStringName.Value;
        }
    }
}
