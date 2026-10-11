using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

static class N
{
    public delegate bool EnumProc(IntPtr h, IntPtr l);
    [StructLayout(LayoutKind.Sequential)] public struct POINT { public int X, Y; }
    [StructLayout(LayoutKind.Sequential)] public struct RECT { public int L, T, R, B; }
    [StructLayout(LayoutKind.Sequential)] public struct MONITORINFO { public int cb; public RECT mon, work; public int flags; }
    [DllImport("user32.dll")] public static extern bool SetProcessDPIAware();
    [DllImport("user32.dll")] public static extern bool EnumWindows(EnumProc p, IntPtr l);
    [DllImport("user32.dll")] public static extern bool IsWindowVisible(IntPtr h);
    [DllImport("user32.dll")] public static extern bool IsWindow(IntPtr h);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] public static extern int GetWindowText(IntPtr h, StringBuilder s, int n);
    [DllImport("user32.dll")] public static extern int GetWindowTextLength(IntPtr h);
    [DllImport("user32.dll")] public static extern int GetWindowLong(IntPtr h, int i);
    [DllImport("user32.dll")] public static extern int SetWindowLong(IntPtr h, int i, int v);
    [DllImport("user32.dll")] public static extern bool SetLayeredWindowAttributes(IntPtr h, uint k, byte a, uint f);
    [DllImport("user32.dll")] public static extern bool GetLayeredWindowAttributes(IntPtr h, out uint k, out byte a, out uint f);
    [DllImport("user32.dll")] public static extern bool SetWindowPos(IntPtr h, IntPtr after, int x, int y, int cx, int cy, uint f);
    [DllImport("user32.dll")] public static extern uint GetWindowThreadProcessId(IntPtr h, out uint pid);
    [DllImport("user32.dll")] public static extern IntPtr WindowFromPoint(POINT p);
    [DllImport("user32.dll")] public static extern IntPtr GetAncestor(IntPtr h, uint f);
    [DllImport("user32.dll")] public static extern bool RegisterHotKey(IntPtr h, int id, uint m, uint vk);
    [DllImport("user32.dll")] public static extern bool UnregisterHotKey(IntPtr h, int id);
    [DllImport("user32.dll")] public static extern bool ReleaseCapture();
    [DllImport("user32.dll")] public static extern IntPtr SendMessage(IntPtr h, int m, int w, int l);
    [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr h);
    [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr h, out RECT r);
    [DllImport("user32.dll")] public static extern IntPtr MonitorFromWindow(IntPtr h, uint f);
    [DllImport("user32.dll")] public static extern bool GetMonitorInfo(IntPtr m, ref MONITORINFO mi);
    [DllImport("user32.dll")] public static extern bool IsZoomed(IntPtr h);
    [DllImport("user32.dll")] public static extern bool IsIconic(IntPtr h);
    [DllImport("user32.dll")] public static extern IntPtr SetThreadDpiAwarenessContext(IntPtr c);
    [DllImport("kernel32.dll")] public static extern IntPtr OpenProcess(uint a, bool inh, uint pid);
    [DllImport("kernel32.dll")] public static extern bool CloseHandle(IntPtr h);
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)] public static extern bool QueryFullProcessImageName(IntPtr h, int flags, StringBuilder s, ref int size);
    [DllImport("dwmapi.dll")] public static extern int DwmGetWindowAttribute(IntPtr h, int a, out int v, int sz);
    [DllImport("dwmapi.dll")] public static extern int DwmGetWindowAttribute(IntPtr h, int a, out RECT r, int sz);
    [DllImport("dwmapi.dll")] public static extern int DwmSetWindowAttribute(IntPtr h, int a, ref int v, int sz);
    [DllImport("gdi32.dll")] public static extern IntPtr CreateRoundRectRgn(int a, int b, int c, int d, int e, int f);

    public const int GWL_EXSTYLE = -20;
    public const int WS_EX_TOPMOST = 0x8, WS_EX_TRANSPARENT = 0x20, WS_EX_TOOLWINDOW = 0x80,
                     WS_EX_APPWINDOW = 0x40000, WS_EX_LAYERED = 0x80000;
    public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1), HWND_NOTOPMOST = new IntPtr(-2);
    public const uint SWP_NOSIZE = 1, SWP_NOMOVE = 2, SWP_NOZORDER = 4, SWP_NOACTIVATE = 0x10;
}

static class UI
{
    public static float S = 1f; // DPI 縮放倍率（100% = 1.0）
    public static int P(int v) { return (int)Math.Round(v * S); }

    // iOS 深色風格色盤
    public static readonly Color BG0 = ColorTranslator.FromHtml("#1A1B20");
    public static readonly Color BG1 = ColorTranslator.FromHtml("#0F1013");
    public static readonly Color CARD = ColorTranslator.FromHtml("#1D1E24");
    public static readonly Color CARD2 = ColorTranslator.FromHtml("#272830");
    public static readonly Color DEEP = ColorTranslator.FromHtml("#14151A");
    public static readonly Color FILL = ColorTranslator.FromHtml("#2A2B33");
    public static readonly Color FILLH = ColorTranslator.FromHtml("#383A44");
    public static readonly Color LINE = ColorTranslator.FromHtml("#2C2D36");
    public static readonly Color TRACK = ColorTranslator.FromHtml("#3A3B45");
    public static readonly Color ACC = ColorTranslator.FromHtml("#0A84FF");
    public static readonly Color ACCH = ColorTranslator.FromHtml("#3D9CFF");
    public static readonly Color GREEN = ColorTranslator.FromHtml("#30D158");
    public static readonly Color ORANGE = ColorTranslator.FromHtml("#FF9F0A");
    public static readonly Color RED = ColorTranslator.FromHtml("#FF453A");
    public static readonly Color TXT = ColorTranslator.FromHtml("#F2F2F7");
    public static readonly Color MUT = ColorTranslator.FromHtml("#8E8E99");

