using System;
using System.Collections.Generic;
using System.Linq;
using Google.OrTools.LinearSolver;

namespace TaskAssigner
{
    public class AssignerCore
    {
        private readonly Solver solver_;
        private readonly AssignmentDescription assignment_;
        private readonly int[,] costs_;
        private readonly Variable[,] variables_;

        // TODO(zhangshuai.ds): Pass in 3 policies.
        public AssignerCore(Solver solver, AssignmentDescription assignment)
        {
            this.solver_ = solver ?? throw new ArgumentNullException(nameof(solver));
            this.assignment_ = assignment ?? throw new ArgumentNullException(nameof(assignment));
            this.costs_ = this.ComputeCosts(assignment);
            this.variables_ = this.solver_.MakeBoolVarMatrix(assignment.Nodes.Count, assignment.Tasks.Count);
        }

        public AssignmentDescription Run()
        {
            this.AddObjective();
            this.AddConstraints();

            Solver.ResultStatus status = this.solver_.Solve();
            if (status == Solver.ResultStatus.OPTIMAL)
            {
                var result = new AssignmentDescription();

                foreach (NodeDescription n in this.assignment_.Nodes)
                {
                    result.Nodes.Add(n);
                }

                foreach (TaskDescription t in this.assignment_.Tasks)
                {
                    result.Tasks.Add(t);
                }

                for (int i = 1; i < this.assignment_.Nodes.Count; i++)
                {
                    for (int j = 0; j < this.assignment_.Tasks.Count; j++)
                    {
                        if (this.variables_[i, j].SolutionValue() > 0)
                        {
                            if (!result.Assignments.TryGetValue(i, out ISet<int> tasks))
                            {
                                tasks = new HashSet<int>();
                                result.Assignments.Add(i, tasks);
                            }
                            tasks.Add(j);
                            Console.WriteLine($"Task {j} assigned to node {i} with cost {this.costs_[i, j]}.");
                        }
                    }
                }
                Console.WriteLine($"Total cost is {this.solver_.Objective().Value()}");

                return result;
            }

            throw new NotImplementedException($"Not implemented for status: {status}");
        }

        protected int[,] ComputeCosts(AssignmentDescription assignment)
        {
            int[,] costs = new int[assignment.Nodes.Count, assignment.Tasks.Count];
            for (int j = 0; j < assignment.Tasks.Count; j++)
            {
                costs[0, j] = 500;  // UNASSIGNED cost
                if (assignment.Assignments[0].Contains(j))  // Not assigned yet.
                {
                    for (int i = 1; i < assignment.Nodes.Count; i++)
                    {
                        costs[i, j] = 0;  // No fee for assignment.
                    }
                }
                else
                {
                    for (int i = 1; i < assignment.Nodes.Count; i++)
                    {
                        if (assignment.Assignments[i].Contains(j))
                        {
                            costs[i, j] = 0;  // Don't move.
                        }
                        else
                        {
                            costs[i, j] = 200;  // Moving cost.
                        }
                    }
                }
            }
            return costs;
        }

        protected void AddObjective()
        {
            var movingCosts = new LinearExpr();
            for (int i = 0; i < this.assignment_.Nodes.Count; i++)
            {
                for (int j = 0; j < this.assignment_.Tasks.Count; j++)
                {
                    movingCosts += this.costs_[i, j] * this.variables_[i, j];
                }
            }

            // TODO(zhangshuai.ds): Computes it.
            var imbalanceCosts = new LinearExpr();

            // TODO(zhangshuai.ds): Add coefficients for the two costs.
            this.solver_.Minimize(movingCosts + imbalanceCosts);
        }

        protected void AddConstraints()
        {
            // Each task is assigned to exactly one node (including UNASSIGNED virtual node).
            for (int j = 0; j < this.assignment_.Tasks.Count; j++)
            {
                LinearExpr expr = Enumerable.Range(0, this.assignment_.Nodes.Count)
                    .Select(i => this.variables_[i, j])
                    .ToArray()
                    .Sum();
                this.solver_.Add(expr == 1);
            }

            // Each worker cannot exceed its capacity.
            for (int i = 0; i < this.assignment_.Nodes.Count; i++)
            {
                var cpuCores = new LinearExpr();
                var memoryMiB = new LinearExpr();

                for (int j = 0; j < this.assignment_.Tasks.Count; j++)
                {
                    cpuCores += this.variables_[i, j] * this.assignment_.Tasks[j].CpuCores;
                    memoryMiB += this.variables_[i, j] * this.assignment_.Tasks[j].MemoryMiB;
                }

                this.solver_.Add(cpuCores <= this.assignment_.Nodes[i].CpuCores);
                this.solver_.Add(memoryMiB <= this.assignment_.Nodes[i].MemoryMiB);
            }
        }
    }
}
