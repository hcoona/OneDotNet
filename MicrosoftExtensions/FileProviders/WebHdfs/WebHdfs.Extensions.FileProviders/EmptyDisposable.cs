using System;

namespace WebHdfs.Extensions.FileProviders
{
    internal class EmptyDisposable : IDisposable
    {
        public static EmptyDisposable Instance { get; } = new EmptyDisposable();

        public void Dispose()
        {
        }
    }
}
