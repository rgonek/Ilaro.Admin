using System;

namespace Ilaro.Admin.Commons
{
    public interface IKnowTheTime
    {
        DateTime UtcNow { get; }
        DateTime Now { get; }
    }
}