// <copyright file="ConvertUtils.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using Google.Protobuf.WellKnownTypes;
using GrpcAlarm = GeothermalResearchInstitute.v2.Alarm;
using GrpcMetric = GeothermalResearchInstitute.v2.Metric;
using ModelAlarm = GeothermalResearchInstitute.ServerConsole.Models.Alarm;
using ModelMetric = GeothermalResearchInstitute.ServerConsole.Models.Metric;

namespace GeothermalResearchInstitute.ServerConsole.Utils
{
    internal static class ConvertUtils
    {
        public static GrpcAlarm AssignFrom(this GrpcAlarm gAlarm, ModelAlarm alarm)
        {
            gAlarm.CreateTime = Timestamp.FromDateTimeOffset(alarm.Timestamp);
            gAlarm.LowFlowRate = alarm.LowFlowRate;
            gAlarm.HighHeaterPressure = alarm.HighHeaterPressure;
            gAlarm.LowHeaterPressure = alarm.LowHeaterPressure;
            gAlarm.NoPower = alarm.NoPower;
            gAlarm.HeaterOverloadedBroken = alarm.HeaterOverloadedBroken;
            gAlarm.ElectricalHeaterBorken = alarm.ElectricalHeaterBorken;
            return gAlarm;
        }

        public static GrpcAlarm AssignTo(this GrpcAlarm gAlarm, ModelAlarm alarm)
        {
            alarm.Timestamp = gAlarm.CreateTime.ToDateTimeOffset();
            alarm.LowFlowRate = gAlarm.LowFlowRate;
            alarm.HighHeaterPressure = gAlarm.HighHeaterPressure;
            alarm.LowHeaterPressure = gAlarm.LowHeaterPressure;
            alarm.NoPower = gAlarm.NoPower;
            alarm.HeaterOverloadedBroken = gAlarm.HeaterOverloadedBroken;
            alarm.ElectricalHeaterBorken = gAlarm.ElectricalHeaterBorken;
            return gAlarm;
        }

        public static GrpcMetric AssignFrom(this GrpcMetric gMetric, ModelMetric metric)
        {
            gMetric.CreateTime = Timestamp.FromDateTimeOffset(metric.Timestamp);
            gMetric.OutputWaterCelsiusDegree = metric.OutputWaterCelsiusDegree;
            gMetric.InputWaterCelsiusDegree = metric.InputWaterCelsiusDegree;
            gMetric.HeaterOutputWaterCelsiusDegree = metric.HeaterOutputWaterCelsiusDegree;
            gMetric.EnvironmentCelsiusDegree = metric.EnvironmentCelsiusDegree;
            gMetric.OutputWaterPressureMeter = metric.OutputWaterPressureMeter;
            gMetric.InputWaterPressureMeter = metric.InputWaterPressureMeter;
            gMetric.HeaterPowerKilowatt = metric.HeaterPowerKilowatt;
            gMetric.WaterPumpFlowRateCubicMeterPerHour = metric.WaterPumpFlowRateCubicMeterPerHour;
            return gMetric;
        }

        public static GrpcMetric AssignTo(this GrpcMetric gMetric, ModelMetric metric)
        {
            metric.Timestamp = gMetric.CreateTime.ToDateTimeOffset();
            metric.OutputWaterCelsiusDegree = gMetric.OutputWaterCelsiusDegree;
            metric.InputWaterCelsiusDegree = gMetric.InputWaterCelsiusDegree;
            metric.HeaterOutputWaterCelsiusDegree = gMetric.HeaterOutputWaterCelsiusDegree;
            metric.EnvironmentCelsiusDegree = gMetric.EnvironmentCelsiusDegree;
            metric.OutputWaterPressureMeter = gMetric.OutputWaterPressureMeter;
            metric.InputWaterPressureMeter = gMetric.InputWaterPressureMeter;
            metric.HeaterPowerKilowatt = gMetric.HeaterPowerKilowatt;
            metric.WaterPumpFlowRateCubicMeterPerHour = gMetric.WaterPumpFlowRateCubicMeterPerHour;
            return gMetric;
        }
    }
}
