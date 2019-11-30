// <copyright file="PlcClientRequestContext.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Threading;
using System.Threading.Tasks;
using GeothermalResearchInstitute.v2;
using Google.Protobuf;

namespace GeothermalResearchInstitute.Plc
{
    public partial class PlcClient
    {
        private class RequestContext
        {
            public string Path { get; set; }

            public IMessage Message { get; set; }

            public DateTime? Deadline { get; set; }

            public CancellationToken CancellationToken { get; set; }

            public TaskCompletionSource<UnifiedFrameContent> TaskCompletionSource { get; set; }
        }
    }
}
