using System.Drawing;

namespace KaliteKontrolApp.Utils;

public static class Typography
{
    public static string FontFamily => "Segoe UI";
    public static float SizeSmall => 9F;
    public static float SizeNormal => 10F;
    public static float SizeMedium => 11F;
    public static float SizeLarge => 12F;
    public static float SizeXLarge => 14F;
    public static float SizeXXLarge => 16F;
    public static float SizeTitle => 18F;
    public static float SizeHeader => 20F;
    
    public static Font Small => new Font(FontFamily, SizeSmall, FontStyle.Regular);
    public static Font SmallBold => new Font(FontFamily, SizeSmall, FontStyle.Bold);
    public static Font Normal => new Font(FontFamily, SizeNormal, FontStyle.Regular);
    public static Font NormalBold => new Font(FontFamily, SizeNormal, FontStyle.Bold);
    public static Font Medium => new Font(FontFamily, SizeMedium, FontStyle.Regular);
    public static Font MediumBold => new Font(FontFamily, SizeMedium, FontStyle.Bold);
    public static Font Large => new Font(FontFamily, SizeLarge, FontStyle.Regular);
    public static Font LargeBold => new Font(FontFamily, SizeLarge, FontStyle.Bold);
    public static Font XLarge => new Font(FontFamily, SizeXLarge, FontStyle.Regular);
    public static Font XLargeBold => new Font(FontFamily, SizeXLarge, FontStyle.Bold);
    public static Font XXLarge => new Font(FontFamily, SizeXXLarge, FontStyle.Regular);
    public static Font XXLargeBold => new Font(FontFamily, SizeXXLarge, FontStyle.Bold);
    public static Font Title => new Font(FontFamily, SizeTitle, FontStyle.Bold);
    public static Font Header => new Font(FontFamily, SizeHeader, FontStyle.Bold);
    
    public static Color TextPrimary => ThemeColors.TextPrimary;
    public static Color TextSecondary => ThemeColors.TextSecondary;
    public static Color TextMuted => ThemeColors.TextMuted;
    public static Color TextOnPrimary => ThemeColors.TextOnPrimary;
}
