using Newtonsoft.Json;

namespace WebHdfs.Extensions.FileProviders
{
    internal class WebHdfsFileStatus
    {
        public long Length { get; set; }

        public long ModificationTime { get; set; }

        public string PathSuffix { get; set; }

        public WebHdfsFileType Type { get; set; }

        internal static WebHdfsFileStatus ParseJson(string json)
        {
            return JsonConvert.DeserializeAnonymousType(json, new
            {
                FileStatus = new WebHdfsFileStatus()
            }).FileStatus;
        }

        internal static WebHdfsFileStatus Empty { get; } = new WebHdfsFileStatus();
    }

    internal enum WebHdfsFileType
    {
        FILE, DIRECTORY, SYMLINK
    }
}
