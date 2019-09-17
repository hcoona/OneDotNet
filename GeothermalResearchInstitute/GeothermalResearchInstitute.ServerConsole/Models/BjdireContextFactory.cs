// <copyright file="BjdireContextFactory.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GeothermalResearchInstitute.ServerConsole.Models
{
    public class BjdireContextFactory : IDesignTimeDbContextFactory<BjdireContext>
    {
        public BjdireContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<BjdireContext>();
            Program.DbContextOptionsBuilderAction.Invoke(builder);
            return new BjdireContext(builder.Options);
        }
    }
}
