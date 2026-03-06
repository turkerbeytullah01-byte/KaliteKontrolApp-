using System;
using System.Collections.Generic;
using System.Linq;

namespace KaliteKontrolApp.Utils
{
    public static class Statistics
    {
        /// <summary>
        /// Aritmetik ortalama hesaplar
        /// </summary>
        public static double Mean(IEnumerable<double> data)
        {
            var list = data.ToList();
            if (list.Count == 0) return 0;
            return list.Average();
        }

        /// <summary>
        /// Standart sapma hesaplar (n-1 ile bölünür - örneklem)
        /// </summary>
        public static double StandardDeviation(IEnumerable<double> data)
        {
            var list = data.ToList();
            if (list.Count < 2) return 0;
            
            double avg = list.Average();
            double sumOfSquares = list.Sum(x => (x - avg) * (x - avg));
            return Math.Sqrt(sumOfSquares / (list.Count - 1));
        }

        /// <summary>
        /// Standart sapma (n ile bölünür - popülasyon)
        /// </summary>
        public static double PopulationStandardDeviation(IEnumerable<double> data)
        {
            var list = data.ToList();
            if (list.Count == 0) return 0;
            
            double avg = list.Average();
            double sumOfSquares = list.Sum(x => (x - avg) * (x - avg));
            return Math.Sqrt(sumOfSquares / list.Count);
        }

        /// <summary>
        /// Varyans hesaplar
        /// </summary>
        public static double Variance(IEnumerable<double> data)
        {
            var list = data.ToList();
            if (list.Count < 2) return 0;
            
            double avg = list.Average();
            return list.Sum(x => (x - avg) * (x - avg)) / (list.Count - 1);
        }

        /// <summary>
        /// Minimum değer
        /// </summary>
        public static double Min(IEnumerable<double> data)
        {
            return data.Any() ? data.Min() : 0;
        }

        /// <summary>
        /// Maksimum değer
        /// </summary>
        public static double Max(IEnumerable<double> data)
        {
            return data.Any() ? data.Max() : 0;
        }

        /// <summary>
        /// Medyan (ortanca değer)
        /// </summary>
        public static double Median(IEnumerable<double> data)
        {
            var list = data.OrderBy(x => x).ToList();
            if (list.Count == 0) return 0;
            
            int mid = list.Count / 2;
            if (list.Count % 2 == 0)
                return (list[mid - 1] + list[mid]) / 2.0;
            else
                return list[mid];
        }

        /// <summary>
        /// Cp (Process Capability) hesaplar - Kalite kontrol için
        /// USL: Üst Spesifikasyon Limiti, LSL: Alt Spesifikasyon Limiti
        /// </summary>
        public static double Cp(IEnumerable<double> data, double usl, double lsl)
        {
            double stdDev = StandardDeviation(data);
            if (stdDev == 0) return 0;
            return (usl - lsl) / (6 * stdDev);
        }

        /// <summary>
        /// Cpk (Process Capability Index) hesaplar - Kalite kontrol için
        /// </summary>
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
