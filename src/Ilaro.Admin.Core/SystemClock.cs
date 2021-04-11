﻿using System;

namespace Ilaro.Admin.Core
{
    public class SystemClock : IKnowTheTime
    {
        public static readonly SystemClock Instance = new SystemClock();

        public DateTime UtcNow
        {
            get { return DateTime.UtcNow; }
        }

        public DateTime Now
        {
            get { return DateTime.Now; }
        }
    }
}