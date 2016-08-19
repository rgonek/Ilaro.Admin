using System;

namespace Ilaro.Admin.Core
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