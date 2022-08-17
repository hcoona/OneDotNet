using System;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace WebHdfs.Extensions.FileProviders
{
    public class WebHdfsFileProvider : IFileProvider
    {
        public WebHdfsFileProvider(Uri nameNodeUri)
            : this(nameNodeUri, TimeSpan.FromSeconds(5))
        { }

        public WebHdfsFileProvider(Uri nameNodeUri, TimeSpan defaultPollingInterval)
        {
            this.NameNodeUri = nameNodeUri;
            this.DefaultPollingInterval = defaultPollingInterval;
        }

        public Uri NameNodeUri { get; }

        public TimeSpan DefaultPollingInterval { get; set; }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            var directoryInfo = new WebHdfsFileInfo(NameNodeUri, subpath);
            if (directoryInfo.Exists && directoryInfo.IsDirectory)
            {
                return new WebHdfsDirectoryContents(directoryInfo);
            }
            else
            {
                return NotFoundDirectoryContents.Singleton;
            }
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            return new WebHdfsFileInfo(NameNodeUri, subpath);
        }

        public IChangeToken Watch(string filter)
        {
            return Watch(filter, DefaultPollingInterval);
        }

        public IChangeToken Watch(string filter, TimeSpan pollingInterval)
        {
            if (filter.Contains("*"))
            {
                throw new NotImplementedException("Do not support watching glob filter for now.");
            }
            else
            {
                return new PollingFileChangeToken(
                    (WebHdfsFileInfo)GetFileInfo(filter),
                    pollingInterval);
            }
        }
    }
}
