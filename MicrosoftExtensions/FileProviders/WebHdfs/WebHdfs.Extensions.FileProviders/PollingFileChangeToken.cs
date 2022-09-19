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
            this.previousWriteTimeUtc = GetLastWriteTimeUtc();
        }

        private DateTime GetLastWriteTimeUtc()
        {
            fileInfo.Refresh();
            return fileInfo.Exists ? fileInfo.LastModified.UtcDateTime : DateTime.MinValue;
        }

        public bool HasChanged
        {
            get
            {
                if (hasChanged)
                {
                    return hasChanged;
                }

                var currentTime = DateTime.UtcNow;
                if (currentTime - lastCheckedTimeUtc < pollingInterval)
                {
                    return hasChanged;
                }

                var lastWriteTimeUtc = GetLastWriteTimeUtc();
                if (previousWriteTimeUtc != lastWriteTimeUtc)
                {
                    previousWriteTimeUtc = lastWriteTimeUtc;
                    hasChanged = true;
                }

                lastCheckedTimeUtc = currentTime;
                return hasChanged;
            }
        }

        public bool ActiveChangeCallbacks => false;

        public IDisposable RegisterChangeCallback(Action<object> callback, object state)
        {
            return EmptyDisposable.Instance;
        }
    }
}

