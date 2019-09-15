// <copyright file="BjdireContext.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;

namespace GeothermalResearchInstitute.ServerConsole.Model
{
    public class BjdireContext : DbContext
    {
        public DbSet<Device> Devices { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=bjdire.sqlite");
        }
    }
}
