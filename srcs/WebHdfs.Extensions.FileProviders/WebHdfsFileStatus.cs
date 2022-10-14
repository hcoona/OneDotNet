// <copyright file="WebHdfsFileStatus.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using Newtonsoft.Json;

namespace WebHdfs.Extensions.FileProviders
{
    internal enum WebHdfsFileType
    {
        FILE,
        DIRECTORY,
        SYMLINK,
    }

    internal class WebHdfsFileStatus
    {
        public long Length { get; set; }

        public long ModificationTime { get; set; }

        public string PathSuffix { get; set; }

        public WebHdfsFileType Type { get; set; }

        internal static WebHdfsFileStatus Empty { get; } = new WebHdfsFileStatus();

        internal static WebHdfsFileStatus ParseJson(string json)
        {
            return JsonConvert.DeserializeAnonymousType(json, new
            {
                FileStatus = new WebHdfsFileStatus(),
            }).FileStatus;
        }
    }
}