    static float FS(float s)
    {
#if PREVIEW
        return s * S; // 預覽環境（Mono）不會自動依 DPI 放大字型
#else
        return s;
#endif
    }
    public static Font F(float s, bool b) { return new Font("Microsoft JhengHei UI", FS(s), b ? FontStyle.Bold : FontStyle.Regular, GraphicsUnit.Point); }
    static bool Has(string n) { try { foreach (var f in FontFamily.Families) if (f.Name == n) return true; } catch { } return false; }
    // 大數字用 Segoe UI Semibold；系統沒有時退回微軟正黑體粗體
    public static Font FN(float s) { return Has("Segoe UI Semibold") ? new Font("Segoe UI Semibold", FS(s), FontStyle.Regular, GraphicsUnit.Point) : F(s * 0.85f, true); }

    public static Color A(Color c, int a) { return Color.FromArgb(Math.Max(0, Math.Min(255, a)), c); }
    public static Color Lerp(Color a, Color b, float t)
    {
        t = Math.Max(0f, Math.Min(1f, t));
        return Color.FromArgb((int)(a.A + (b.A - a.A) * t), (int)(a.R + (b.R - a.R) * t), (int)(a.G + (b.G - a.G) * t), (int)(a.B + (b.B - a.B) * t));
    }

    public static GraphicsPath Round(Rectangle r, int d)
    {
        d = Math.Max(2, Math.Min(d, Math.Min(r.Width, r.Height)));
        var p = new GraphicsPath();
        p.AddArc(r.X, r.Y, d, d, 180, 90);
        p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        p.CloseFigure();
        return p;
    }

    public static Bitmap LoadPng(string name)
    {
        try { using (var s = Assembly.GetExecutingAssembly().GetManifestResourceStream(name)) return new Bitmap(s); }
        catch { return null; }
    }
}

class Ctl : Control
{
    public Ctl()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
        BackColor = Color.Transparent;
    }
    protected override void WndProc(ref Message m)
    {
        if (m.Msg == 0x0203) m.Msg = 0x0201; // WM_LBUTTONDBLCLK → WM_LBUTTONDOWN：快速連點時每一下都算一次點擊
        base.WndProc(ref m);
    }
}

// iOS 風格滑桿：細軌道、白色圓形把手
class Slider : Ctl
{
    public int Min = 0, Max = 100;
    int val = 100; bool drag;
    public event EventHandler Changed;
    public int Value { get { return val; } set { val = Math.Max(Min, Math.Min(Max, value)); Invalidate(); } }
    public Slider() { Cursor = Cursors.Hand; }
    int M { get { return UI.P(16); } }
    void FromX(int x)
    {
        double t = (x - M) / (double)(Width - 2 * M);
        int v = Math.Max(Min, Math.Min(Max, (int)Math.Round(Min + t * (Max - Min))));
        if (v != val) { val = v; Invalidate(); if (Changed != null) Changed(this, EventArgs.Empty); }
    }
    protected override void OnMouseDown(MouseEventArgs e) { drag = true; FromX(e.X); }
    protected override void OnMouseMove(MouseEventArgs e) { if (drag) FromX(e.X); }
    protected override void OnMouseUp(MouseEventArgs e) { drag = false; Invalidate(); }
    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
        int cy = Height / 2, th = UI.P(6), kd = UI.P(26) + (drag ? UI.P(2) : 0);
        float x = M + (val - Min) / (float)(Max - Min) * (Width - 2 * M);
        var track = new Rectangle(M, cy - th / 2, Width - 2 * M, th);
        using (var p = UI.Round(track, th)) using (var b = new SolidBrush(UI.TRACK)) g.FillPath(b, p);
        if (x - M > th)
        {
            var fr = new Rectangle(M, cy - th / 2, (int)(x - M), th);
            using (var p = UI.Round(fr, th)) using (var b = new SolidBrush(UI.ACC)) g.FillPath(b, p);
        }
        for (int i = 3; i >= 1; i--) // 柔和陰影
            using (var b = new SolidBrush(UI.A(Color.Black, 26)))
                g.FillEllipse(b, x - kd / 2f - i, cy - kd / 2f + UI.P(2) - i, kd + 2 * i, kd + 2 * i);
        using (var b = new SolidBrush(Color.White)) g.FillEllipse(b, x - kd / 2f, cy - kd / 2f, kd, kd);
    }
}

// iOS 風格開關：綠色、白色圓鈕、帶動畫；開／關兩種狀態尺寸完全一致
class Switch : Ctl
{
    bool on, down; float pos, stretch;
    System.Windows.Forms.Timer tm;
    public event EventHandler Changed;
    public bool On { get { return on; } }
    public Switch() { Size = new Size(UI.P(51), UI.P(31)); Cursor = Cursors.Hand; }
    public void SetOn(bool v, bool animate)
    {
        on = v;
        if (animate) Kick(); else { pos = v ? 1f : 0f; Invalidate(); }
    }
    void Kick()
    {
        if (tm == null) { tm = new System.Windows.Forms.Timer(); tm.Interval = 15; tm.Tick += Step; }
        tm.Start();
    }
    void Step(object s, EventArgs e)
    {
        float tp = on ? 1f : 0f, ts = down ? 1f : 0f;
        pos += (tp - pos) * 0.30f; stretch += (ts - stretch) * 0.40f;
        bool done = Math.Abs(tp - pos) < 0.01f && Math.Abs(ts - stretch) < 0.01f;
        if (done) { pos = tp; stretch = ts; tm.Stop(); }
        Invalidate();
    }
    protected override void OnMouseDown(MouseEventArgs e) { down = true; Kick(); base.OnMouseDown(e); }
    protected override void OnMouseUp(MouseEventArgs e) { down = false; Kick(); base.OnMouseUp(e); }
    protected override void OnMouseLeave(EventArgs e) { if (down) { down = false; Kick(); } base.OnMouseLeave(e); }
    protected override void OnClick(EventArgs e)
    {
        on = !on; Kick();
        if (Changed != null) Changed(this, EventArgs.Empty);
    }
    protected override void Dispose(bool d) { if (d && tm != null) tm.Dispose(); base.Dispose(d); }
    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
        var r = new Rectangle(0, 0, Width - 1, Height - 1);
        using (var p = UI.Round(r, Height - 1)) using (var b = new SolidBrush(UI.Lerp(UI.TRACK, UI.GREEN, pos))) g.FillPath(b, p);
        int pad = UI.P(2), d = Height - 2 * pad - 1;
        int kw = d + (int)(stretch * UI.P(5));
        float x = pad + pos * (Width - 1 - 2 * pad - kw);
        var kr = new RectangleF(x, pad, kw, d);
        using (var b = new SolidBrush(UI.A(Color.Black, 40))) g.FillEllipse(b, kr.X - 1, kr.Y + UI.P(2), kr.Width + 2, kr.Height);
        using (var p = UI.Round(Rectangle.Round(kr), d)) using (var b = new SolidBrush(Color.White)) g.FillPath(b, p);
    }
}

