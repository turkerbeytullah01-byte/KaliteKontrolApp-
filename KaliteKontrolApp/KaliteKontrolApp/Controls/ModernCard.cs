using KaliteKontrolApp.Utils;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace KaliteKontrolApp.Controls;

public class ModernCard : Panel
{
    private int _borderRadius = 12;
    private int _shadowDepth = 4;
    private Color _cardColor = Color.White;
    private Padding _cardPadding = new Padding(20);
    
    public int BorderRadius 
    { 
        get => _borderRadius; 
        set { _borderRadius = value; Invalidate(); }
    }
    
    public int ShadowDepth 
    { 
        get => _shadowDepth; 
        set { _shadowDepth = value; Invalidate(); }
    }
    
    public Color CardColor 
    { 
        get => _cardColor; 
        set { _cardColor = value; Invalidate(); }
    }
    
    public Padding CardPadding 
    { 
        get => _cardPadding; 
        set { _cardPadding = value; Padding = value; Invalidate(); }
    }
    
    public ModernCard()
    {
        BackColor = Color.Transparent;
        Padding = _cardPadding;
        DoubleBuffered = true;
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
    }
    
    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        
        var cardRect = new Rectangle(_shadowDepth, 0, Width - _shadowDepth * 2 - 1, Height - _shadowDepth - 1);
        
        for (int i = 0; i < _shadowDepth; i++)
        {
            int alpha = 25 - i * 5;
            using var shadowBrush = new SolidBrush(Color.FromArgb(alpha, 0, 0, 0));
            var shadowRect = new Rectangle(i, i + 2, Width - i * 2 - 1, Height - i - 1);
            using var shadowPath = GetRoundedPath(shadowRect, _borderRadius);
            g.FillPath(shadowBrush, shadowPath);
        }
        
        using var cardPath = GetRoundedPath(cardRect, _borderRadius);
        using var cardBrush = new SolidBrush(_cardColor);
        g.FillPath(cardBrush, cardPath);
        
        using var borderPen = new Pen(ThemeColors.BorderLight, 1);
        g.DrawPath(borderPen, cardPath);
        
        base.OnPaint(e);
    }
    
    private GraphicsPath GetRoundedPath(Rectangle rect, int radius)
    {
        var path = new GraphicsPath();
        int diameter = radius * 2;
        
        path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
        path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
        path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        
        return path;
    }
}

public class StatCard : Panel
{
    private string _title = "Başlık";
    private string _value = "0";
    private Image? _icon;
    private Color _iconBgColor = ThemeColors.Primary50;
    private Color _iconColor = ThemeColors.Primary;
    private Color _valueColor = ThemeColors.TextPrimary;
    
    public string Title 
    { 
        get => _title; 
        set { _title = value; Invalidate(); }
    }
    
    public string Value 
    { 
        get => _value; 
        set { _value = value; Invalidate(); }
    }
    
    public Image? Icon 
    { 
        get => _icon; 
        set { _icon = value; Invalidate(); }
    }
    
    public Color IconBgColor 
    { 
        get => _iconBgColor; 
        set { _iconBgColor = value; Invalidate(); }
    }
    
    public Color IconColor 
    { 
        get => _iconColor; 
        set { _iconColor = value; Invalidate(); }
    }
    
    public Color ValueColor 
    { 
        get => _valueColor; 
        set { _valueColor = value; Invalidate(); }
    }
    
    public StatCard()
    {
        BackColor = Color.White;
        Size = new Size(240, 120);
        Padding = new Padding(20);
        DoubleBuffered = true;
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
    }
    
    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        
        var rect = new Rectangle(0, 0, Width - 1, Height - 1);
        
        using var path = GetRoundedPath(rect, 12);
        
        for (int i = 1; i <= 4; i++)
        {
            using var shadowBrush = new SolidBrush(Color.FromArgb(15, 0, 0, 0));
            var shadowRect = new Rectangle(i, i + 1, Width - i * 2, Height - i);
            using var shadowPath = GetRoundedPath(shadowRect, 12);
            g.FillPath(shadowBrush, shadowPath);
        }
        
        using var bgBrush = new SolidBrush(BackColor);
        g.FillPath(bgBrush, path);
        
        using var borderPen = new Pen(ThemeColors.BorderLight, 1);
        g.DrawPath(borderPen, path);
        
        var iconRect = new Rectangle(20, 30, 48, 48);
        using var iconBgBrush = new SolidBrush(_iconBgColor);
        g.FillEllipse(iconBgBrush, iconRect);
        
        if (_icon != null)
        {
            var iconSize = 24;
            var iconX = iconRect.X + (iconRect.Width - iconSize) / 2;
            var iconY = iconRect.Y + (iconRect.Height - iconSize) / 2;
            using var iconBmp = new Bitmap(_icon);
            g.DrawImage(iconBmp, iconX, iconY, iconSize, iconSize);
        }
        
        using var valueFont = new Font(Typography.FontFamily, 24F, FontStyle.Bold);
        using var valueBrush = new SolidBrush(_valueColor);
        var valueRect = new Rectangle(80, 20, Width - 100, 45);
        g.DrawString(_value, valueFont, valueBrush, valueRect);
        
        using var titleFont = new Font(Typography.FontFamily, 11F);
        using var titleBrush = new SolidBrush(ThemeColors.TextSecondary);
        var titleRect = new Rectangle(80, 65, Width - 100, 25);
        g.DrawString(_title, titleFont, titleBrush, titleRect);
        
        base.OnPaint(e);
    }
    
    private GraphicsPath GetRoundedPath(Rectangle rect, int radius)
    {
        var path = new GraphicsPath();
        int diameter = radius * 2;
        
        path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
        path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
        path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        
        return path;
    }
}
