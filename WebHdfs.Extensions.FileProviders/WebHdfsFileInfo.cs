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

        public WebHdfsFileInfo(Uri nameNodeUri, string relativePath)
        {
            this.NameNodeUri = nameNodeUri;
            this.RelativePath = relativePath;
            this.httpClient = new HttpClient();
            this.fileWebHdfsUriBuilder = new UriBuilder(new Uri(NameNodeUri, $"/webhdfs/v1/{RelativePath.Trim('/')}"));

            Refresh();
        }

        internal WebHdfsFileInfo(Uri nameNodeUri, string relativePath, WebHdfsFileStatus fileStatus)
        {
            this.NameNodeUri = nameNodeUri;
            this.RelativePath = relativePath;
            this.httpClient = new HttpClient();
            this.fileWebHdfsUriBuilder = new UriBuilder(new Uri(NameNodeUri, $"/webhdfs/v1/{RelativePath.Trim('/')}"));

            SetFileStatus(fileStatus);
        }

        public Uri NameNodeUri { get; }

        public string RelativePath { get; }

        public bool Exists { get; private set; }

        public long Length => fileStatus.Length;

        public string PhysicalPath => null;

        public string Name => Path.GetFileName(RelativePath);

        public DateTimeOffset LastModified => FromUnixTimeMilliseconds(fileStatus.ModificationTime);

        public bool IsDirectory => fileStatus.Type == WebHdfsFileType.DIRECTORY;

        public Stream CreateReadStream()
        {
            if (IsDirectory)
            {
                throw new InvalidOperationException("You cannot create read stream against a directory.");
            }

            fileWebHdfsUriBuilder.Query = "OP=OPEN";
            return httpClient.GetStreamAsync(fileWebHdfsUriBuilder.Uri).Result;
        }

        public void Refresh()
        {
            try
            {
                var statusObj = GetFileStatus().Result;
                SetFileStatus(WebHdfsFileStatus.ParseJson(GetFileStatus().Result));
            }
            catch (AggregateException ex) when (ex.InnerException is FileNotFoundException)
            {
                SetFileStatus(null);
            }
        }

        private void SetFileStatus(WebHdfsFileStatus fileStatus)
        {
            if (fileStatus == null || fileStatus == WebHdfsFileStatus.Empty)
            {
                Exists = false;
                this.fileStatus = WebHdfsFileStatus.Empty;
            }
            else
            {
                Exists = true;
                this.fileStatus = fileStatus;
            }
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }

        private async Task<string> GetFileStatus()
        {
            fileWebHdfsUriBuilder.Query = "OP=GETFILESTATUS";
            var response = await httpClient.GetAsync(fileWebHdfsUriBuilder.Uri);

            var responseContent = await response.Content.ReadAsStringAsync();
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
                    case HttpStatusCode.NotFound: throw new FileNotFoundException(message, Name);
                    case HttpStatusCode.InternalServerError: throw new InvalidOperationException(message);
                    default: throw new InvalidOperationException(message);
                }
            }
        }

        internal async Task<string> GetFileStatuses()
        {
            fileWebHdfsUriBuilder.Query = "OP=LISTSTATUS";
            var response = await httpClient.GetAsync(fileWebHdfsUriBuilder.Uri);

            var responseContent = await response.Content.ReadAsStringAsync();
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
                    case HttpStatusCode.NotFound: throw new FileNotFoundException(message, Name);
                    case HttpStatusCode.InternalServerError: throw new InvalidOperationException(message);
                    default: throw new InvalidOperationException(message);
                }
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
            const int DaysPer4Years = DaysPerYear * 4 + 1;       // 1461
            // Number of days in 100 years
            const int DaysPer100Years = DaysPer4Years * 25 - 1;  // 36524
            // Number of days in 400 years
            const int DaysPer400Years = DaysPer100Years * 4 + 1; // 146097

            const int DaysTo1970 = DaysPer400Years * 4 + DaysPer100Years * 3 + DaysPer4Years * 17 + DaysPerYear; // 719,162

            const long UnixEpochTicks = TimeSpan.TicksPerDay * DaysTo1970; // 621,355,968,000,000,000
            const long UnixEpochMilliseconds = UnixEpochTicks / TimeSpan.TicksPerMillisecond; // 62,135,596,800,000

            long MinMilliseconds = DateTime.MinValue.Ticks / TimeSpan.TicksPerMillisecond - UnixEpochMilliseconds;
            long MaxMilliseconds = DateTime.MaxValue.Ticks / TimeSpan.TicksPerMillisecond - UnixEpochMilliseconds;

            if (milliseconds < MinMilliseconds || milliseconds > MaxMilliseconds)
            {
                throw new ArgumentOutOfRangeException("milliseconds",
                    string.Format("Valid value between {0} and {1} (included).", MinMilliseconds, MaxMilliseconds));
            }

            long ticks = milliseconds * TimeSpan.TicksPerMillisecond + UnixEpochTicks;
            return new DateTimeOffset(ticks, TimeSpan.Zero);
#endif
        }
    }
}
