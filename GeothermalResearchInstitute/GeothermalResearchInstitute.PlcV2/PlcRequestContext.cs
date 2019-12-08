// <copyright file="PlcRequestContext.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Threading.Tasks;

namespace GeothermalResearchInstitute.PlcV2
{
    public class PlcRequestContext
    {
        public TaskCompletionSource<PlcFrame> TaskCompletionSource { get; set; }

        public PlcFrame RequestFrame { get; set; }

        public DateTime? Deadline { get; set; }
    }
}
