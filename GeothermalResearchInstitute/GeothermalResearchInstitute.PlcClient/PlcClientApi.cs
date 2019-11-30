// <copyright file="PlcClientApi.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Threading;
using System.Threading.Tasks;
using GeothermalResearchInstitute.v2;

namespace GeothermalResearchInstitute.Plc
{
    public partial class PlcClient
    {
        public Task<TestResponse> TestAsync(
            TestRequest request,
            DateTime? deadline = null,
            CancellationToken cancellationToken = default)
        {
            return this.InvokeAsync(
                "/bjdire.v2.DeviceService/Test",
                request,
                TestResponse.Parser,
                deadline,
                cancellationToken);
        }
    }
}
