// <copyright file="IWithStatistics.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace PhiFailureDetector
{
    public interface IWithStatistics
    {
        double Avg { get; }

        int Count { get; }

        double StdDeviation { get; }

        long Sum { get; }

        double Variance { get; }
    }
}

