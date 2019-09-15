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
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<byte[]>(nullable: false),
                    Name = table.Column<string>(nullable: true),
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
                    table.PrimaryKey("PK_Devices", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder is null)
            {
                throw new ArgumentNullException(nameof(migrationBuilder));
            }

            migrationBuilder.DropTable(name: "Devices");
        }
    }
}
