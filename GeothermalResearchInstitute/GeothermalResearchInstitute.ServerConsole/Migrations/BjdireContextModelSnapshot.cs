// <auto-generated />
using GeothermalResearchInstitute.ServerConsole.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GeothermalResearchInstitute.ServerConsole.Migrations
{
    [DbContext(typeof(BjdireContext))]
    partial class BjdireContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.0");

            modelBuilder.Entity("GeothermalResearchInstitute.ServerConsole.Models.Alarm", b =>
                {
                    b.Property<string>("DeviceId")
                        .HasColumnType("TEXT");

                    b.Property<long>("Timestamp")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("ElectricalHeaterBroken")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("EmergencyStopped")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("HeaterOverloadedBroken")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("HighHeaterPressure")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("HighVoltage")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("LowFlowRate")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("LowHeaterPressure")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("LowVoltage")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("NoPower")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("NoWater")
                        .HasColumnType("INTEGER");

                    b.HasKey("DeviceId", "Timestamp");

                    b.ToTable("Alarms");
                });

            modelBuilder.Entity("GeothermalResearchInstitute.ServerConsole.Models.AlarmChange", b =>
                {
                    b.Property<string>("DeviceId")
                        .HasColumnType("TEXT");

                    b.Property<long>("Timestamp")
                        .HasColumnType("INTEGER");

                    b.Property<byte>("Type")
                        .HasColumnType("INTEGER");

                    b.Property<byte>("Direction")
                        .HasColumnType("INTEGER");

                    b.HasKey("DeviceId", "Timestamp", "Type");

                    b.ToTable("AlarmChanges");
                });

            modelBuilder.Entity("GeothermalResearchInstitute.ServerConsole.Models.Metric", b =>
                {
                    b.Property<string>("DeviceId")
                        .HasColumnType("TEXT");

                    b.Property<long>("Timestamp")
                        .HasColumnType("INTEGER");

                    b.Property<float>("EnvironmentCelsiusDegree")
                        .HasColumnType("REAL");

                    b.Property<float>("HeaterOutputWaterCelsiusDegree")
                        .HasColumnType("REAL");

                    b.Property<float>("HeaterPowerKilowatt")
                        .HasColumnType("REAL");

                    b.Property<float>("InputWaterCelsiusDegree")
                        .HasColumnType("REAL");

                    b.Property<float>("InputWaterPressureMeter")
                        .HasColumnType("REAL");

                    b.Property<float>("OutputWaterCelsiusDegree")
                        .HasColumnType("REAL");

                    b.Property<float>("OutputWaterPressureMeter")
                        .HasColumnType("REAL");

                    b.Property<float>("WaterPumpFlowRateCubicMeterPerHour")
                        .HasColumnType("REAL");

                    b.HasKey("DeviceId", "Timestamp");

                    b.ToTable("Metrics");
                });
#pragma warning restore 612, 618
        }
    }
}
