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
        private bool disposedValue;

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
#if NETSTANDARD2_0
                    FileStatus = Array.Empty<WebHdfsFileStatus>(),
#else
                    FileStatus = new WebHdfsFileStatus[0],
#endif
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
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.directoryInfo.Dispose();
                }

                this.disposedValue = true;
            }
        }
    }
}
