// <copyright file="PollingFileChangeToken.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Microsoft.Extensions.Primitives;

namespace WebHdfs.Extensions.FileProviders
{
    internal class PollingFileChangeToken : IChangeToken
    {
        private readonly WebHdfsFileInfo fileInfo;
        private readonly TimeSpan pollingInterval;
        private DateTime previousWriteTimeUtc;
        private DateTime lastCheckedTimeUtc;
        private bool hasChanged;

        public PollingFileChangeToken(WebHdfsFileInfo fileInfo, TimeSpan pollingInterval)
        {
            this.fileInfo = fileInfo;
            this.pollingInterval = pollingInterval;
            this.previousWriteTimeUtc = this.GetLastWriteTimeUtc();
        }

        public bool HasChanged
        {
            get
            {
                if (this.hasChanged)
                {
                    return this.hasChanged;
                }

                var currentTime = DateTime.UtcNow;
                if (currentTime - this.lastCheckedTimeUtc < this.pollingInterval)
                {
                    return this.hasChanged;
                }

                var lastWriteTimeUtc = this.GetLastWriteTimeUtc();
                if (this.previousWriteTimeUtc != lastWriteTimeUtc)
                {
                    this.previousWriteTimeUtc = lastWriteTimeUtc;
                    this.hasChanged = true;
                }

                this.lastCheckedTimeUtc = currentTime;
                return this.hasChanged;
            }
        }

        public bool ActiveChangeCallbacks => false;

        private DateTime GetLastWriteTimeUtc()
        {
            this.fileInfo.Refresh();
            return this.fileInfo.Exists ? this.fileInfo.LastModified.UtcDateTime : DateTime.MinValue;
        }

        public IDisposable RegisterChangeCallback(Action<object> callback, object state)
        {
            return EmptyDisposable.Instance;
        }
    }
}