class Btn : Ctl
{
    public bool Primary, Flat, Active; public int Radius = 12; public Color HoverFill = Color.Empty;
    bool down; float hv; bool hover;
    System.Windows.Forms.Timer tm;
    public Btn(string t) { Text = t; Cursor = Cursors.Hand; ForeColor = UI.TXT; }
    void Kick()
    {
        if (tm == null) { tm = new System.Windows.Forms.Timer(); tm.Interval = 15; tm.Tick += Step; }
        tm.Start();
    }
    void Step(object s, EventArgs e)
    {
        float tg = hover ? 1f : 0f; hv += (tg - hv) * 0.32f;
        if (Math.Abs(tg - hv) < 0.02f) { hv = tg; tm.Stop(); }
        Invalidate();
    }
    protected override void OnMouseEnter(EventArgs e) { hover = true; Kick(); }
    protected override void OnMouseLeave(EventArgs e) { hover = false; down = false; Kick(); }
    protected override void OnMouseDown(MouseEventArgs e) { down = true; Invalidate(); base.OnMouseDown(e); }
    protected override void OnMouseUp(MouseEventArgs e) { down = false; Invalidate(); base.OnMouseUp(e); }
    protected override void Dispose(bool d) { if (d && tm != null) tm.Dispose(); base.Dispose(d); }
    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
        var r = new Rectangle(0, 0, Width - 1, Height - 1);
        Color fill;
        if (Primary) fill = UI.Lerp(UI.ACC, UI.ACCH, hv);
        else if (Active) fill = UI.A(UI.ACC, 95);
        else if (Flat) fill = UI.A(HoverFill.IsEmpty ? UI.FILLH : HoverFill, (int)(hv * 255));
        else fill = UI.Lerp(UI.FILL, UI.FILLH, hv);
        using (var p = UI.Round(r, UI.P(Radius)))
        {
            using (var b = new SolidBrush(fill)) g.FillPath(b, p);
            if (down) using (var b = new SolidBrush(UI.A(Color.Black, 50))) g.FillPath(b, p);
        }
        TextRenderer.DrawText(g, Text, Font, new Rectangle(0, 0, Width, Height), (Primary || Active) ? Color.White : ForeColor,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
    }
}

class StatusPill : Ctl
{
    public Color Dot = UI.MUT; string msg = "";
    public void Set(string s, Color dot) { msg = s; Dot = dot; Invalidate(); }
    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
        var r = new Rectangle(0, 0, Width - 1, Height - 1);
        using (var p = UI.Round(r, Height - 1)) { using (var b = new SolidBrush(UI.DEEP)) g.FillPath(b, p); using (var pen = new Pen(UI.LINE)) g.DrawPath(pen, p); }
        int dr = UI.P(14), cx = UI.P(10), cy = Height / 2;
        using (var b = new SolidBrush(UI.A(Dot, 60))) g.FillEllipse(b, cx, cy - dr / 2, dr, dr);
        int ir = UI.P(6);
        using (var b = new SolidBrush(Dot)) g.FillEllipse(b, cx + (dr - ir) / 2, cy - ir / 2, ir, ir);
        TextRenderer.DrawText(g, msg, Font, new Rectangle(UI.P(32), 0, Width - UI.P(44), Height), UI.TXT,
            TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
    }
}

class WinItem
{
    public IntPtr H; public string Title, Proc; public Bitmap Icon;
}

static class IconCache
{
    static Dictionary<string, Bitmap> c = new Dictionary<string, Bitmap>();
    public static Bitmap For(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        Bitmap b;
        if (c.TryGetValue(path, out b)) return b;
        try { using (var ic = Icon.ExtractAssociatedIcon(path)) b = new Bitmap(ic.ToBitmap()); } catch { b = null; }
        c[path] = b; return b;
    }
}

delegate string BadgeFn(IntPtr h);

