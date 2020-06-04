using Google.OrTools.LinearSolver;
using System;

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
