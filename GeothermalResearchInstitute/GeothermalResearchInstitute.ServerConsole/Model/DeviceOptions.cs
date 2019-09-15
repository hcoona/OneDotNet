// <copyright file="DeviceOptions.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace GeothermalResearchInstitute.ServerConsole.Model
{
    public class DeviceOptions
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Usage", "CA2227:集合属性应为只读", Justification = "Sets with reflection.")]
        public ICollection<DeviceOptionsEntry> Devices { get; set; }
    }
}
