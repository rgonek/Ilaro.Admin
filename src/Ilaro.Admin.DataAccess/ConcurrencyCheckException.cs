using System;

namespace Ilaro.Admin.DataAccess
{
    public class ConcurrencyCheckException : Exception
    {
        public ConcurrencyCheckException()
            : base()
        {
        }

        public ConcurrencyCheckException(string message)
            : base(message)
        {
        }
    }
}
