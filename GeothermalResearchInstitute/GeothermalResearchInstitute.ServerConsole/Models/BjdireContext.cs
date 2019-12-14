// <copyright file="BjdireContext.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GeothermalResearchInstitute.v2;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GeothermalResearchInstitute.ServerConsole.Models
{
    public class BjdireContext : DbContext
    {
        public BjdireContext(DbContextOptions<BjdireContext> options)
            : base(options)
        {
        }

        public DbSet<Alarm> Alarms { get; set; }

        public DbSet<AlarmChange> AlarmChanges { get; set; }

        public DbSet<Metric> Metrics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder is null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            modelBuilder.Entity<Alarm>()
                .HasKey(m => new { m.DeviceId, m.Timestamp });
            modelBuilder.Entity<AlarmChange>()
                .HasKey(m => new { m.DeviceId, m.Timestamp, m.Type });
            modelBuilder.Entity<AlarmChange>()
                .Property(m => m.Type)
                .HasConversion(new EnumToNumberConverter<AlarmType, byte>());
            modelBuilder.Entity<AlarmChange>()
                .Property(m => m.Direction)
                .HasConversion(new EnumToNumberConverter<AlarmChangeDirection, byte>());
            modelBuilder.Entity<Metric>()
                .HasKey(m => new { m.DeviceId, m.Timestamp });

            if (this.Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
            {
                // SQLite does not have proper support for DateTimeOffset via Entity Framework Core, see the limitations
                // here: https://docs.microsoft.com/en-us/ef/core/providers/sqlite/limitations#query-limitations
                // To work around this, when the SQLite database provider is used, all model properties of type DateTimeOffset
                // use the DateTimeOffsetToBinaryConverter
                // Based on: https://github.com/aspnet/EntityFrameworkCore/issues/10784#issuecomment-415769754
                // This only supports millisecond precision, but should be sufficient for most use cases.
                foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
                {
                    IEnumerable<PropertyInfo> properties = entityType.ClrType.GetProperties()
                        .Where(p => p.PropertyType == typeof(DateTimeOffset));
                    foreach (PropertyInfo property in properties)
                    {
                        modelBuilder
                            .Entity(entityType.Name)
                            .Property(property.Name)
                            .HasConversion(new DateTimeOffsetToBinaryConverter());
                    }
                }
            }
        }
    }
}
