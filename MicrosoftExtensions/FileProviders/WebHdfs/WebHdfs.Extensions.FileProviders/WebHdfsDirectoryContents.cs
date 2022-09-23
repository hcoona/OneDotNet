// <copyright file="WebHdfsDirectoryContents.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

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

        public bool Exists => this.directoryInfo.Exists;

        public IEnumerator<IFileInfo> GetEnumerator()
        {
            var responseContent = this.directoryInfo.GetFileStatuses().Result;
            var fileStatuses = JsonConvert.DeserializeAnonymousType(responseContent, new
            {
                FileStatuses = new
                {
                    FileStatus = new WebHdfsFileStatus[0],
                },
            }).FileStatuses.FileStatus;

            return fileStatuses.Select(s => new WebHdfsFileInfo(
                this.directoryInfo.NameNodeUri,
                Path.Combine(this.directoryInfo.RelativePath, s.PathSuffix),
                s) as IFileInfo).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Dispose()
        {
            this.directoryInfo.Dispose();
        }
    }
}
