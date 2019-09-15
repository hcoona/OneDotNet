// <copyright file="SystemStopwatch.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Diagnostics;

namespace Clocks
{
    /// <summary>
    /// System built-in stopwatch implemented by <seealso cref="Stopwatch"/>
    /// </summary>
    /// <seealso cref="Clocks.IStopwatch" />
    internal sealed class SystemStopwatch : IStopwatch
    {
        private readonly Stopwatch stopwatch;

        public SystemStopwatch(Stopwatch stopwatch)
        {
            this.stopwatch = stopwatch;
        }

        public bool IsRunning => this.stopwatch.IsRunning;

        public TimeSpan Elapsed => this.stopwatch.Elapsed;

        public void Reset()
        {
            this.stopwatch.Reset();
        }

        public void Restart()
        {
#if NET20
            stopwatch.Stop();
            stopwatch.Start();
#else
            this.stopwatch.Restart();
#endif
        }

        public void Start()
        {
            this.stopwatch.Start();
        }

        public void Stop()
        {
            this.stopwatch.Stop();
        }
    }
}
