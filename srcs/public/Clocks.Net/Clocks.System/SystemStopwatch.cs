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
            this.stopwatch.Stop();
            this.stopwatch.Start();
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
