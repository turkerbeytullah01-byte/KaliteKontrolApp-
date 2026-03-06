using System;
using System.IO;
using System.Windows.Forms;

namespace KaliteKontrolApp
{
    static class Program
    {
        public static string DatabasePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "KaliteKontrolApp",
            "database.db");

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Veritabanı klasörünü oluştur
            var dbDir = Path.GetDirectoryName(DatabasePath);
            if (!Directory.Exists(dbDir))
                Directory.CreateDirectory(dbDir);
            
            Application.Run(new MainForm());
        }
    }
}
