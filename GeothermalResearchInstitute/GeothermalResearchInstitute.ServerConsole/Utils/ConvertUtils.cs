// <copyright file="ConvertUtils.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using Google.Protobuf.WellKnownTypes;
using GrpcMetric = GeothermalResearchInstitute.v2.Metric;
using ModelMetric = GeothermalResearchInstitute.ServerConsole.Models.Metric;

namespace GeothermalResearchInstitute.ServerConsole.Utils
{
    internal static class ConvertUtils
    {
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
    }
}
