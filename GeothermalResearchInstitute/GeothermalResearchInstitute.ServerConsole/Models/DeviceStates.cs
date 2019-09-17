// <copyright file="DeviceStates.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using GeothermalResearchInstitute.v1;

namespace GeothermalResearchInstitute.ServerConsole.Models
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Performance", "CA1819:属性不应返回数组", Justification = "Disable for DTO.")]
    public class DeviceStates : IEquatable<DeviceStates>
    {
        [Key]
        public byte[] Id { get; set; }

        public DeviceWorkingMode WorkingMode { get; set; }

        public float SummerTemperature { get; set; }

        public float WinterTemperature { get; set; }

        public float WarmCapacity { get; set; }

        public float ColdCapacity { get; set; }

        public float FlowCapacity { get; set; }

        public float RateCapacity { get; set; }

        public MotorMode MotorMode { get; set; }

        public WaterPumpMode WaterPumpMode { get; set; }

        public bool DevicePower { get; set; }

        public bool ExhaustPower { get; set; }

        public bool HeatPumpAuto { get; set; }

        public bool HeatPumpPower { get; set; }

        public bool HeatPumpFanOn { get; set; }

        public bool HeatPumpCompressorOn { get; set; }

        public bool HeatPumpFourWayReversingValue { get; set; }

        public static bool operator ==(DeviceStates lhs, DeviceStates rhs)
        {
            return object.Equals(lhs, rhs);
        }

        public static bool operator !=(DeviceStates lhs, DeviceStates rhs)
        {
            return !object.Equals(lhs, rhs);
        }

        public override bool Equals(object obj)
        {
            if (obj is DeviceStates)
            {
                return this.Equals((DeviceStates)obj);
            }

            return false;
        }

        public bool Equals([AllowNull] DeviceStates other)
        {
            if (other == null)
            {
                return false;
            }

            return (this.Id == other.Id || (this.Id != null && this.Id.SequenceEqual(other.Id))) &&
                typeof(DeviceStates)
                    .GetProperties()
                    .Where(p => p.Name != "Id")
                    .All(pro => pro.GetValue(this).Equals(pro.GetValue(other)));
        }

        public override int GetHashCode()
        {
            int hashCode = typeof(DeviceStates)
                .GetProperties()
                .Aggregate(0, (current, pro) => current ^ pro.GetValue(this).GetHashCode());

            return hashCode.GetHashCode();
        }

        public override string ToString()
        {
            return "[Id=" +
                string.Join("-", this.Id.Select(b => b.ToString("X2", CultureInfo.InvariantCulture))) + "," +
                string.Join(',', typeof(DeviceStates)
                    .GetProperties()
                    .Where(p => p.Name != "Id")
                    .Select(p => $"{p.Name}={p.GetValue(this)}")) + "]";
        }
    }
}