class WinList : Ctl
{
    public BadgeFn Badge;
    public List<WinItem> Items = new List<WinItem>();
    public event EventHandler SelChanged;
    int sel = -1, hov = -1, top = 0; bool sbDrag, sbHover; int dragDy;
    Font fT = UI.F(9.5f, false), fS = UI.F(8f, false), fL = UI.F(10f, true);
    static readonly Color[] pal = { ColorTranslator.FromHtml("#0A84FF"), ColorTranslator.FromHtml("#30D158"), ColorTranslator.FromHtml("#FF9F0A"),
        ColorTranslator.FromHtml("#FF375F"), ColorTranslator.FromHtml("#BF5AF2"), ColorTranslator.FromHtml("#5E5CE6"), ColorTranslator.FromHtml("#40C8E0") };
    int RH { get { return UI.P(38); } }
    public WinItem Selected { get { return sel >= 0 && sel < Items.Count ? Items[sel] : null; } }
#if PREVIEW
    public void PreviewSel(int i) { sel = i; Invalidate(); }
#endif
    int Total { get { return Items.Count * RH; } }
    void Clamp() { top = Math.Max(0, Math.Min(top, Math.Max(0, Total - Height))); }
    public void SetItems(List<WinItem> l, IntPtr keep)
    {
        Items = l; sel = -1; hov = -1; top = 0;
        for (int i = 0; i < l.Count; i++) if (l[i].H == keep) { sel = i; break; }
        EnsureVisible(); Invalidate();
    }
    public bool Select(IntPtr h)
    {
        for (int i = 0; i < Items.Count; i++)
            if (Items[i].H == h) { sel = i; EnsureVisible(); Invalidate(); if (SelChanged != null) SelChanged(this, EventArgs.Empty); return true; }
        return false;
    }
    public void Insert(WinItem w) { Items.Insert(0, w); sel = 0; top = 0; Invalidate(); if (SelChanged != null) SelChanged(this, EventArgs.Empty); }
    void EnsureVisible()
    {
        if (sel < 0) return;
        if (sel * RH < top) top = sel * RH;
        else if ((sel + 1) * RH > top + Height) top = (sel + 1) * RH - Height;
        Clamp();
    }
    int IndexAt(int y) { int i = (y + top) / RH; return (i >= 0 && i < Items.Count) ? i : -1; }
    Rectangle Thumb()
    {
        int th = Math.Max(UI.P(28), (int)((long)Height * Height / Math.Max(1, Total)));
        int ty = (Total - Height) > 0 ? (int)((long)(Height - th - UI.P(8)) * top / (Total - Height)) + UI.P(4) : UI.P(4);
        return new Rectangle(Width - UI.P(9), ty, UI.P(4), th);
    }
    protected override void OnMouseWheel(MouseEventArgs e) { top -= Math.Sign(e.Delta) * RH; Clamp(); Invalidate(); }
    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (Total > Height && e.X > Width - UI.P(18))
        {
            var t = Thumb(); sbDrag = true; dragDy = (e.Y >= t.Y && e.Y <= t.Bottom) ? e.Y - t.Y : t.Height / 2; MoveThumb(e.Y); return;
        }
        int i = IndexAt(e.Y);
        if (i >= 0) { sel = i; Invalidate(); if (SelChanged != null) SelChanged(this, EventArgs.Empty); }
    }
    void MoveThumb(int y)
    {
        var t = Thumb(); int range = Height - t.Height - UI.P(8);
        if (range > 0) { top = (int)((long)(y - dragDy - UI.P(4)) * (Total - Height) / range); Clamp(); Invalidate(); }
    }
    protected override void OnMouseMove(MouseEventArgs e)
    {
        if (sbDrag) { MoveThumb(e.Y); return; }
        int h = IndexAt(e.Y); bool sh = e.X > Width - UI.P(18);
        if (h != hov || sh != sbHover) { hov = h; sbHover = sh; Invalidate(); }
    }
    protected override void OnMouseUp(MouseEventArgs e) { sbDrag = false; }
    protected override void OnMouseLeave(EventArgs e) { hov = -1; sbHover = false; Invalidate(); }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias; g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        var r = new Rectangle(0, 0, Width - 1, Height - 1);
        using (var path = UI.Round(r, UI.P(14)))
        {
            using (var b = new SolidBrush(UI.DEEP)) g.FillPath(b, path);
            g.SetClip(path);
            if (Items.Count == 0)
                TextRenderer.DrawText(g, "沒有可選的視窗，按「重新整理」", fT, new Rectangle(0, 0, Width, Height), UI.MUT, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            var fl = TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix | TextFormatFlags.Left;
            int rh = RH, m = UI.P(6);
            for (int i = 0; i < Items.Count; i++)
            {
                int y = i * rh - top;
                if (y + rh < 0 || y > Height) continue;
                var row = new Rectangle(m, y + UI.P(2), Width - 2 * m - (Total > Height ? UI.P(8) : 0), rh - UI.P(4));
                if (i == sel)
                {
                    using (var p = UI.Round(row, UI.P(10))) using (var b = new SolidBrush(UI.A(UI.ACC, 48))) g.FillPath(b, p);
                    using (var p = UI.Round(new Rectangle(row.X, row.Y + UI.P(8), UI.P(3), row.Height - UI.P(16)), UI.P(3))) using (var b = new SolidBrush(UI.ACC)) g.FillPath(b, p);
                }
                else if (i == hov) using (var p = UI.Round(row, UI.P(10))) using (var b = new SolidBrush(UI.CARD2)) g.FillPath(b, p);

                var w = Items[i];
                int isz = UI.P(24);
                var ir = new Rectangle(row.X + UI.P(10), row.Y + (row.Height - isz) / 2, isz, isz);
                if (w.Icon != null) g.DrawImage(w.Icon, ir);
                else
                {
                    string pn = w.Proc ?? ""; int hash = 0; foreach (char ch in pn) hash = hash * 31 + ch;
                    Color pc = pal[Math.Abs(hash) % pal.Length];
                    using (var p = UI.Round(ir, UI.P(8))) using (var b = new SolidBrush(pc)) g.FillPath(b, p);
                    string c1 = pn.Length == 0 ? "?" : pn.Substring(0, 1).ToUpper();
                    TextRenderer.DrawText(g, c1, fL, ir, Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }
                int tx = row.X + UI.P(46), tw = row.Width - UI.P(54);
                string badge = null;
                try { if (Badge != null) badge = Badge(w.H); } catch { }
                if (!string.IsNullOrEmpty(badge))
                {
                    int bw = TextRenderer.MeasureText(g, badge, fS, new Size(1000, 100), TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix).Width + UI.P(14);
                    var br = new Rectangle(row.Right - bw - UI.P(8), row.Y + (row.Height - UI.P(18)) / 2, bw, UI.P(18));
                    Color bc = badge.IndexOf("穿透") >= 0 ? UI.ORANGE : UI.ACC;
                    using (var p = UI.Round(br, br.Height)) using (var b = new SolidBrush(UI.A(bc, 48))) g.FillPath(b, p);
                    TextRenderer.DrawText(g, badge, fS, br, bc, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);
                    tw -= bw + UI.P(10);
                }
                TextRenderer.DrawText(g, w.Title, fT, new Rectangle(tx, row.Y + UI.P(1), tw, UI.P(18)), UI.TXT, fl);
                TextRenderer.DrawText(g, w.Proc, fS, new Rectangle(tx, row.Y + UI.P(17), tw, UI.P(14)), UI.MUT, fl);
            }
            if (Total > Height)
            {
                var t = Thumb();
                using (var p = UI.Round(t, t.Width)) using (var b = new SolidBrush(sbHover || sbDrag ? UI.MUT : UI.TRACK)) g.FillPath(b, p);
            }
            g.ResetClip();
            using (var pen = new Pen(UI.LINE)) g.DrawPath(pen, path);
        }
    }
}

