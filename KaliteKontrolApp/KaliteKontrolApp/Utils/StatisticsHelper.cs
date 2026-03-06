using System;
using System.Collections.Generic;
using System.Linq;

namespace KaliteKontrolApp.Utils
{
    public static class StatisticsHelper
    {
        public static double Mean(IEnumerable<double> data)
        {
            var list = data.ToList();
            if (list.Count == 0) return 0;
            return list.Average();
        }

        public static double StandardDeviation(IEnumerable<double> data)
        {
            var list = data.ToList();
            if (list.Count < 2) return 0;
            
            double avg = list.Average();
            double sumOfSquares = list.Sum(x => (x - avg) * (x - avg));
            return Math.Sqrt(sumOfSquares / (list.Count - 1));
        }

        public static double Cp(IEnumerable<double> data, double usl, double lsl)
        {
            double stdDev = StandardDeviation(data);
            if (stdDev == 0) return 0;
            return (usl - lsl) / (6 * stdDev);
        }

        public static double Cpk(IEnumerable<double> data, double usl, double lsl)
        {
            double mean = Mean(data);
            double stdDev = StandardDeviation(data);
            if (stdDev == 0) return 0;
            
            double cpu = (usl - mean) / (3 * stdDev);
            double cpl = (mean - lsl) / (3 * stdDev);
            return Math.Min(cpu, cpl);
        }
    }
}
