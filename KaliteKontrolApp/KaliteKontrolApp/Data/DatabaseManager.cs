using System;
using System.Collections.Generic;
using KaliteKontrolApp.Models;

namespace KaliteKontrolApp.Data
{
    public class DatabaseManager
    {
        private static DatabaseManager? _instance;
        public static DatabaseManager Instance => _instance ??= new DatabaseManager();

        private DatabaseManager() { }

        public Statistics GetStatistics()
        {
            // TODO: Gerçek veritabanı sorgusu eklenecek
            return new Statistics
            {
                TotalPlans = 0,
                TotalMeasurements = 0,
                OkProducts = 0,
                NokProducts = 0,
                ConditionalProducts = 0
            };
        }

        public List<MonthlyData> GetMonthlyStatistics()
        {
            return new List<MonthlyData>();
        }

        public List<object> GetAllPlans()
        {
            return new List<object>();
        }

        public List<object> GetAllMeasurements()
        {
            return new List<object>();
        }
    }
}
