// <copyright file="20191215142314_Add4MoreAlarmTypes.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore.Migrations;

namespace GeothermalResearchInstitute.ServerConsole.Migrations
{
    public partial class Add4MoreAlarmTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder is null)
            {
                throw new System.ArgumentNullException(nameof(migrationBuilder));
            }

            migrationBuilder.AddColumn<bool>(
                name: "EmergencyStopped",
                table: "Alarms",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HighVoltage",
                table: "Alarms",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LowVoltage",
                table: "Alarms",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NoWater",
                table: "Alarms",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder is null)
            {
                throw new System.ArgumentNullException(nameof(migrationBuilder));
            }

            migrationBuilder.DropColumn(
                name: "EmergencyStopped",
                table: "Alarms");

            migrationBuilder.DropColumn(
                name: "HighVoltage",
                table: "Alarms");

            migrationBuilder.DropColumn(
                name: "LowVoltage",
                table: "Alarms");

            migrationBuilder.DropColumn(
                name: "NoWater",
                table: "Alarms");
        }
    }
}
