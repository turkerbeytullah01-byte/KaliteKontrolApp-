using System.Windows.Forms;

namespace KaliteKontrolApp.Forms
{
    public partial class CalculatorForm : Form
    {
        public CalculatorForm()
        {
            InitializeComponent();
        }
        
        private void InitializeComponent()
        {
            Text = "Hesap Makinesi";
            Size = new System.Drawing.Size(400, 500);
        }
    }
}
