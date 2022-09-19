// <copyright file="TaskDescription.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace TaskAssigner
{
    public class TaskDescription
    {
        public int Id { get; set; }

        public int CpuCores { get; set; }

        public int MemoryMiB { get; set; }
    }
}

