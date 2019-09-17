// <copyright file="DeviceOptionsEntry.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Net.NetworkInformation;

namespace GeothermalResearchInstitute.ServerConsole.Models
{
    public class DeviceOptionsEntry
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public byte[] ComputeIdBinary()
        {
            return PhysicalAddress
                .Parse(this.Id
#if NET48
                    .Replace(":", null)
                    .Replace("-", null))
#else
                    .Replace(":", null, StringComparison.Ordinal)
                    .Replace("-", null, StringComparison.Ordinal))
#endif
                .GetAddressBytes();
        }
    }
}
