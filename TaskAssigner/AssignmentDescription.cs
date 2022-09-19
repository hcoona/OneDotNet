// <copyright file="AssignmentDescription.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace TaskAssigner
{
    public class AssignmentDescription
    {
        // Nodes[0] means UNASSIGNED
        public IList<NodeDescription> Nodes { get; } = new List<NodeDescription>();

        public IList<TaskDescription> Tasks { get; } = new List<TaskDescription>();

        public IDictionary<int /* nodeIndex */, ISet<int /* taskIndex */>> Assignments { get; } =
            new Dictionary<int, ISet<int>>();
    }
}

