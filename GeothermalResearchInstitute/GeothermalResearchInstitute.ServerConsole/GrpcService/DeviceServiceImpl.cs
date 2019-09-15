// <copyright file="DeviceServiceImpl.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Threading.Tasks;
using GeothermalResearchInstitute.v1;
using Grpc.Core;

namespace GeothermalResearchInstitute.ServerConsole.GrpcService
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Performance", "CA1812", Justification = "Instantiated with reflection.")]
    internal class DeviceServiceImpl : DeviceService.DeviceServiceBase
    {
        public override Task<ListDevicesResponse> ListDevices(ListDevicesRequest request, ServerCallContext context)
        {
            return base.ListDevices(request, context);
        }

        public override Task<Device> GetDevice(GetDeviceRequest request, ServerCallContext context)
        {
            return base.GetDevice(request, context);
        }

        public override Task<Device> UpdateDevice(UpdateDeviceRequest request, ServerCallContext context)
        {
            return base.UpdateDevice(request, context);
        }
    }
}
