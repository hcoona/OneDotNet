// <copyright file="20191212150337_InitialCreate.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GeothermalResearchInstitute.ServerConsole.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder is null)
            {
                throw new ArgumentNullException(nameof(migrationBuilder));
            }

            migrationBuilder.CreateTable(
                name: "Metrics",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(nullable: false),
                    OutputWaterCelsiusDegree = table.Column<float>(nullable: false),
                    InputWaterCelsiusDegree = table.Column<float>(nullable: false),
                    HeaterOutputWaterCelsiusDegree = table.Column<float>(nullable: false),
                    EnvironmentCelsiusDegree = table.Column<float>(nullable: false),
                    OutputWaterPressureMeter = table.Column<float>(nullable: false),
                    InputWaterPressureMeter = table.Column<float>(nullable: false),
                    HeaterPowerKilowatt = table.Column<float>(nullable: false),
                    WaterPumpFlowRateCubicMeterPerHour = table.Column<float>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Metrics", x => new { x.Id, x.Timestamp });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder is null)
            {
                throw new ArgumentNullException(nameof(migrationBuilder));
            }

            migrationBuilder.DropTable(
                name: "Metrics");
        }
    }
}
