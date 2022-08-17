using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhiFailureDetector.UnitTest
{
    [TestClass]
    public class NormalPhi_UnitTest
    {
        internal static double CDF(double phi)
        {
            return 1 - Math.Pow(10, -phi);
        }

        internal static double Phi(long interval, double mean, double stdDeviation)
        {
            return PhiFailureDetector.Normal(interval, 0, new FakeWithStatistics
            {
                Avg = mean,
                StdDeviation = stdDeviation
            });
        }

        internal class FakeWithStatistics : IWithStatistics
        {
            public double Avg { get; set; }

            public int Count { get; set; }

            public double StdDeviation { get; set; }

            public long Sum { get; set; }

            public double Variance { get; set; }
        }

        [TestMethod]
        public void Test_CDFProperties()
        {
            Assert.AreEqual(0.5, CDF(Phi(0, 0, 10)), 0.001);
            Assert.AreEqual(0.7257, CDF(Phi(6, 0, 10)), 0.001);
            Assert.AreEqual(0.9332, CDF(Phi(15, 0, 10)), 0.001);
            Assert.AreEqual(0.97725, CDF(Phi(20, 0, 10)), 0.001);
            Assert.AreEqual(0.99379, CDF(Phi(25, 0, 10)), 0.001);
            Assert.AreEqual(0.99977, CDF(Phi(35, 0, 10)), 0.001);
            Assert.AreEqual(0.99997, CDF(Phi(40, 0, 10)), 0.0001);

            foreach (var t in Enumerable.Range(0, 40).Zip(Enumerable.Range(1, 40), Tuple.Create))
            {
                Assert.IsTrue(Phi(t.Item1, 0, 10) < Phi(t.Item2, 0, 10));
            }

            Assert.AreEqual(0.7475, CDF(Phi(22, 20.0, 3)), 0.001);
        }

        [TestMethod]
        public void Test_PhiOutliers()
        {
            Assert.AreEqual(38, Phi(10, 0, 1), 1);
            Assert.AreEqual(0, Phi(-25, 0, 1));
        }

        [TestMethod]
        public void Test_PhiRealisticData()
        {
            var data = new Dictionary<long, double>
            {
                { 0, 0 },
                { 500, 0.1 },
                { 1000, 0.3 },
                { 1200, 1.6 },
                { 1400, 4.7 },
                { 1600, 10.8 },
                { 1700, 15.3 }
            };

            foreach (var p in data)
            {
                Assert.AreEqual(p.Value, Phi(p.Key, 1000, 100), 0.1);
            }

            // larger stdDeviation results => lower phi
            Assert.IsTrue(Phi(1100, 1000, 500) < Phi(1100, 1000, 100));
        }

        [TestMethod]
        public void Test_OnlyOneHeartbeat()
        {
            Assert.AreEqual(0.3, Phi(1000, 1000, 250), 0.2);
            Assert.AreEqual(4.5, Phi(2000, 1000, 250), 0.3);
            Assert.IsTrue(15 < Phi(3000, 1000, 250));
        }
    }
}
