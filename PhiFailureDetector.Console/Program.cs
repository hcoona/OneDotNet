using System;
using System.Linq;
using System.Text;

namespace PhiFailureDetector.ConsoleApp
{
    class Program
    {
        const long toNano = 1000000L;
        const long last = 555 * toNano;

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("===== Normal Distribution =====");

            var arrivalWindow = new LongIntervalHistory(1000);
            var mean = TimeSpan.FromSeconds(1).TotalMilliseconds;
            var stdDeviation = mean / 4;
            arrivalWindow.Enqueue((long)(mean - stdDeviation));
            arrivalWindow.Enqueue((long)(mean + stdDeviation));
            //arrivalWindow.Enqueue(1000);
            Console.WriteLine(Normal(1000, arrivalWindow));
            Console.WriteLine(Normal(2000, arrivalWindow));
            Console.WriteLine(Normal(3000, arrivalWindow));

            //PrintPhi(666, last, arrivalWindow, PhiFailureDetector.Normal);
            //PrintPhi(777, last, arrivalWindow, PhiFailureDetector.Normal);
            //PrintPhi(888, last, arrivalWindow, PhiFailureDetector.Normal);
            //PrintPhi(999, last, arrivalWindow, PhiFailureDetector.Normal);
            //PrintPhi(1100, last, arrivalWindow, PhiFailureDetector.Normal);
            //PrintPhi(1200, last, arrivalWindow, PhiFailureDetector.Normal);

            Console.WriteLine();


            Console.WriteLine("===== Exponential Distribution =====");

            arrivalWindow = new LongIntervalHistory(4);
            arrivalWindow.Enqueue(111 * toNano);
            arrivalWindow.Enqueue(111 * toNano);
            arrivalWindow.Enqueue(111 * toNano);
            arrivalWindow.Enqueue(111 * toNano);

            PrintPhi(666, last, arrivalWindow, PhiFailureDetector.Exponential);
            PrintPhi(777, last, arrivalWindow, PhiFailureDetector.Exponential);
            PrintPhi(888, last, arrivalWindow, PhiFailureDetector.Exponential);
            PrintPhi(999, last, arrivalWindow, PhiFailureDetector.Exponential);
            PrintPhi(1100, last, arrivalWindow, PhiFailureDetector.Exponential);
            PrintPhi(1200, last, arrivalWindow, PhiFailureDetector.Exponential);
            PrintPhi(3000, last, arrivalWindow, PhiFailureDetector.Exponential);

            Console.WriteLine();
        }

        private static void PrintPhi(int nowTimestamp, long lastTimestamp, LongIntervalHistory arrivalWindow, PhiFailureDetector.PhiFunc phiFunc)
        {
            Console.WriteLine("Phi({0}) = {1:F3}", nowTimestamp, phiFunc(nowTimestamp * toNano, lastTimestamp, arrivalWindow));
        }

        private static double CDF(double phi)
        {
            return 1 - Math.Pow(10, -phi);
        }

        private static double Normal(long duration, dynamic intervalHistory)
        {
            var y = (duration - intervalHistory.Avg) / intervalHistory.StdDeviation;
            var exp = Math.Exp(-y * (1.5976 + 0.070566 * y * y));
            if (duration > intervalHistory.Avg)
            {
                return -Math.Log10(exp / (1 + exp));
            }
            else
            {
                return -Math.Log10(1 - 1 / (1 + exp));
            }
        }
    }
}