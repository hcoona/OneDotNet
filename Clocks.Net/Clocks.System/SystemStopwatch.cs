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

        public bool IsRunning => stopwatch.IsRunning;

        public TimeSpan Elapsed => stopwatch.Elapsed;

        public void Reset()
        {
            stopwatch.Reset();
        }

        public void Restart()
        {
#if NET20
            stopwatch.Stop();
            stopwatch.Start();
#else
            stopwatch.Restart();
#endif
        }

        public void Start()
        {
            stopwatch.Start();
        }

        public void Stop()
        {
            stopwatch.Stop();
        }
    }
}
