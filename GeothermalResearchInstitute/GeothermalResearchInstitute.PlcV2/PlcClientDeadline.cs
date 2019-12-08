// <copyright file="PlcClientDeadline.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace GeothermalResearchInstitute.PlcV2
{
    public partial class PlcClient
    {
        private void DeadlineBackgroundTaskEntryPoint()
        {
            CancellationToken closingCancellationToken = this.closingCancellationTokenSource.Token;
            while (!closingCancellationToken.WaitHandle.WaitOne(300))
            {
                DateTime now = DateTime.Now;
                foreach (KeyValuePair<int, PlcRequestContext> entry in this.requestContextReceivingDictionary)
                {
                    if (entry.Value.Deadline < now)
                    {
                        if (this.requestContextReceivingDictionary.TryRemove(entry.Key, out PlcRequestContext requestContext))
                        {
                            this.logger.LogWarning("Request {0} to {1} exceed deadline.", entry.Key, this.RemoteEndPoint);
                            requestContext.TaskCompletionSource.TrySetException(new RpcException(new Status(
                                StatusCode.DeadlineExceeded, string.Empty)));
                        }
                    }
                }
            }

            foreach (KeyValuePair<int, PlcRequestContext> entry in this.requestContextReceivingDictionary)
            {
                if (this.requestContextReceivingDictionary.TryRemove(entry.Key, out PlcRequestContext requestContext))
                {
                    requestContext.TaskCompletionSource.TrySetException(new RpcException(Status.DefaultCancelled));
                }
            }
        }
    }
}
