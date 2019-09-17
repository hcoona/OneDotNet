// <copyright file="BjdireContext.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace GeothermalResearchInstitute.ServerConsole.Models
{
    [SuppressMessage("Design", "CA1062:验证公共方法的参数", Justification = "Guaranteed by EF Core.")]
    public class BjdireContext : DbContext
    {
        public BjdireContext(DbContextOptions<BjdireContext> options)
            : base(options)
        {
        }

        public DbSet<DeviceActualStates> DevicesActualStates { get; set; }

        public DbSet<DeviceDesiredStates> DevicesDesiredStates { get; set; }

        public DbSet<DeviceMetrics> DevicesMetrics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DeviceMetrics>()
                .HasKey(m => new { m.Id, m.Timestamp });
        }
    }
}
