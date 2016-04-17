using System;
using Ilaro.Admin.Core;

namespace Ilaro.Admin.Tests
{
    public class FakeClock : IKnowTheTime
    {
        private readonly Func<DateTime> _utcNow;
        private readonly Func<DateTime> _now;

        public DateTime UtcNow
        {
            get { return _utcNow(); }
        }
        public DateTime Now
        {
            get { return _now(); }
        }

        public FakeClock(Func<DateTime> utcNow, Func<DateTime> now)
        {
            if (utcNow == null) throw new ArgumentNullException(nameof(utcNow));
            if (now == null) throw new ArgumentNullException(nameof(now));

            _utcNow = utcNow;
            _now = now;
        }
    }
}
