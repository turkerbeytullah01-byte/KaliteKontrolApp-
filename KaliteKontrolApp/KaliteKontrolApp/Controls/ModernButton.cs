using System.Drawing;
using System.Windows.Forms;

namespace KaliteKontrolApp.Controls
{
    public class ModernButton : Button
    {
        public Color ButtonColor { get; set; } = Color.FromArgb(67, 97, 238);
        
        public ModernButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 10, FontStyle.Regular);
            Cursor = Cursors.Hand;
        }
    }

    public class ModernOutlineButton : Button
    {
        public Color BorderColor { get; set; } = Color.FromArgb(67, 97, 238);
        
        public ModernOutlineButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 1;
            BackColor = Color.White;
            ForeColor = BorderColor;
            Font = new Font("Segoe UI", 10, FontStyle.Regular);
            Cursor = Cursors.Hand;
        }
    }
}
