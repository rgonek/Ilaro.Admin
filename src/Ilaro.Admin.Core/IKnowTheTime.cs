﻿using System;

namespace Ilaro.Admin.Core
{
    public interface IKnowTheTime
    {
        DateTime UtcNow { get; }
        DateTime Now { get; }
    }
}