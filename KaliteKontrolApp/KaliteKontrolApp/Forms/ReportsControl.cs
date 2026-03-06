using KaliteKontrolApp.Controls;
using KaliteKontrolApp.Models;  // DEĞİŞTİRİLDİ: Utils yerine Models
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace KaliteKontrolApp.Forms
{
    public partial class ReportsControl : UserControl
    {
        private SKControl _pieChartControl = null!;
        private SKControl _trendChartControl = null!;
        private Statistics _currentStats = new();
    
    public ReportsControl()
    {
        InitializeComponent();
        LoadStatistics();
    }
    
    private void InitializeComponent()
    {
        BackColor = ThemeColors.Background;
        Dock = DockStyle.Fill;
        Padding = new Padding(24);
        
        var mainLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 2
        };
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        
        var statsCard = new ModernCard
        {
            Dock = DockStyle.Fill,
            CardPadding = new Padding(20),
            Margin = new Padding(0, 0, 12, 12)
        };
        
        var statsTitle = new Label
        {
            Text = "Genel İstatistikler",
            Font = Typography.XLargeBold,
            ForeColor = ThemeColors.TextPrimary,
            Dock = DockStyle.Top,
            Height = 30
        };
        statsCard.Controls.Add(statsTitle);
        
        var statsPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 20, 0, 0)
        };
        statsPanel.Name = "statsPanel";
        statsCard.Controls.Add(statsPanel);
        
        mainLayout.Controls.Add(statsCard, 0, 0);
        
        var pieCard = new ModernCard
        {
            Dock = DockStyle.Fill,
            CardPadding = new Padding(20),
            Margin = new Padding(12, 0, 0, 12)
        };
        
        var pieTitle = new Label
        {
            Text = "Kalite Dağılımı",
            Font = Typography.XLargeBold,
            ForeColor = ThemeColors.TextPrimary,
            Dock = DockStyle.Top,
            Height = 30
        };
        pieCard.Controls.Add(pieTitle);
        
        _pieChartControl = new SKControl
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 20, 0, 0)
        };
        _pieChartControl.PaintSurface += PieChartControl_PaintSurface;
        pieCard.Controls.Add(_pieChartControl);
        
        mainLayout.Controls.Add(pieCard, 1, 0);
        
        var trendCard = new ModernCard
        {
            Dock = DockStyle.Fill,
            CardPadding = new Padding(20),
            Margin = new Padding(0, 12, 12, 0)
        };
        
        var trendTitle = new Label
        {
            Text = "Aylık Trend",
            Font = Typography.XLargeBold,
            ForeColor = ThemeColors.TextPrimary,
            Dock = DockStyle.Top,
            Height = 30
        };
        trendCard.Controls.Add(trendTitle);
        
        _trendChartControl = new SKControl
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 20, 0, 0)
        };
        _trendChartControl.PaintSurface += TrendChartControl_PaintSurface;
        trendCard.Controls.Add(_trendChartControl);
        
        mainLayout.Controls.Add(trendCard, 0, 1);
        
        var exportCard = new ModernCard
        {
            Dock = DockStyle.Fill,
            CardPadding = new Padding(20),
            Margin = new Padding(12, 12, 0, 0)
        };
        
        var exportTitle = new Label
        {
            Text = "Rapor İşlemleri",
            Font = Typography.XLargeBold,
            ForeColor = ThemeColors.TextPrimary,
            Dock = DockStyle.Top,
            Height = 30
        };
        exportCard.Controls.Add(exportTitle);
        
        var exportPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            Margin = new Padding(0, 20, 0, 0),
            Padding = new Padding(10)
        };
        
        var btnExportPlans = new ModernButton
        {
            Text = "Planları Excel'e Aktar",
            Size = new Size(240, 44),
            Margin = new Padding(0, 0, 0, 12),
            ButtonColor = ThemeColors.Primary
        };
        btnExportPlans.Click += BtnExportPlans_Click;
        
        var btnExportMeasurements = new ModernButton
        {
            Text = "Ölçümleri Excel'e Aktar",
            Size = new Size(240, 44),
            Margin = new Padding(0, 0, 0, 12),
            ButtonColor = ThemeColors.Secondary
        };
        btnExportMeasurements.Click += BtnExportMeasurements_Click;
        
        var btnProductReport = new ModernButton
        {
            Text = "Ürün Bazlı Rapor",
            Size = new Size(240, 44),
            Margin = new Padding(0, 0, 0, 12),
            ButtonColor = ThemeColors.Info
        };
        btnProductReport.Click += BtnProductReport_Click;
        
        var btnBackup = new ModernOutlineButton
        {
            Text = "Veritabanı Yedekle",
            Size = new Size(240, 44),
            Margin = new Padding(0, 0, 0, 12),
            BorderColor = ThemeColors.Success
        };
        btnBackup.Click += BtnBackup_Click;
        
        exportPanel.Controls.AddRange(new Control[] { btnExportPlans, btnExportMeasurements, btnProductReport, btnBackup });
        exportCard.Controls.Add(exportPanel);
        
        mainLayout.Controls.Add(exportCard, 1, 1);
        
        Controls.Add(mainLayout);
    }
    
    private void PieChartControl_PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.White);
        
        var stats = _currentStats;
        var total = stats.OkProducts + stats.NokProducts + stats.ConditionalProducts;
        
        if (total == 0)
        {
            using var textPaint = new SKPaint
            {
                Color = SKColors.Gray,
                TextSize = 16,
                IsAntialias = true,
                TextAlign = SKTextAlign.Center
            };
            canvas.DrawText("Veri yok", e.Info.Width / 2f, e.Info.Height / 2f, textPaint);
            return;
        }
        
        var centerX = e.Info.Width / 2f - 80;
        var centerY = e.Info.Height / 2f;
        var radius = Math.Min(centerX, centerY) - 20;
        
        var rect = new SKRect(centerX - radius, centerY - radius, centerX + radius, centerY + radius);
        float startAngle = -90;
        
        var slices = new[]
        {
            (stats.OkProducts, ThemeColors.Success, "Uygun"),
            (stats.NokProducts, ThemeColors.Error, "Uygun Değil"),
            (stats.ConditionalProducts, ThemeColors.Warning, "Şartlı")
        };
        
        foreach (var (value, color, label) in slices)
        {
            if (value > 0)
            {
                var sweepAngle = 360f * value / total;
                using var paint = new SKPaint
                {
                    Color = new SKColor(color.R, color.G, color.B),
                    Style = SKPaintStyle.Fill,
                    IsAntialias = true
                };
                canvas.DrawArc(rect, startAngle, sweepAngle - 2, true, paint);
                startAngle += sweepAngle;
            }
        }
        
        using var holePaint = new SKPaint
        {
            Color = SKColors.White,
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        };
        canvas.DrawCircle(centerX, centerY, radius * 0.5f, holePaint);
        
        using var centerTextPaint = new SKPaint
        {
            Color = new SKColor(ThemeColors.TextPrimary.R, ThemeColors.TextPrimary.G, ThemeColors.TextPrimary.B),
            TextSize = 24,
            IsAntialias = true,
            TextAlign = SKTextAlign.Center,
            Typeface = SKTypeface.FromFamilyName("Segoe UI", SKFontStyle.Bold)
        };
        using var centerSubTextPaint = new SKPaint
        {
            Color = new SKColor(ThemeColors.TextSecondary.R, ThemeColors.TextSecondary.G, ThemeColors.TextSecondary.B),
            TextSize = 12,
            IsAntialias = true,
            TextAlign = SKTextAlign.Center
        };
        
        canvas.DrawText(total.ToString(), centerX, centerY - 5, centerTextPaint);
        canvas.DrawText("Toplam", centerX, centerY + 15, centerSubTextPaint);
        
        var legendX = (int)centerX + radius + 30;
        var legendY = (int)centerY - 50;
        
        foreach (var (value, color, label) in slices)
        {
            if (value > 0)
            {
                using var boxPaint = new SKPaint
                {
                    Color = new SKColor(color.R, color.G, color.B),
                    Style = SKPaintStyle.Fill,
                    IsAntialias = true
                };
                canvas.DrawRoundRect(new SKRect(legendX, legendY, legendX + 12, legendY + 12), 3, 3, boxPaint);
                
                using var labelPaint = new SKPaint
                {
                    Color = new SKColor(ThemeColors.TextPrimary.R, ThemeColors.TextPrimary.G, ThemeColors.TextPrimary.B),
                    TextSize = 12,
                    IsAntialias = true
                };
                canvas.DrawText($"{label}: {value}", legendX + 20, legendY + 10, labelPaint);
                
                legendY += 25;
            }
        }
    }
    
    private void TrendChartControl_PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.White);
        
        var width = e.Info.Width - 60;
        var height = e.Info.Height - 60;
        var left = 40f;
        var top = 20f;
        var bottom = top + height;
        var right = left + width;
        
        var data = DatabaseManager.Instance.GetMonthlyStatistics();
        if (data.Count == 0) return;
        
        var maxValue = Math.Max(data.Max(d => d.Ok + d.Nok + d.Conditional), 1);
        var barWidth = width / (data.Count * 1.5f);
        var gap = barWidth * 0.5f;
        
        using var axisPaint = new SKPaint
        {
            Color = new SKColor(ThemeColors.TextMuted.R, ThemeColors.TextMuted.G, ThemeColors.TextMuted.B),
            StrokeWidth = 1,
            IsAntialias = true
        };
        
        canvas.DrawLine(left, top, left, bottom, axisPaint);
        canvas.DrawLine(left, bottom, right, bottom, axisPaint);
        
        using var labelPaint = new SKPaint
        {
            Color = new SKColor(ThemeColors.TextSecondary.R, ThemeColors.TextSecondary.G, ThemeColors.TextSecondary.B),
            TextSize = 10,
            IsAntialias = true,
            TextAlign = SKTextAlign.Right
        };
        
        for (int i = 0; i <= 5; i++)
        {
            var y = bottom - (height * i / 5);
            var value = (int)(maxValue * i / 5);
            canvas.DrawText(value.ToString(), left - 8, y + 4, labelPaint);
            
            if (i > 0)
            {
                using var gridPaint = new SKPaint
                {
                    Color = new SKColor(230, 230, 230),
                    StrokeWidth = 1,
                    PathEffect = SKPathEffect.CreateDash(new[] { 4f, 4f }, 0)
                };
                canvas.DrawLine(left, y, right, y, gridPaint);
            }
        }
        
        for (int i = 0; i < data.Count; i++)
        {
            var x = left + gap + i * (barWidth + gap);
            var barHeight = (data[i].Ok + data[i].Nok + data[i].Conditional) / maxValue * height;
            
            var currentY = bottom;
            
            if (data[i].Ok > 0)
            {
                var okHeight = data[i].Ok / maxValue * height;
                using var okPaint = new SKPaint
                {
                    Color = new SKColor(ThemeColors.Success.R, ThemeColors.Success.G, ThemeColors.Success.B),
                    Style = SKPaintStyle.Fill,
                    IsAntialias = true
                };
                canvas.DrawRect(x, currentY - okHeight, barWidth, okHeight, okPaint);
                currentY -= okHeight;
            }
            
            if (data[i].Nok > 0)
            {
                var nokHeight = data[i].Nok / maxValue * height;
                using var nokPaint = new SKPaint
                {
                    Color = new SKColor(ThemeColors.Error.R, ThemeColors.Error.G, ThemeColors.Error.B),
                    Style = SKPaintStyle.Fill,
                    IsAntialias = true
                };
                canvas.DrawRect(x, currentY - nokHeight, barWidth, nokHeight, nokPaint);
                currentY -= nokHeight;
            }
            
            if (data[i].Conditional > 0)
            {
                var condHeight = data[i].Conditional / maxValue * height;
                using var condPaint = new SKPaint
                {
                    Color = new SKColor(ThemeColors.Warning.R, ThemeColors.Warning.G, ThemeColors.Warning.B),
                    Style = SKPaintStyle.Fill,
                    IsAntialias = true
                };
                canvas.DrawRect(x, currentY - condHeight, barWidth, condHeight, condPaint);
            }
            
            using var monthPaint = new SKPaint
            {
                Color = new SKColor(ThemeColors.TextSecondary.R, ThemeColors.TextSecondary.G, ThemeColors.TextSecondary.B),
                TextSize = 10,
                IsAntialias = true,
                TextAlign = SKTextAlign.Center
            };
            canvas.DrawText(data[i].Month, x + barWidth / 2, bottom + 15, monthPaint);
        }
    }
    
    private void LoadStatistics()
    {
        try
        {
            _currentStats = DatabaseManager.Instance.GetStatistics();
            
            foreach (Control ctrl in Controls[0].Controls)
            {
                if (ctrl is ModernCard card && card.Controls["statsPanel"] is Panel statsPanel)
                {
                    statsPanel.Controls.Clear();
                    
                    var items = new[]
                    {
                        ("Toplam Kalite Planı", _currentStats.TotalPlans, ThemeColors.Primary),
                        ("Toplam Ölçüm", _currentStats.TotalMeasurements, ThemeColors.Secondary),
                        ("Uygun Ürünler", _currentStats.OkProducts, ThemeColors.Success),
                        ("Uygun Değil", _currentStats.NokProducts, ThemeColors.Error),
                        ("Şartlı Kabul", _currentStats.ConditionalProducts, ThemeColors.Warning)
                    };
                    
                    int y = 0;
                    foreach (var (label, value, color) in items)
                    {
                        var itemPanel = new Panel
                        {
                            Dock = DockStyle.Top,
                            Height = 40,
                            Margin = new Padding(0, 0, 0, 8)
                        };
                        
                        var colorBox = new Panel
                        {
                            Size = new Size(12, 12),
                            Location = new Point(0, 14),
                            BackColor = color
                        };
                        
                        var lblText = new Label
                        {
                            Text = label,
                            Font = Typography.Medium,
                            ForeColor = ThemeColors.TextSecondary,
                            Location = new Point(20, 10),
                            AutoSize = true
                        };
                        
                        var lblValue = new Label
                        {
                            Text = value.ToString(),
                            Font = Typography.XLargeBold,
                            ForeColor = color,
                            Location = new Point(200, 8),
                            AutoSize = true
                        };
                        
                        itemPanel.Controls.AddRange(new Control[] { colorBox, lblText, lblValue });
                        statsPanel.Controls.Add(itemPanel);
                        y += 48;
                    }
                }
            }
            
            _pieChartControl?.Invalidate();
            _trendChartControl?.Invalidate();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"İstatistikler yüklenirken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    
    private void BtnExportPlans_Click(object? sender, EventArgs e)
    {
        using var dialog = new SaveFileDialog
        {
            Filter = "Excel files (*.xlsx)|*.xlsx",
            FileName = $"KalitePlanlari_{DateTime.Now:yyyyMMdd}.xlsx"
        };
        
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                var plans = DatabaseManager.Instance.GetAllPlans();
                ExcelHelper.ExportPlansToExcel(plans, dialog.FileName);
                MessageBox.Show("Planlar başarıyla dışa aktarıldı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Dışa aktarma hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
    
    private void BtnExportMeasurements_Click(object? sender, EventArgs e)
    {
        using var dialog = new SaveFileDialog
        {
            Filter = "Excel files (*.xlsx)|*.xlsx",
            FileName = $"Olcumler_{DateTime.Now:yyyyMMdd}.xlsx"
        };
        
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                var measurements = DatabaseManager.Instance.GetAllMeasurements();
                ExcelHelper.ExportMeasurementsToExcel(measurements, dialog.FileName);
                MessageBox.Show("Ölçümler başarıyla dışa aktarıldı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Dışa aktarma hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
    
    private void BtnProductReport_Click(object? sender, EventArgs e)
    {
        using var reportForm = new ProductReportForm();
        reportForm.ShowDialog(this);
    }
    
    private void BtnBackup_Click(object? sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog();
        
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                var backupPath = System.IO.Path.Combine(dialog.SelectedPath, 
                    $"kalitekontrol_backup_{DateTime.Now:yyyyMMdd_HHmmss}.db");
                System.IO.File.Copy(Program.DatabasePath, backupPath);
                MessageBox.Show($"Yedekleme başarılı!\nKonum: {backupPath}", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Yedekleme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

public class MonthlyData
{
    public string Month { get; set; } = "";
    public int Ok { get; set; }
    public int Nok { get; set; }
    public int Conditional { get; set; }
}
