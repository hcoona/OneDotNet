// <copyright file="Assigner.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Google.OrTools.LinearSolver;

namespace TaskAssigner
{
    public class Assigner
    {
        private readonly Solver solver_;

        public Assigner(Solver solver)
        {
            this.solver_ = solver ?? throw new ArgumentNullException(nameof(solver));
        }

        public AssignmentDescription Solve(AssignmentDescription assignment)
        {
            if (assignment is null)
            {
                throw new ArgumentNullException(nameof(assignment));
            }

            var assignerCore = new AssignerCore(this.solver_, assignment);
            return assignerCore.Run();
        }
    }
}

