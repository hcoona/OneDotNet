// Copyright (c) 2022 Zhang Shuai<zhangshuai.ustc@gmail.com>.
// All rights reserved.
//
// This file is part of OneDotNet.
//
// OneDotNet is free software: you can redistribute it and/or modify it under
// the terms of the GNU General Public License as published by the Free
// Software Foundation, either version 3 of the License, or (at your option)
// any later version.
//
// OneDotNet is distributed in the hope that it will be useful, but WITHOUT ANY
// WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE. See the GNU General Public License for more
// details.
//
// You should have received a copy of the GNU General Public License along with
// OneDotNet. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Diagnostics;

namespace Clocks
{
    /// <summary>
    /// System built-in stopwatch provider implemented by <seealso cref="Stopwatch"/>.
    /// </summary>
    /// <seealso cref="Clocks.IStopwatchProvider{long}" />
    public class SystemStopwatchProvider : IStopwatchProvider<long>
    {
        public bool IsHighResolution => Stopwatch.IsHighResolution;

        public IStopwatch Create()
        {
            return new SystemStopwatch(new Stopwatch());
        }

        public long GetNextTimestamp(long from, TimeSpan interval)
        {
            return from + (long)Math.Round(interval.TotalSeconds * Stopwatch.Frequency);
        }

        public long GetTimestamp()
        {
            return Stopwatch.GetTimestamp();
        }

        public TimeSpan ParseDuration(long fromTimestamp, long toTimestamp)
        {
            return
                TimeSpan.FromSeconds((toTimestamp - fromTimestamp) / (double)Stopwatch.Frequency);
        }

        public IStopwatch StartNew()
        {
            return new SystemStopwatch(Stopwatch.StartNew());
        }
    }
}
