// <copyright file="SystemStopwatchProvider.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Diagnostics;

namespace Clocks
{
    /// <summary>
    /// System built-in stopwatch provider implemented by <seealso cref="Stopwatch"/>
    /// </summary>
    /// <seealso cref="Clocks.IStopwatchProvider{System.Int64}" />
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
            return TimeSpan.FromSeconds((toTimestamp - fromTimestamp) / (double)Stopwatch.Frequency);
        }

        public IStopwatch StartNew()
        {
            return new SystemStopwatch(Stopwatch.StartNew());
        }
    }
}
