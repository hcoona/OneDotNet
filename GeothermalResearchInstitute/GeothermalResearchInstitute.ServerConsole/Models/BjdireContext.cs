// <copyright file="BjdireContext.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;

namespace GeothermalResearchInstitute.ServerConsole.Models
{
    public class BjdireContext : DbContext
    {
        public BjdireContext(DbContextOptions<BjdireContext> options)
            : base(options)
        {
        }

        public DbSet<Alarm> Alarms { get; set; }

        public DbSet<Metric> Metrics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder is null)
            {
                throw new System.ArgumentNullException(nameof(modelBuilder));
            }

            modelBuilder.Entity<Alarm>()
                .HasKey(m => new { m.DeviceId, m.Timestamp });
            modelBuilder.Entity<Metric>()
                .HasKey(m => new { m.DeviceId, m.Timestamp });
        }
    }
}
