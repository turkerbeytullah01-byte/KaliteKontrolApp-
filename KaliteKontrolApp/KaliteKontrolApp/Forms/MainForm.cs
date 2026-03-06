using KaliteKontrolApp.Controls;
using KaliteKontrolApp.Utils;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace KaliteKontrolApp.Forms;

public partial class MainForm : Form
{
    private Panel _sidebarPanel = null!;
    private Panel _contentPanel = null!;
    private Panel _headerPanel = null!;
    private Label _titleLabel = null!;
    private Label _dateLabel = null!;
    private Button _toggleButton = null!;
    private string _currentPage = "dashboard";
    private bool _sidebarExpanded = true;
    private System.Windows.Forms.Timer _animationTimer = null!;
    private int _targetSidebarWidth;
    private int _currentSidebarWidth;
    private const int SIDEBAR_EXPANDED_WIDTH = 280;
    private const int SIDEBAR_COLLAPSED_WIDTH = 70;
    private const int ANIMATION_DURATION = 15;
    private const int ANIMATION_STEPS = 10;
    private int _animationStep = 0;
    
    public MainForm()
    {
        InitializeComponent();
        InitializeModernUI();
        InitializeAnimation();
    }
    
    private void InitializeComponent()
    {
        Text = "Pro Kalite Kontrol Sistemi";
        Size = new Size(1400, 900);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = ThemeColors.Background;
        Font = Typography.Normal;
        FormBorderStyle = FormBorderStyle.Sizable;
        MinimumSize = new Size(1000, 700);
        DoubleBuffered = true;
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
    }
    
    private void InitializeAnimation()
    {
        _animationTimer = new System.Windows.Forms.Timer();
        _animationTimer.Interval = ANIMATION_DURATION;
        _animationTimer.Tick += AnimationTimer_Tick;
        _currentSidebarWidth = SIDEBAR_EXPANDED_WIDTH;
        _targetSidebarWidth = SIDEBAR_EXPANDED_WIDTH;
    }
    
    private void AnimationTimer_Tick(object? sender, EventArgs e)
    {
        _animationStep++;
        float progress = (float)_animationStep / ANIMATION_STEPS;
        float easedProgress = 1 - (float)Math.Pow(1 - progress, 3);
        int newWidth = _currentSidebarWidth + (int)((_targetSidebarWidth - _currentSidebarWidth) * easedProgress);
        _sidebarPanel.Width = newWidth;
        
        if (_animationStep >= ANIMATION_STEPS)
        {
            _animationTimer.Stop();
            _currentSidebarWidth = _targetSidebarWidth;
            _sidebarPanel.Width = _targetSidebarWidth;
            UpdateMenuButtonTexts();
            this.SuspendLayout();
            _sidebarPanel.ResumeLayout(true);
            _contentPanel.ResumeLayout(true);
            this.ResumeLayout(true);
        }
        else
        {
            float opacity = _sidebarExpanded ? easedProgress : (1 - easedProgress);
            UpdateMenuButtonTexts(opacity);
        }
    }
    
    private void UpdateMenuButtonTexts(float opacity = 1.0f)
    {
        foreach (Control ctrl in _sidebarPanel.Controls)
        {
            if (ctrl is Panel panel)
            {
                foreach (Control btnCtrl in panel.Controls)
                {
                    if (btnCtrl is Button btn)
                    {
                        btn.Width = Math.Max(46, Math.Min(256, _sidebarPanel.Width - 24));
                        if (_sidebarPanel.Width > 150)
                        {
                            string fullText = GetMenuText(btn.Tag?.ToString() ?? "");
                            btn.Text = "    " + fullText;
                            btn.ForeColor = Color.FromArgb((int)(255 * opacity), btn.ForeColor.R, btn.ForeColor.G, btn.ForeColor.B);
                        }
                        else
                        {
                            btn.Text = "";
                        }
                    }
                }
            }
        }
    }
    
    private void InitializeModernUI()
    {
        _headerPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 70,
            BackColor = Color.White
        };
        _headerPanel.Paint += HeaderPanel_Paint;
        
        _toggleButton = new Button
        {
            Size = new Size(40, 40),
            Location = new Point(20, 15),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.Transparent,
            Cursor = Cursors.Hand,
            Image = CreateMenuIcon(),
            ImageAlign = ContentAlignment.MiddleCenter
        };
        _toggleButton.FlatAppearance.BorderSize = 0;
        _toggleButton.Click += ToggleButton_Click;
        
