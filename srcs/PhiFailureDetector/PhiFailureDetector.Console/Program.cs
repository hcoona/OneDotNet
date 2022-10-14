// <copyright file="Program.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Text;

namespace PhiFailureDetector.ConsoleApp
{
    internal class Program
    {
        private const long ToNano = 1000000L;
        private const long Last = 555 * ToNano;

        public static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("===== Normal Distribution =====");

            var arrivalWindow = new LongIntervalHistory(1000);
            var mean = TimeSpan.FromSeconds(1).TotalMilliseconds;
            var stdDeviation = mean / 4;
            arrivalWindow.Enqueue((long)(mean - stdDeviation));
            arrivalWindow.Enqueue((long)(mean + stdDeviation));
#if false
            arrivalWindow.Enqueue(1000);
#endif
            Console.WriteLine(Normal(1000, arrivalWindow));
            Console.WriteLine(Normal(2000, arrivalWindow));
            Console.WriteLine(Normal(3000, arrivalWindow));

#if false
            PrintPhi(666, last, arrivalWindow, PhiFailureDetector.Normal);
            PrintPhi(777, last, arrivalWindow, PhiFailureDetector.Normal);
            PrintPhi(888, last, arrivalWindow, PhiFailureDetector.Normal);
            PrintPhi(999, last, arrivalWindow, PhiFailureDetector.Normal);
            PrintPhi(1100, last, arrivalWindow, PhiFailureDetector.Normal);
            PrintPhi(1200, last, arrivalWindow, PhiFailureDetector.Normal);
#endif

            Console.WriteLine();

            Console.WriteLine("===== Exponential Distribution =====");

            arrivalWindow = new LongIntervalHistory(4);
            arrivalWindow.Enqueue(111 * ToNano);
            arrivalWindow.Enqueue(111 * ToNano);
            arrivalWindow.Enqueue(111 * ToNano);
            arrivalWindow.Enqueue(111 * ToNano);

            PrintPhi(666, Last, arrivalWindow, PhiFailureDetector.Exponential);
            PrintPhi(777, Last, arrivalWindow, PhiFailureDetector.Exponential);
            PrintPhi(888, Last, arrivalWindow, PhiFailureDetector.Exponential);
            PrintPhi(999, Last, arrivalWindow, PhiFailureDetector.Exponential);
            PrintPhi(1100, Last, arrivalWindow, PhiFailureDetector.Exponential);
            PrintPhi(1200, Last, arrivalWindow, PhiFailureDetector.Exponential);
            PrintPhi(3000, Last, arrivalWindow, PhiFailureDetector.Exponential);

            Console.WriteLine();
        }

        private static void PrintPhi(int nowTimestamp, long lastTimestamp, LongIntervalHistory arrivalWindow, PhiFailureDetector.PhiFunc phiFunc)
        {
            Console.WriteLine("Phi({0}) = {1:F3}", nowTimestamp, phiFunc(nowTimestamp * ToNano, lastTimestamp, arrivalWindow));
        }

        private static double CDF(double phi)
        {
            return 1 - Math.Pow(10, -phi);
        }

        private static double Normal(long duration, dynamic intervalHistory)
        {
            var y = (duration - intervalHistory.Avg) / intervalHistory.StdDeviation;
            var exp = Math.Exp(-y * (1.5976 + (0.070566 * y * y)));
            if (duration > intervalHistory.Avg)
            {
                return -Math.Log10(exp / (1 + exp));
            }
            else
            {
                return -Math.Log10(1 - (1 / (1 + exp)));
            }
        }
    }
}
