// <copyright file="DeviceActualStates.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace GeothermalResearchInstitute.ServerConsole.Models
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Performance", "CA1819:属性不应返回数组", Justification = "Disable for DTO.")]
    public class DeviceActualStates : DeviceStates
    {
        public byte[] IPAddress { get; set; }
    }
}
