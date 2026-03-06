using System;
using System.IO;
using System.Windows.Forms;
using KaliteKontrolApp.Forms;  // EKLENDİ: MainForm için

namespace KaliteKontrolApp
{
    static class Program
    {
        public static string DatabasePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "KaliteKontrolApp",
            "database.db");
        
        public static string AppVersion = "1.0.0";  // EKLENDİ

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Veritabanı klasörünü oluştur
            var dbDir = Path.GetDirectoryName(DatabasePath);
            if (!string.IsNullOrEmpty(dbDir) && !Directory.Exists(dbDir))
                Directory.CreateDirectory(dbDir);
            
            Application.Run(new MainForm());
        }
    }
}
