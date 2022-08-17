using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;

namespace WebHdfs.Extensions.FileProviders
{
    public class WebHdfsDirectoryContents : IDirectoryContents, IDisposable
    {
        private readonly WebHdfsFileInfo directoryInfo;

        public WebHdfsDirectoryContents(WebHdfsFileInfo directoryInfo)
        {
            this.directoryInfo = directoryInfo;
        }

        public bool Exists => directoryInfo.Exists;

        public IEnumerator<IFileInfo> GetEnumerator()
        {
            var responseContent = directoryInfo.GetFileStatuses().Result;
            var fileStatuses = JsonConvert.DeserializeAnonymousType(responseContent, new
            {
                FileStatuses = new
                {
                    FileStatus = new WebHdfsFileStatus[0]
                }
            }).FileStatuses.FileStatus;

            return fileStatuses.Select(s => new WebHdfsFileInfo(
                directoryInfo.NameNodeUri,
                Path.Combine(directoryInfo.RelativePath, s.PathSuffix),
                s) as IFileInfo).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Dispose()
        {
            directoryInfo.Dispose();
        }
    }
}
