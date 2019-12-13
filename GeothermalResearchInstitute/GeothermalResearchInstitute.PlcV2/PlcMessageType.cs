// <copyright file="PlcMessageType.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;

namespace GeothermalResearchInstitute.PlcV2
{
    [SuppressMessage("Design", "CA1028:枚举存储应为 Int32", Justification = "业务需求")]
    public enum PlcMessageType : ushort
    {
        ConnectRequest = 0x0100,
        ConnectResponse = 0x0101,
        GetMetricRequest = 0x0200,
        GetMetricResponse = 0x0201,
        GetSwitchRequest = 0x0300,
        GetSwitchResponse = 0x0301,
        UpdateSwitchRequest = 0x0400,
        GetRunningParameterRequest = 0x0500,
        GetRunningParameterResponse = 0x0501,
        UpdateRunningParameterRequest = 0x0600,
        GetWorkingModeRequest = 0x0700,
        GetWorkingModeResponse = 0x0700,
        UpdateWorkingModeRequest = 0x0800,
        GetAlarmRequest = 0x0900,
        GetAlarmResponse = 0x0901,
    }
}
