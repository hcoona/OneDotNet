// <copyright file="Program.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using Google.OrTools.LinearSolver;

namespace TaskAssigner
{
    internal class Program
    {
        private static void Main()
        {
            using var solver = Solver.CreateSolver(
                "CBC_MIXED_INTEGER_PROGRAMMING");

            var assigner = new Assigner(solver);

            var assignment = new AssignmentDescription();
            assignment.Nodes.Add(new NodeDescription
            {
                Id = 0,
                CpuCores = int.MaxValue,
                MemoryMiB = int.MaxValue,
            });
            for (int i = 1; i <= 3; i++)
            {
                assignment.Nodes.Add(new NodeDescription
                {
                    Id = i,
                    CpuCores = 48,
                    MemoryMiB = 64 << 10,  // 64 GiB
                });
            }

            for (int i = 0; i < 13; i++)
            {
                assignment.Tasks.Add(new TaskDescription
                {
                    Id = i,
                    CpuCores = 8,
                    MemoryMiB = 32 << 10,  // 32 GiB, CPU:MEM=1:4
                });
            }

            for (int i = 0; i < 17; i++)
            {
                assignment.Tasks.Add(new TaskDescription
                {
                    Id = 13 + i,
                    CpuCores = 16,
                    MemoryMiB = 32 << 10,  // 16 GiB, CPU:MEM=1:2
                });
            }

            assignment.Assignments[0] = new HashSet<int>();
            for (int i = 0; i < assignment.Tasks.Count; i++)
            {
                assignment.Assignments[0].Add(i);
            }

            for (int i = 1; i < assignment.Nodes.Count; i++)
            {
                assignment.Assignments[i] = new HashSet<int>();
            }

            assignment = assigner.Solve(assignment);
        }
    }
}
