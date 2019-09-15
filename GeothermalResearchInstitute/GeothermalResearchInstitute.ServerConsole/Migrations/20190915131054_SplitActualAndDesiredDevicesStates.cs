// <copyright file="20190915131054_SplitActualAndDesiredDevicesStates.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GeothermalResearchInstitute.ServerConsole.Migrations
{
    public partial class SplitActualAndDesiredDevicesStates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder is null)
            {
                throw new ArgumentNullException(nameof(migrationBuilder));
            }

            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.CreateTable(
                name: "DevicesActualStates",
                columns: table => new
                {
                    Id = table.Column<byte[]>(nullable: false),
                    WorkingMode = table.Column<int>(nullable: false),
                    SummerTemperature = table.Column<float>(nullable: false),
                    WinterTemperature = table.Column<float>(nullable: false),
                    WarmCapacity = table.Column<float>(nullable: false),
                    ColdCapacity = table.Column<float>(nullable: false),
                    FlowCapacity = table.Column<float>(nullable: false),
                    RateCapacity = table.Column<float>(nullable: false),
                    MotorMode = table.Column<int>(nullable: false),
                    WaterPumpMode = table.Column<int>(nullable: false),
                    DevicePower = table.Column<bool>(nullable: false),
                    ExhaustPower = table.Column<bool>(nullable: false),
                    HeatPumpAuto = table.Column<bool>(nullable: false),
                    HeatPumpPower = table.Column<bool>(nullable: false),
                    HeatPumpFanOn = table.Column<bool>(nullable: false),
                    HeatPumpCompressorOn = table.Column<bool>(nullable: false),
                    HeatPumpFourWayReversingValue = table.Column<bool>(nullable: false),
                    IPAddress = table.Column<byte[]>(nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DevicesActualStates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DevicesDesiredStates",
                columns: table => new
                {
                    Id = table.Column<byte[]>(nullable: false),
                    WorkingMode = table.Column<int>(nullable: false),
                    SummerTemperature = table.Column<float>(nullable: false),
                    WinterTemperature = table.Column<float>(nullable: false),
                    WarmCapacity = table.Column<float>(nullable: false),
                    ColdCapacity = table.Column<float>(nullable: false),
                    FlowCapacity = table.Column<float>(nullable: false),
                    RateCapacity = table.Column<float>(nullable: false),
                    MotorMode = table.Column<int>(nullable: false),
                    WaterPumpMode = table.Column<int>(nullable: false),
                    DevicePower = table.Column<bool>(nullable: false),
                    ExhaustPower = table.Column<bool>(nullable: false),
                    HeatPumpAuto = table.Column<bool>(nullable: false),
                    HeatPumpPower = table.Column<bool>(nullable: false),
                    HeatPumpFanOn = table.Column<bool>(nullable: false),
                    HeatPumpCompressorOn = table.Column<bool>(nullable: false),
                    HeatPumpFourWayReversingValue = table.Column<bool>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DevicesDesiredStates", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder is null)
            {
                throw new ArgumentNullException(nameof(migrationBuilder));
            }

            migrationBuilder.DropTable(
                name: "DevicesActualStates");

            migrationBuilder.DropTable(
                name: "DevicesDesiredStates");

            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<byte[]>(nullable: false),
                    ColdCapacity = table.Column<float>(nullable: false),
                    DevicePower = table.Column<bool>(nullable: false),
                    ExhaustPower = table.Column<bool>(nullable: false),
                    FlowCapacity = table.Column<float>(nullable: false),
                    HeatPumpAuto = table.Column<bool>(nullable: false),
                    HeatPumpCompressorOn = table.Column<bool>(nullable: false),
                    HeatPumpFanOn = table.Column<bool>(nullable: false),
                    HeatPumpFourWayReversingValue = table.Column<bool>(nullable: false),
                    HeatPumpPower = table.Column<bool>(nullable: false),
                    MotorMode = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    RateCapacity = table.Column<float>(nullable: false),
                    SummerTemperature = table.Column<float>(nullable: false),
                    WarmCapacity = table.Column<float>(nullable: false),
                    WaterPumpMode = table.Column<int>(nullable: false),
                    WinterTemperature = table.Column<float>(nullable: false),
                    WorkingMode = table.Column<int>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                });
        }
    }
}