class MainForm : Form
{
    Font fSmall = UI.F(8.5f, false), fBold = UI.F(10f, true), fTitle = UI.F(12.5f, true), fMain = UI.F(9.5f, false), fChip = UI.F(8.5f, true), fBig = UI.FN(21f);
    WinList list; Slider sl; Switch swLock, swTop; Label lblVal, lblCount; StatusPill pill;
    Btn bPick, bRef, bRst, bRstAll; List<Btn> chips = new List<Btn>();
    Bitmap logo; Icon appIcon;
    IntPtr target = IntPtr.Zero;
    Dictionary<IntPtr, int> orig = new Dictionary<IntPtr, int>();
    bool syncing, exiting, picking, dwmRound;
    Rectangle[] cards;

    static Rectangle R(int x, int y, int w, int h) { return new Rectangle(UI.P(x), UI.P(y), UI.P(w), UI.P(h)); }
    static T At<T>(T c, int x, int y, int w, int h) where T : Control { c.SetBounds(UI.P(x), UI.P(y), UI.P(w), UI.P(h)); return c; }

    protected override CreateParams CreateParams { get { var cp = base.CreateParams; cp.ClassStyle |= 0x20000; cp.Style |= 0x20000 | 0x80000; return cp; } }

    Label L(string t, int x, int y, int w, int h, Color c, Font f, ContentAlignment a)
    {
        var l = new Label { Text = t, ForeColor = c, Font = f, BackColor = Color.Transparent, TextAlign = a, AutoEllipsis = true, UseMnemonic = false };
        At(l, x, y, w, h); Controls.Add(l); return l;
    }

    public MainForm()
    {
        Text = "視窗透明器"; FormBorderStyle = FormBorderStyle.None; StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(UI.P(380), UI.P(708)); BackColor = UI.BG1; Font = fMain; DoubleBuffered = true;
        TopMost = true; ShowInTaskbar = true; KeyPreview = true;
        cards = new Rectangle[] { R(18, 256, 344, 106), R(18, 372, 344, 60), R(18, 440, 344, 48), R(18, 496, 344, 76) };
        logo = UI.LoadPng("icon96.png");
        if (logo != null) { try { appIcon = Icon.FromHandle(new Bitmap(logo, 32, 32).GetHicon()); Icon = appIcon; } catch { } }

        var title = L("視窗透明器", 66, 13, 200, 24, UI.TXT, fTitle, ContentAlignment.MiddleLeft);
        var sub = L("Always-on-Top  ·  透明 / 穿透 / 置頂", 66, 37, 220, 16, UI.MUT, fSmall, ContentAlignment.MiddleLeft);
        var bMin = At(new Btn("—") { Flat = true, Font = fMain, Radius = 10 }, 296, 14, 32, 28);
        var bClose = At(new Btn("✕") { Flat = true, Font = fMain, Radius = 10, HoverFill = UI.RED }, 330, 14, 32, 28);
        bMin.Click += delegate { WindowState = FormWindowState.Minimized; };
        bClose.Click += delegate { Close(); };
        Controls.Add(bMin); Controls.Add(bClose);
        MouseDown += Drag; title.MouseDown += Drag; sub.MouseDown += Drag;

        L("目標視窗", 20, 64, 120, 18, UI.MUT, fSmall, ContentAlignment.MiddleLeft);
        lblCount = L("", 240, 64, 122, 18, UI.MUT, fSmall, ContentAlignment.MiddleRight);
        list = At(new WinList(), 18, 86, 344, 114);
        list.SelChanged += delegate { OnSelect(); };
        list.Badge = BadgeFor;
        Controls.Add(list);

        bPick = At(new Btn("⌖   拖曳準星到目標視窗") { Primary = true, Font = fBold }, 18, 210, 222, 38);
        bPick.MouseDown += delegate { picking = true; bPick.Capture = true; Cursor = Cursors.Cross; SetStatus("放開滑鼠在目標視窗上…", UI.ACC); };
        bPick.MouseUp += PickUp;
        bRef = At(new Btn("↻   重新整理") { Font = fMain }, 248, 210, 114, 38);
        bRef.Click += delegate { RefreshList(); };
        Controls.Add(bPick); Controls.Add(bRef);

        L("透明度", 36, 266, 100, 22, UI.TXT, fBold, ContentAlignment.MiddleLeft);
        lblVal = L("100%", 236, 262, 112, 30, UI.TXT, fBig, ContentAlignment.MiddleRight);
        sl = At(new Slider(), 26, 294, 328, 34);
        sl.Changed += delegate { UpdateVal(); Apply(); };
        Controls.Add(sl);
        int[] presets = { 0, 25, 50, 75, 100 };
        for (int i = 0; i < presets.Length; i++)
        {
            var c = At(new Btn(presets[i] + "%") { Font = fChip, Radius = 10, Tag = presets[i] }, 36 + i * 64, 330, 56, 24);
            c.Click += delegate(object s, EventArgs ev) { sl.Value = (int)((Btn)s).Tag; UpdateVal(); Apply(); };
            chips.Add(c); Controls.Add(c);
        }

        L("鎖定互動", 36, 380, 200, 22, UI.TXT, fBold, ContentAlignment.MiddleLeft);
        L("滑鼠穿透視窗，直接操作下方程式", 36, 402, 250, 18, UI.MUT, fSmall, ContentAlignment.MiddleLeft);
        swLock = new Switch(); swLock.Left = UI.P(305); swLock.Top = UI.P(387); swLock.Changed += delegate { Apply(); }; Controls.Add(swLock);

        L("保持置頂", 36, 451, 200, 24, UI.TXT, fBold, ContentAlignment.MiddleLeft);
        swTop = new Switch(); swTop.Left = UI.P(305); swTop.Top = UI.P(450); swTop.Changed += delegate { Apply(); }; Controls.Add(swTop);

        L("視窗位置", 36, 506, 120, 22, UI.TXT, fBold, ContentAlignment.MiddleLeft);
        L("依視窗大小貼齊螢幕", 200, 506, 148, 22, UI.MUT, fSmall, ContentAlignment.MiddleRight);
        string[] pn = { "↖ 左上", "↗ 右上", "◎ 置中", "↙ 左下", "↘ 右下" };
        for (int i = 0; i < 5; i++)
        {
            var c = At(new Btn(pn[i]) { Font = fChip, Radius = 10, Tag = i }, 36 + i * 64, 534, 56, 28);
            c.Click += delegate(object s, EventArgs ev) { SnapTo((int)((Btn)s).Tag); };
            Controls.Add(c);
        }

        bRst = At(new Btn("還原選定視窗") { Font = fMain }, 18, 582, 168, 38);
        bRstAll = At(new Btn("全部還原") { Font = fMain }, 194, 582, 168, 38);
        bRst.Click += delegate { if (target != IntPtr.Zero) { Restore(target); SyncFromWindow(); list.Invalidate(); SetStatus("已還原選定視窗", UI.GREEN); } };
        bRstAll.Click += delegate { RestoreAll(); };
        Controls.Add(bRst); Controls.Add(bRstAll);

        pill = At(new StatusPill { Font = fMain }, 18, 630, 344, 32);
        Controls.Add(pill); SetStatus("請先選擇目標視窗", UI.MUT);
        L("快捷鍵  Ctrl + Alt +\nL 鎖定　↑ ↓ 透明度　H 呼出面板　R 全部還原", 20, 668, 342, 36, UI.MUT, fSmall, ContentAlignment.TopLeft);

        RefreshList();
    }

