// <copyright file="RandomUtils.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Threading;

namespace GeothermalResearchInstitute.Wpf.Common
{
    public static class RandomUtils
    {
        private static readonly ThreadLocal<Random> ThreadLocalRandom = new ThreadLocal<Random>(() => new Random());

        public static double NextDouble()
        {
            return ThreadLocalRandom.Value.NextDouble();
        }

        public static double NextDouble(double startInclusive, double endExclusive)
        {
            return startInclusive + (ThreadLocalRandom.Value.NextDouble() * (endExclusive - startInclusive));
        }

        public static float NextFloat(float startInclusive, float endExclusive)
            => (float)NextDouble(startInclusive, endExclusive);
    }
}
