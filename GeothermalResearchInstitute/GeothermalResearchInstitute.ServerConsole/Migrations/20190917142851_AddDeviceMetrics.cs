// <copyright file="20190917142851_AddDeviceMetrics.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GeothermalResearchInstitute.ServerConsole.Migrations
{
    public partial class AddDeviceMetrics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder is null)
            {
                throw new ArgumentNullException(nameof(migrationBuilder));
            }

            migrationBuilder.CreateTable(
                name: "DevicesMetrics",
                columns: table => new
                {
                    Id = table.Column<byte[]>(nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(nullable: false),
                    WaterOutTemperature = table.Column<float>(nullable: false),
                    WaterInTemperature = table.Column<float>(nullable: false),
                    HeaterWaterOutTemperature = table.Column<float>(nullable: false),
                    EnvironmentTemperature = table.Column<float>(nullable: false),
                    WaterOutPressure = table.Column<float>(nullable: false),
                    WaterInPressure = table.Column<float>(nullable: false),
                    HeaterPower = table.Column<float>(nullable: false),
                    FlowCapacity = table.Column<float>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DevicesMetrics", x => new { x.Id, x.Timestamp });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder is null)
            {
                throw new ArgumentNullException(nameof(migrationBuilder));
            }

            migrationBuilder.DropTable(
                name: "DevicesMetrics");
        }
    }
}
