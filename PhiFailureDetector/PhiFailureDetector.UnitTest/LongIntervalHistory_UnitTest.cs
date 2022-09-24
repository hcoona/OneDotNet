// <copyright file="LongIntervalHistory_UnitTest.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhiFailureDetector.UnitTest
{
    [TestClass]
    public class LongIntervalHistory_UnitTest
    {
        [TestMethod]
        public void Test_Baseline()
        {
            var arrivalWindow = new LongIntervalHistory(5);

            arrivalWindow.Enqueue(1);
            arrivalWindow.Enqueue(2);
            arrivalWindow.Enqueue(3);
            arrivalWindow.Enqueue(4);
            arrivalWindow.Enqueue(5);
            arrivalWindow.Enqueue(6);
            arrivalWindow.Enqueue(7);

            var queueArray = arrivalWindow.ToArray();
            Array.Sort(queueArray);

            CollectionAssert.AreEqual(
                new long[] { 3, 4, 5, 6, 7 },
                queueArray);

            Assert.AreEqual(25, arrivalWindow.Sum);
            Assert.AreEqual(5, arrivalWindow.Avg);
        }
    }
}
