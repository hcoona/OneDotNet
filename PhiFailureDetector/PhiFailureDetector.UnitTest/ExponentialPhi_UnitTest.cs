// <copyright file="ExponentialPhi_UnitTest.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhiFailureDetector.UnitTest
{
    [TestClass]
    public class ExponentialPhi_UnitTest
    {
        [TestMethod]
        public void Test_CassandraCase()
        {
            const long toNano = 1000000L;

            var intervalHistory = new LongIntervalHistory(4);
            intervalHistory.Enqueue(111 * toNano);
            intervalHistory.Enqueue(111 * toNano);
            intervalHistory.Enqueue(111 * toNano);
            intervalHistory.Enqueue(111 * toNano);
            intervalHistory.Enqueue(111 * toNano);

            Assert.AreEqual(1.0, PhiFailureDetector.Exponential(666 * toNano, 555 * toNano, intervalHistory));
            Assert.AreEqual(22.03, PhiFailureDetector.Exponential(3000 * toNano, 555 * toNano, intervalHistory), 0.01);
        }
    }
}