    void Drag(object s, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left && e.Y < UI.P(60)) { N.ReleaseCapture(); N.SendMessage(Handle, 0xA1, 2, 0); }
    }

    // 把面板拉到所有置頂視窗的最上層
    void Raise()
    {
        try { N.SetWindowPos(Handle, N.HWND_TOPMOST, 0, 0, 0, 0, N.SWP_NOMOVE | N.SWP_NOSIZE | N.SWP_NOACTIVATE); } catch { }
    }

    void ShowPanel()
    {
        Show(); WindowState = FormWindowState.Normal; Raise(); Activate();
        try { N.SetForegroundWindow(Handle); } catch { }
    }

    void SetStatus(string s, Color c) { if (pill != null) pill.Set(s, c); }
    void UpdateVal()
    {
        lblVal.Text = sl.Value + "%";
        foreach (var c in chips) { bool a = (int)c.Tag == sl.Value; if (c.Active != a) { c.Active = a; c.Invalidate(); } }
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        using (var b = new LinearGradientBrush(ClientRectangle, UI.BG0, UI.BG1, LinearGradientMode.Vertical)) e.Graphics.FillRectangle(b, ClientRectangle);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias; g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        foreach (var r in cards)
        {
            var rr = new Rectangle(r.X, r.Y, r.Width - 1, r.Height - 1);
            using (var p = UI.Round(rr, UI.P(16))) { using (var b = new SolidBrush(UI.CARD)) g.FillPath(b, p); using (var pen = new Pen(UI.LINE)) g.DrawPath(pen, p); }
        }
        if (logo != null)
        {
            var lr = R(20, 12, 36, 36);
            using (var p = UI.Round(lr, UI.P(10))) { g.SetClip(p); g.DrawImage(logo, lr); g.ResetClip(); using (var pen = new Pen(UI.A(Color.White, 28))) g.DrawPath(pen, p); }
        }
        if (!dwmRound)
            using (var p = UI.Round(new Rectangle(0, 0, Width - 1, Height - 1), UI.P(28))) using (var pen = new Pen(UI.LINE)) g.DrawPath(pen, p);
    }

    static string ProcPath(IntPtr h)
    {
        try
        {
            uint pid; N.GetWindowThreadProcessId(h, out pid);
            IntPtr p = N.OpenProcess(0x1000, false, pid);
            if (p == IntPtr.Zero) return null;
            try { var sb = new StringBuilder(1024); int n = sb.Capacity; return N.QueryFullProcessImageName(p, 0, sb, ref n) ? sb.ToString() : null; }
            finally { N.CloseHandle(p); }
        }
        catch { return null; }
    }

    static string TitleOf(IntPtr h)
    {
        int n = N.GetWindowTextLength(h); var sb = new StringBuilder(n + 1); N.GetWindowText(h, sb, sb.Capacity); return sb.ToString();
    }

    static WinItem MakeItem(IntPtr h)
    {
        string path = ProcPath(h);
        string name = path != null ? Path.GetFileName(path) : "";
        return new WinItem { H = h, Title = TitleOf(h), Proc = name, Icon = IconCache.For(path) };
    }

    void RefreshList()
    {
        var items = new List<WinItem>();
        try
        {
            N.EnumWindows(delegate(IntPtr h, IntPtr l)
            {
                if (h == Handle || !N.IsWindowVisible(h) || N.GetWindowTextLength(h) == 0) return true;
                int ex = N.GetWindowLong(h, N.GWL_EXSTYLE);
                if ((ex & N.WS_EX_TOOLWINDOW) != 0 && (ex & N.WS_EX_APPWINDOW) == 0) return true;
                try { int c; if (N.DwmGetWindowAttribute(h, 14, out c, 4) == 0 && c != 0) return true; } catch { }
                if (TitleOf(h) == "Program Manager") return true;
                items.Add(MakeItem(h));
                return true;
            }, IntPtr.Zero);
        }
        catch { }
#if PREVIEW
        items.Add(new WinItem { H = new IntPtr(1), Title = "Windows視窗透明度和鎖定工具 - Claude - Google Chrome", Proc = "chrome.exe" });
        items.Add(new WinItem { H = new IntPtr(2), Title = "某某掛機遊戲", Proc = "game.exe" });
        items.Add(new WinItem { H = new IntPtr(3), Title = "YouTube - 影片播放", Proc = "msedge.exe" });
        items.Add(new WinItem { H = new IntPtr(4), Title = "檔案總管", Proc = "explorer.exe" });
        items.Add(new WinItem { H = new IntPtr(5), Title = "Discord", Proc = "Discord.exe" });
#endif
        list.SetItems(items, target);
        lblCount.Text = items.Count + " 個視窗";
    }

    void PickUp(object s, MouseEventArgs e)
    {
        if (!picking) return;
        picking = false; Cursor = Cursors.Default;
        var pt = Cursor.Position;
        IntPtr h = N.GetAncestor(N.WindowFromPoint(new N.POINT { X = pt.X, Y = pt.Y }), 2);
        if (h == IntPtr.Zero || h == Handle) { SetStatus("沒有選到有效視窗，再試一次", UI.MUT); return; }
        RefreshList();
        if (!list.Select(h)) list.Insert(MakeItem(h));
    }

    void OnSelect()
    {
        var w = list.Selected;
        if (w == null) return;
        target = w.H; SyncFromWindow();
    }

    void SyncFromWindow()
    {
        if (target == IntPtr.Zero || !N.IsWindow(target)) return;
        syncing = true;
        int ex = N.GetWindowLong(target, N.GWL_EXSTYLE); int pct = 100;
        if ((ex & N.WS_EX_LAYERED) != 0)
        {
            uint k, f; byte a;
            if (N.GetLayeredWindowAttributes(target, out k, out a, out f) && (f & 2) != 0) pct = (int)Math.Round(a * 100.0 / 255);
        }
        sl.Value = Math.Max(0, Math.Min(100, pct)); UpdateVal();
        swLock.SetOn((ex & N.WS_EX_TRANSPARENT) != 0, true);
        swTop.SetOn((ex & N.WS_EX_TOPMOST) != 0, true);
        syncing = false;
        SetStatus("已選定：" + TitleOf(target), UI.ACC);
    }

    void Apply()
    {
        if (syncing) return;
        if (target == IntPtr.Zero || !N.IsWindow(target)) { SetStatus("請先選擇有效的目標視窗", UI.MUT); return; }
        int ex = N.GetWindowLong(target, N.GWL_EXSTYLE);
        if (!orig.ContainsKey(target)) orig[target] = ex;
        int nex = ex | N.WS_EX_LAYERED;
        nex = swLock.On ? (nex | N.WS_EX_TRANSPARENT) : (nex & ~N.WS_EX_TRANSPARENT);
        if (nex != ex) N.SetWindowLong(target, N.GWL_EXSTYLE, nex);
        N.SetLayeredWindowAttributes(target, 0, (byte)(sl.Value * 255 / 100), 2);
        bool isTop = (ex & N.WS_EX_TOPMOST) != 0;
        if (isTop != swTop.On) N.SetWindowPos(target, swTop.On ? N.HWND_TOPMOST : N.HWND_NOTOPMOST, 0, 0, 0, 0, N.SWP_NOMOVE | N.SWP_NOSIZE | N.SWP_NOACTIVATE);
        list.Invalidate();
        if (WindowState != FormWindowState.Minimized) Raise(); // 面板永遠在被置頂的目標視窗之上
        string s = sl.Value + "%" + (sl.Value == 0 ? "（完全隱形）" : "") + "　" + (swLock.On ? "鎖定穿透" : "可互動") + (swTop.On ? "　置頂" : "");
        SetStatus(s, swLock.On ? UI.ORANGE : UI.GREEN);
    }

    void SnapTo(int pos)
    {
        if (target == IntPtr.Zero || !N.IsWindow(target)) { SetStatus("請先選擇有效的目標視窗", UI.MUT); return; }
        if (N.IsZoomed(target) || N.IsIconic(target)) { SetStatus("視窗為最大化／最小化，無法貼齊", UI.MUT); return; }
        IntPtr oldCtx = IntPtr.Zero; bool ctx = false;
        try { oldCtx = N.SetThreadDpiAwarenessContext(new IntPtr(-4)); ctx = true; } catch { }
        try
        {
            N.RECT r; if (!N.GetWindowRect(target, out r)) return;
            N.RECT f; if (N.DwmGetWindowAttribute(target, 9, out f, 16) != 0) f = r; // 實際可見邊界（排除隱形陰影）
            var mi = new N.MONITORINFO(); mi.cb = Marshal.SizeOf(typeof(N.MONITORINFO));
            if (!N.GetMonitorInfo(N.MonitorFromWindow(target, 2), ref mi)) return;
            int offL = f.L - r.L, offT = f.T - r.T;
            int w = f.R - f.L, h = f.B - f.T;
            var wa = mi.work;
            int vx, vy;
            switch (pos)
            {
                case 0: vx = wa.L; vy = wa.T; break;
                case 1: vx = wa.R - w; vy = wa.T; break;
                case 2: vx = wa.L + ((wa.R - wa.L) - w) / 2; vy = wa.T + ((wa.B - wa.T) - h) / 2; break;
                case 3: vx = wa.L; vy = wa.B - h; break;
                default: vx = wa.R - w; vy = wa.B - h; break;
            }
            N.SetWindowPos(target, IntPtr.Zero, vx - offL, vy - offT, 0, 0, N.SWP_NOSIZE | N.SWP_NOZORDER | N.SWP_NOACTIVATE);
            string[] nm = { "左上角", "右上角", "置中", "左下角", "右下角" };
            SetStatus("已貼齊：" + nm[pos], UI.GREEN);
        }
        catch { SetStatus("貼齊失敗", UI.MUT); }
        finally { if (ctx) { try { N.SetThreadDpiAwarenessContext(oldCtx); } catch { } } }
        if (WindowState != FormWindowState.Minimized) Raise();
    }

    // 清單右側的狀態標籤：顯示該視窗目前被這個工具改了什麼
    string BadgeFor(IntPtr h)
    {
#if PREVIEW
        if (h == new IntPtr(2)) return "穿透 · 50% · 置頂";
        if (h == new IntPtr(3)) return "75%";
        return null;
#else
        if (!orig.ContainsKey(h) || !N.IsWindow(h)) return null;
        int ex = N.GetWindowLong(h, N.GWL_EXSTYLE); var parts = new List<string>();
        if ((ex & N.WS_EX_TRANSPARENT) != 0) parts.Add("穿透");
        if ((ex & N.WS_EX_LAYERED) != 0)
        {
            uint k, f; byte a;
            if (N.GetLayeredWindowAttributes(h, out k, out a, out f) && (f & 2) != 0)
            {
                int pct = (int)Math.Round(a * 100.0 / 255);
                if (pct < 100) parts.Add(pct + "%");
            }
        }
        if ((ex & N.WS_EX_TOPMOST) != 0) parts.Add("置頂");
        return parts.Count == 0 ? "已調整" : string.Join(" · ", parts.ToArray());
#endif
    }

    void Restore(IntPtr h)
    {
        int ex;
        if (!orig.TryGetValue(h, out ex)) return;
        orig.Remove(h);
        if (!N.IsWindow(h)) return;
        N.SetLayeredWindowAttributes(h, 0, 255, 2);
        int cur = N.GetWindowLong(h, N.GWL_EXSTYLE);
        N.SetWindowLong(h, N.GWL_EXSTYLE, (ex & ~N.WS_EX_TOPMOST) | (cur & N.WS_EX_TOPMOST));
        if (((ex ^ cur) & N.WS_EX_TOPMOST) != 0)
            N.SetWindowPos(h, (ex & N.WS_EX_TOPMOST) != 0 ? N.HWND_TOPMOST : N.HWND_NOTOPMOST, 0, 0, 0, 0, N.SWP_NOMOVE | N.SWP_NOSIZE | N.SWP_NOACTIVATE);
    }

    void RestoreAll()
    {
        foreach (var h in new List<IntPtr>(orig.Keys)) Restore(h);
        SyncFromWindow(); list.Invalidate(); SetStatus("已全部還原", UI.GREEN);
        if (WindowState != FormWindowState.Minimized) Raise();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (!exiting)
        {
            exiting = true; // 關閉前還原所有被改過的視窗，避免留下點不到的穿透視窗
            foreach (var h in new List<IntPtr>(orig.Keys)) Restore(h);
        }
        base.OnFormClosing(e);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape) WindowState = FormWindowState.Minimized;
        base.OnKeyDown(e);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        try
        {
            int v = 2; dwmRound = N.DwmSetWindowAttribute(Handle, 33, ref v, 4) == 0; // Win11 圓角
            if (dwmRound) { int col = ColorTranslator.ToWin32(UI.LINE); N.DwmSetWindowAttribute(Handle, 34, ref col, 4); }
        }
        catch { dwmRound = false; }
        if (!dwmRound)
        {
            try { Region = Region.FromHrgn(N.CreateRoundRectRgn(0, 0, Width + 1, Height + 1, UI.P(28), UI.P(28))); } catch { }
        }
        var bad = new List<string>();
        try
        {
            if (!N.RegisterHotKey(Handle, 1, 3, 0x4C)) bad.Add("L");
            if (!N.RegisterHotKey(Handle, 2, 3, 0x26)) bad.Add("↑");
            if (!N.RegisterHotKey(Handle, 3, 3, 0x28)) bad.Add("↓");
            if (!N.RegisterHotKey(Handle, 4, 3, 0x48)) bad.Add("H");
            if (!N.RegisterHotKey(Handle, 5, 3, 0x52)) bad.Add("R");
        }
        catch { }
        if (bad.Count > 0) SetStatus("快捷鍵 " + string.Join("/", bad.ToArray()) + " 被其他程式佔用", UI.MUT);
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        try { for (int i = 1; i <= 5; i++) N.UnregisterHotKey(Handle, i); } catch { }
        base.OnHandleDestroyed(e);
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == 0x0312)
        {
            switch (m.WParam.ToInt32())
            {
                case 1: swLock.SetOn(!swLock.On, true); Apply(); break;
                case 2: sl.Value += 10; UpdateVal(); Apply(); break;
                case 3: sl.Value -= 10; UpdateVal(); Apply(); break;
                case 4: if (WindowState != FormWindowState.Minimized && Form.ActiveForm == this) WindowState = FormWindowState.Minimized; else ShowPanel(); break;
                case 5: RestoreAll(); break;
            }
        }
        base.WndProc(ref m);
    }

    [STAThread]
    static void Main(string[] args)
    {
        bool created;
        using (var mx = new Mutex(true, "AlwaysOnTop_Singleton_7F3A", out created))
        {
            if (!created) return;
#if PREVIEW
            var sc = Environment.GetEnvironmentVariable("SCALE");
            if (sc != null) UI.S = float.Parse(sc);
#else
            try { N.SetProcessDPIAware(); } catch { } // 宣告支援高 DPI，避免被系統放大而模糊
            try { using (var g = Graphics.FromHwnd(IntPtr.Zero)) UI.S = Math.Max(1f, g.DpiX / 96f); } catch { }
#endif
            Application.EnableVisualStyles();
#if PREVIEW
            if (args.Length > 1 && args[0] == "--preview")
            {
                var f = new MainForm(); f.StartPosition = FormStartPosition.Manual; f.Location = new Point(0, 0);
                f.Show(); f.list.PreviewSel(1); f.sl.Value = 50; f.UpdateVal(); f.swLock.SetOn(true, false); f.swTop.SetOn(false, false);
                f.SetStatus("50%　鎖定穿透", UI.ORANGE);
                Application.Run(f); return;
            }
#endif
            Application.Run(new MainForm());
        }
    }
}
