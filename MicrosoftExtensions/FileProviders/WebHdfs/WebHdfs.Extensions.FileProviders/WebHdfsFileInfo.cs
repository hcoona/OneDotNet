// <copyright file="WebHdfsFileInfo.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;

namespace WebHdfs.Extensions.FileProviders
{
    public class WebHdfsFileInfo : IFileInfo, IDisposable
    {
        private readonly HttpClient httpClient;
        private readonly UriBuilder fileWebHdfsUriBuilder;
        private WebHdfsFileStatus fileStatus;
        private bool disposedValue;

        public WebHdfsFileInfo(Uri nameNodeUri, string relativePath)
        {
            this.NameNodeUri = nameNodeUri;
            this.RelativePath = relativePath;
            this.httpClient = new HttpClient();
            this.fileWebHdfsUriBuilder = new UriBuilder(new Uri(this.NameNodeUri, $"/webhdfs/v1/{this.RelativePath.Trim('/')}"));

            this.Refresh();
        }

        internal WebHdfsFileInfo(Uri nameNodeUri, string relativePath, WebHdfsFileStatus fileStatus)
        {
            this.NameNodeUri = nameNodeUri;
            this.RelativePath = relativePath;
            this.httpClient = new HttpClient();
            this.fileWebHdfsUriBuilder = new UriBuilder(new Uri(this.NameNodeUri, $"/webhdfs/v1/{this.RelativePath.Trim('/')}"));

            this.SetFileStatus(fileStatus);
        }

        public Uri NameNodeUri { get; }

        public string RelativePath { get; }

        public bool Exists { get; private set; }

        public long Length => this.fileStatus.Length;

        public string PhysicalPath => null;

        public string Name => Path.GetFileName(this.RelativePath);

        public DateTimeOffset LastModified => FromUnixTimeMilliseconds(this.fileStatus.ModificationTime);

        public bool IsDirectory => this.fileStatus.Type == WebHdfsFileType.DIRECTORY;

        public Stream CreateReadStream()
        {
            if (this.IsDirectory)
            {
                throw new InvalidOperationException("You cannot create read stream against a directory.");
            }

            this.fileWebHdfsUriBuilder.Query = "OP=OPEN";
            return this.httpClient.GetStreamAsync(this.fileWebHdfsUriBuilder.Uri).Result;
        }

        public void Refresh()
        {
            try
            {
                var statusObj = this.GetFileStatus().Result;
                this.SetFileStatus(WebHdfsFileStatus.ParseJson(this.GetFileStatus().Result));
            }
            catch (AggregateException ex) when (ex.InnerException is FileNotFoundException)
            {
                this.SetFileStatus(null);
            }
        }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        internal async Task<string> GetFileStatuses()
        {
            this.fileWebHdfsUriBuilder.Query = "OP=LISTSTATUS";
            var response = await this.httpClient.GetAsync(this.fileWebHdfsUriBuilder.Uri).ConfigureAwait(false);

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                return responseContent;
            }
            else
            {
                var responseContentObject = JsonConvert.DeserializeObject<dynamic>(responseContent);
                string message = responseContentObject.RemoteException.message;
                switch (response.StatusCode)
                {
                    case HttpStatusCode.BadRequest: throw new ArgumentException(message);
                    case HttpStatusCode.Unauthorized: throw new System.Security.SecurityException(message);
                    case HttpStatusCode.Forbidden: throw new IOException(message);
                    case HttpStatusCode.NotFound: throw new FileNotFoundException(message, this.Name);
                    case HttpStatusCode.InternalServerError: throw new InvalidOperationException(message);
                    default: throw new InvalidOperationException(message);
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.httpClient.Dispose();
                }

                this.disposedValue = true;
            }
        }

        private static DateTimeOffset FromUnixTimeMilliseconds(long milliseconds)
        {
#if _NET462 || _NETSTANDARD2_0
            return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds);
#else
            // Number of days in a non-leap year
            const int DaysPerYear = 365;

            // Number of days in 4 years
            const int DaysPer4Years = (DaysPerYear * 4) + 1;       // 1461

            // Number of days in 100 years
            const int DaysPer100Years = (DaysPer4Years * 25) - 1;  // 36524

            // Number of days in 400 years
            const int DaysPer400Years = (DaysPer100Years * 4) + 1; // 146097

            const int DaysTo1970 = (DaysPer400Years * 4) + (DaysPer100Years * 3) + (DaysPer4Years * 17) + DaysPerYear; // 719,162

            const long UnixEpochTicks = TimeSpan.TicksPerDay * DaysTo1970; // 621,355,968,000,000,000
            const long UnixEpochMilliseconds = UnixEpochTicks / TimeSpan.TicksPerMillisecond; // 62,135,596,800,000

            long minMilliseconds = (DateTime.MinValue.Ticks / TimeSpan.TicksPerMillisecond) - UnixEpochMilliseconds;
            long maxMilliseconds = (DateTime.MaxValue.Ticks / TimeSpan.TicksPerMillisecond) - UnixEpochMilliseconds;

            if (milliseconds < minMilliseconds || milliseconds > maxMilliseconds)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(milliseconds),
                    string.Format("Valid value between {0} and {1} (included).", minMilliseconds, maxMilliseconds));
            }

            long ticks = (milliseconds * TimeSpan.TicksPerMillisecond) + UnixEpochTicks;
            return new DateTimeOffset(ticks, TimeSpan.Zero);
#endif
        }

        private void SetFileStatus(WebHdfsFileStatus fileStatus)
        {
            if (fileStatus == null || fileStatus == WebHdfsFileStatus.Empty)
            {
                this.Exists = false;
                this.fileStatus = WebHdfsFileStatus.Empty;
            }
            else
            {
                this.Exists = true;
                this.fileStatus = fileStatus;
            }
        }

        private async Task<string> GetFileStatus()
        {
            this.fileWebHdfsUriBuilder.Query = "OP=GETFILESTATUS";
            var response = await this.httpClient.GetAsync(this.fileWebHdfsUriBuilder.Uri).ConfigureAwait(false);

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                return responseContent;
            }
            else
            {
                var responseContentObject = JsonConvert.DeserializeObject<dynamic>(responseContent);
                string message = responseContentObject.RemoteException.message;
                switch (response.StatusCode)
                {
                    case HttpStatusCode.BadRequest: throw new ArgumentException(message);
                    case HttpStatusCode.Unauthorized: throw new System.Security.SecurityException(message);
                    case HttpStatusCode.Forbidden: throw new IOException(message);
                    case HttpStatusCode.NotFound: throw new FileNotFoundException(message, this.Name);
                    case HttpStatusCode.InternalServerError: throw new InvalidOperationException(message);
                    default: throw new InvalidOperationException(message);
                }
            }
        }
    }
}
