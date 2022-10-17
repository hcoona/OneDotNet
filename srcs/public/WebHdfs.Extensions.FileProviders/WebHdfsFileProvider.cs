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
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace WebHdfs.Extensions.FileProviders
{
    public class WebHdfsFileProvider : IFileProvider
    {
        public WebHdfsFileProvider(Uri nameNodeUri)
            : this(nameNodeUri, TimeSpan.FromSeconds(5))
        {
        }

        public WebHdfsFileProvider(Uri nameNodeUri, TimeSpan defaultPollingInterval)
        {
            this.NameNodeUri = nameNodeUri;
            this.DefaultPollingInterval = defaultPollingInterval;
        }

        public Uri NameNodeUri { get; }

        public TimeSpan DefaultPollingInterval { get; set; }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            var directoryInfo = new WebHdfsFileInfo(this.NameNodeUri, subpath);
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
            return new WebHdfsFileInfo(this.NameNodeUri, subpath);
        }

        public IChangeToken Watch(string filter)
        {
            return this.Watch(filter, this.DefaultPollingInterval);
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
                    (WebHdfsFileInfo)this.GetFileInfo(filter),
                    pollingInterval);
            }
        }
    }
}
