using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;

// ── Theme ────────────────────────────────────────────────────────────────────

static class Theme
{
    public static readonly bool Dark = Detect();

    static bool Detect()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            return key?.GetValue("AppsUseLightTheme") is int v && v == 0;
        }
        catch { return false; }
    }
}

// ── Dark Menu Renderer ────────────────────────────────────────────────────────

sealed class DarkMenuRenderer : ToolStripProfessionalRenderer
{
    static readonly Color MenuBg   = Color.FromArgb(31, 31, 31);
    static readonly Color TextCol  = Color.FromArgb(235, 235, 235);
    static readonly Color TextGray = Color.FromArgb(130, 130, 130);

    public DarkMenuRenderer() : base(new DarkColorTable()) { RoundedEdges = false; }

    protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
    {
        e.TextColor = e.Item.Enabled ? TextCol : TextGray;
        base.OnRenderItemText(e);
    }

    protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        => e.Graphics.Clear(MenuBg);

    protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
    {
        using var pen = new Pen(Color.FromArgb(60, 60, 60));
        int y = e.Item.Height / 2;
        e.Graphics.DrawLine(pen, 28, y, e.Item.Width - 4, y);
    }
}

sealed class DarkColorTable : ProfessionalColorTable
{
    static readonly Color Bg  = Color.FromArgb(31, 31, 31);
    static readonly Color Hov = Color.FromArgb(54, 54, 54);
    static readonly Color Bdr = Color.FromArgb(50, 50, 50);

    public override Color MenuItemSelected              => Hov;
    public override Color MenuItemBorder                => Hov;
    public override Color MenuItemSelectedGradientBegin => Hov;
    public override Color MenuItemSelectedGradientEnd   => Hov;
    public override Color MenuItemPressedGradientBegin  => Hov;
    public override Color MenuItemPressedGradientEnd    => Hov;
    public override Color ToolStripDropDownBackground   => Bg;
    public override Color ImageMarginGradientBegin      => Bg;
    public override Color ImageMarginGradientMiddle     => Bg;
    public override Color ImageMarginGradientEnd        => Bg;
    public override Color MenuBorder                    => Bdr;
    public override Color SeparatorDark                 => Bdr;
    public override Color SeparatorLight                => Bdr;
    public override Color CheckBackground               => Hov;
    public override Color CheckSelectedBackground       => Hov;
    public override Color CheckPressedBackground        => Hov;
}
