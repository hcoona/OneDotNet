// <copyright file="BjdireContext.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;

namespace GeothermalResearchInstitute.ServerConsole.Model
{
    public class BjdireContext : DbContext
    {
        public BjdireContext(DbContextOptions<BjdireContext> options)
            : base(options)
        {
        }

        public DbSet<DeviceActualStates> DevicesActualStates { get; set; }

        public DbSet<DeviceDesiredStates> DevicesDesiredStates { get; set; }
    }
}