        _titleLabel = new Label
        {
            Text = "Ana Sayfa",
            Font = Typography.Title,
            ForeColor = ThemeColors.TextPrimary,
            Location = new Point(80, 12),
            AutoSize = true
        };
        
        _dateLabel = new Label
        {
            Text = DateTime.Now.ToString("dddd, d MMMM yyyy", new System.Globalization.CultureInfo("tr-TR")),
            Font = Typography.Medium,
            ForeColor = ThemeColors.TextSecondary,
            Location = new Point(80, 40),
            AutoSize = true
        };
        
        _headerPanel.Controls.AddRange(new Control[] { _toggleButton, _titleLabel, _dateLabel });
        
        _sidebarPanel = new Panel
        {
            Dock = DockStyle.Left,
            Width = SIDEBAR_EXPANDED_WIDTH,
            BackColor = Color.White,
            Padding = new Padding(0, 20, 0, 20)
        };
        _sidebarPanel.Paint += SidebarPanel_Paint;
        
        InitializeSidebar();
        
        _contentPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = ThemeColors.Background,
            Padding = new Padding(24)
        };
        
        Controls.Add(_contentPanel);
        Controls.Add(_sidebarPanel);
        Controls.Add(_headerPanel);
        
        LoadPage("dashboard");
    }
    
    private void InitializeSidebar()
    {
        var logoPanel = new Panel
        {
            Height = 80,
            Dock = DockStyle.Top,
            BackColor = Color.White,
            Padding = new Padding(20, 0, 20, 0)
        };
        
        var logoBox = new PictureBox
        {
            Size = new Size(48, 48),
            Location = new Point(20, 16),
            Image = CreateLogoImage(),
            SizeMode = PictureBoxSizeMode.Zoom
        };
        
        var companyLabel = new Label
        {
            Text = "ARCA",
            Font = Typography.XLargeBold,
            ForeColor = ThemeColors.TextPrimary,
            Location = new Point(80, 18),
            AutoSize = true
        };
        
        var versionLabel = new Label
        {
            Text = $"v{Program.AppVersion}",
            Font = Typography.Small,
            ForeColor = ThemeColors.TextMuted,
            Location = new Point(80, 42),
            AutoSize = true
        };
        
        logoPanel.Controls.AddRange(new Control[] { logoBox, companyLabel, versionLabel });
        _sidebarPanel.Controls.Add(logoPanel);
        
        var menuPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(12, 20, 12, 0),
            AutoScroll = true
        };
        
        var menuItems = new[]
        {
            ("dashboard", "Ana Sayfa", CreateHomeIcon()),
            ("plans", "Kalite Kontrol Planları", CreateClipboardIcon()),
            ("measurement", "Ölçüm Raporu", CreateRulerIcon()),
            ("measurementsList", "Tüm Ölçümler", CreateListIcon()),
            ("reports", "Raporlar", CreateChartIcon()),
            ("calculator", "Hesap Makinesi", CreateCalcIcon()),
            ("settings", "Ayarlar", CreateSettingsIcon())
        };
        
        int y = 0;
        foreach (var (id, text, icon) in menuItems)
        {
            var btn = CreateMenuButton(id, text, icon);
            btn.Location = new Point(0, y);
            btn.Click += MenuButton_Click;
            menuPanel.Controls.Add(btn);
            y += 56;
        }
        
        _sidebarPanel.Controls.Add(menuPanel);
    }
    
    private Button CreateMenuButton(string id, string text, Image icon)
    {
        var btn = new Button
        {
            Name = $"btn_{id}",
            Size = new Size(256, 48),
            FlatStyle = FlatStyle.Flat,
            BackColor = id == _currentPage ? ThemeColors.Primary50 : Color.White,
            ForeColor = id == _currentPage ? ThemeColors.Primary : ThemeColors.SidebarText,
            Font = id == _currentPage ? Typography.MediumBold : Typography.Medium,
            Text = "    " + text,
            TextAlign = ContentAlignment.MiddleLeft,
            Image = icon,
            ImageAlign = ContentAlignment.MiddleLeft,
            TextImageRelation = TextImageRelation.ImageBeforeText,
            Cursor = Cursors.Hand,
            Tag = id
        };
        btn.FlatAppearance.BorderSize = 0;
        btn.FlatAppearance.MouseOverBackColor = ThemeColors.SidebarHover;
        btn.FlatAppearance.MouseDownBackColor = ThemeColors.Primary50;
        
        return btn;
    }
    
    private void MenuButton_Click(object? sender, EventArgs e)
    {
        if (sender is Button btn && btn.Tag is string pageId)
        {
            LoadPage(pageId);
        }
    }
    
    private void LoadPage(string pageId)
    {
        _currentPage = pageId;
        
        var titles = new System.Collections.Generic.Dictionary<string, string>
        {
            ["dashboard"] = "Ana Sayfa",
            ["plans"] = "Kalite Kontrol Planları",
            ["measurement"] = "Ölçüm Raporu",
            ["measurementsList"] = "Tüm Ölçümler",
            ["reports"] = "Raporlar",
            ["calculator"] = "Hesap Makinesi",
            ["settings"] = "Ayarlar"
        };
        _titleLabel.Text = titles.GetValueOrDefault(pageId, "Ana Sayfa");
        
        foreach (Control ctrl in _sidebarPanel.Controls)
        {
            if (ctrl is Panel panel)
            {
                foreach (Control btnCtrl in panel.Controls)
                {
                    if (btnCtrl is Button btn && btn.Tag is string btnId)
                    {
                        btn.BackColor = btnId == pageId ? ThemeColors.Primary50 : Color.White;
                        btn.ForeColor = btnId == pageId ? ThemeColors.Primary : ThemeColors.SidebarText;
                        btn.Font = btnId == pageId ? Typography.MediumBold : Typography.Medium;
                    }
                }
            }
        }
        
        if (pageId == "calculator")
        {
            using var calc = new CalculatorForm();
            calc.ShowDialog();
            LoadPage("dashboard");
            return;
        }
        
        _contentPanel.Controls.Clear();
        UserControl? content = pageId switch
        {
            "dashboard" => new DashboardControl(),
            "plans" => new PlansControl(),
            "measurement" => new MeasurementControl(),
            "measurementsList" => new MeasurementsListControl(),
            "reports" => new ReportsControl(),
            "settings" => new SettingsControl(),
            _ => null
        };
        
        if (content != null)
        {
            content.Dock = DockStyle.Fill;
            _contentPanel.Controls.Add(content);
        }
    }
    
    private void ToggleButton_Click(object? sender, EventArgs e)
    {
        _sidebarExpanded = !_sidebarExpanded;
        _targetSidebarWidth = _sidebarExpanded ? SIDEBAR_EXPANDED_WIDTH : SIDEBAR_COLLAPSED_WIDTH;
        _currentSidebarWidth = _sidebarPanel.Width;
        _animationStep = 0;
        
        if (!_sidebarExpanded)
        {
            UpdateMenuButtonTexts(0);
        }
        
        this.SuspendLayout();
        _sidebarPanel.SuspendLayout();
        _contentPanel.SuspendLayout();
        
        _animationTimer.Start();
    }
    
    private string GetMenuText(string id)
    {
        return id switch
        {
            "dashboard" => "Ana Sayfa",
            "plans" => "Kalite Kontrol Planları",
            "measurement" => "Ölçüm Raporu",
            "measurementsList" => "Tüm Ölçümler",
            "reports" => "Raporlar",
            "calculator" => "Hesap Makinesi",
            "settings" => "Ayarlar",
            _ => ""
        };
    }
    
    private void HeaderPanel_Paint(object? sender, PaintEventArgs e)
    {
        using var pen = new Pen(ThemeColors.Border, 1);
        e.Graphics.DrawLine(pen, 0, _headerPanel.Height - 1, _headerPanel.Width, _headerPanel.Height - 1);
    }
    
    private void SidebarPanel_Paint(object? sender, PaintEventArgs e)
    {
        using var pen = new Pen(ThemeColors.Border, 1);
        e.Graphics.DrawLine(pen, _sidebarPanel.Width - 1, 0, _sidebarPanel.Width - 1, _sidebarPanel.Height);
    }
    
    private Image CreateMenuIcon()
    {
        var bmp = new Bitmap(24, 24);
        using var g = Graphics.FromImage(bmp);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var pen = new Pen(ThemeColors.TextPrimary, 2);
        g.DrawLine(pen, 4, 6, 20, 6);
        g.DrawLine(pen, 4, 12, 20, 12);
        g.DrawLine(pen, 4, 18, 20, 18);
        return bmp;
    }
    
    private Image CreateHomeIcon()
    {
        var bmp = new Bitmap(20, 20);
        using var g = Graphics.FromImage(bmp);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var pen = new Pen(ThemeColors.Primary, 2);
        g.DrawPolygon(pen, new[] { new Point(10, 2), new Point(2, 10), new Point(4, 10), new Point(4, 16), new Point(8, 16), new Point(8, 12), new Point(12, 12), new Point(12, 16), new Point(16, 16), new Point(16, 10), new Point(18, 10) });
        return bmp;
    }
    
    private Image CreateClipboardIcon()
    {
        var bmp = new Bitmap(20, 20);
        using var g = Graphics.FromImage(bmp);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var pen = new Pen(ThemeColors.Primary, 2);
        g.DrawRectangle(pen, 4, 6, 12, 12);
        g.DrawRectangle(pen, 7, 2, 6, 4);
        return bmp;
    }
    
    private Image CreateRulerIcon()
    {
        var bmp = new Bitmap(20, 20);
        using var g = Graphics.FromImage(bmp);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var pen = new Pen(ThemeColors.Primary, 2);
        g.DrawLine(pen, 4, 16, 16, 4);
        g.DrawLine(pen, 6, 14, 6, 16);
        g.DrawLine(pen, 9, 11, 9, 13);
        g.DrawLine(pen, 12, 8, 12, 10);
        g.DrawLine(pen, 15, 5, 15, 7);
        return bmp;
    }
    
    private Image CreateListIcon()
    {
        var bmp = new Bitmap(20, 20);
        using var g = Graphics.FromImage(bmp);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var pen = new Pen(ThemeColors.Primary, 2);
        for (int i = 0; i < 4; i++)
        {
            g.DrawLine(pen, 6, 4 + i * 4, 18, 4 + i * 4);
            g.FillEllipse(Brushes.Gray, 2, 2 + i * 4, 3, 3);
        }
        return bmp;
    }
    
    private Image CreateChartIcon()
    {
        var bmp = new Bitmap(20, 20);
        using var g = Graphics.FromImage(bmp);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var pen = new Pen(ThemeColors.Primary, 2);
        g.DrawLine(pen, 2, 18, 18, 18);
        g.DrawLine(pen, 2, 18, 2, 2);
        g.DrawLine(pen, 6, 14, 6, 10);
        g.DrawLine(pen, 10, 14, 10, 6);
        g.DrawLine(pen, 14, 14, 14, 4);
        return bmp;
    }
    
    private Image CreateCalcIcon()
    {
        var bmp = new Bitmap(20, 20);
        using var g = Graphics.FromImage(bmp);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var pen = new Pen(ThemeColors.Primary, 2);
        g.DrawRectangle(pen, 2, 2, 16, 16);
        g.DrawLine(pen, 4, 5, 16, 5);
        g.DrawLine(pen, 4, 9, 8, 9);
        g.DrawLine(pen, 4, 13, 8, 13);
        g.DrawLine(pen, 12, 9, 16, 9);
        g.DrawLine(pen, 12, 13, 16, 13);
        return bmp;
    }
    
    private Image CreateSettingsIcon()
    {
        var bmp = new Bitmap(20, 20);
        using var g = Graphics.FromImage(bmp);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var pen = new Pen(ThemeColors.Primary, 2);
        g.DrawEllipse(pen, 6, 6, 8, 8);
        for (int i = 0; i < 8; i++)
        {
            double angle = i * Math.PI / 4;
            int x1 = 10 + (int)(4 * Math.Cos(angle));
            int y1 = 10 + (int)(4 * Math.Sin(angle));
            int x2 = 10 + (int)(8 * Math.Cos(angle));
            int y2 = 10 + (int)(8 * Math.Sin(angle));
            g.DrawLine(pen, x1, y1, x2, y2);
        }
        return bmp;
    }
    
    private Image CreateLogoImage()
    {
        var bmp = new Bitmap(48, 48);
        using var g = Graphics.FromImage(bmp);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var brush = new LinearGradientBrush(new Rectangle(0, 0, 48, 48), 
            Color.FromArgb(0, 100, 180), Color.FromArgb(0, 60, 120), 45);
        g.FillEllipse(brush, 0, 0, 48, 48);
        using var font = new Font("Segoe UI", 20F, FontStyle.Bold);
        using var textBrush = new SolidBrush(Color.White);
        var size = g.MeasureString("A", font);
        g.DrawString("A", font, textBrush, (48 - size.Width) / 2, (48 - size.Height) / 2);
        return bmp;
    }
}
