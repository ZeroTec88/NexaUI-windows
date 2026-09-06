using System.Drawing;

namespace NexaUI.Icons;

/// <summary>
/// Lightweight, vector-style fallback icon. Draws a simple geometric glyph for the
/// requested <see cref="NexaIconKind"/>. Used by demos until richer icon content is added.
/// </summary>
public sealed class NexaGlyphIcon : INexaIconSource
{
    public NexaGlyphIcon(NexaIconKind kind)
    {
        Kind = kind;
    }

    public NexaIconKind Kind { get; }

    public Bitmap? ToBitmap(Size size, Color tint)
    {
        if (size.Width <= 0 || size.Height <= 0) return null;

        var bmp = new Bitmap(size.Width, size.Height);
        using var graphics = Graphics.FromImage(bmp);
        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        graphics.Clear(Color.Transparent);

        using var pen = new Pen(tint, Math.Max(1F, Math.Min(size.Width, size.Height) / 12F));
        using var brush = new SolidBrush(tint);

        var rect = new RectangleF(pen.Width, pen.Width, size.Width - pen.Width * 2, size.Height - pen.Width * 2);
        switch (Kind)
        {
            case NexaIconKind.Check:
                graphics.DrawLines(pen, new[]
                {
                    new PointF(rect.Left, rect.Top + rect.Height / 2),
                    new PointF(rect.Left + rect.Width * 0.4F, rect.Bottom - pen.Width),
                    new PointF(rect.Right, rect.Top + pen.Width)
                });
                break;
            case NexaIconKind.Cross:
                graphics.DrawLine(pen, rect.Left, rect.Top, rect.Right, rect.Bottom);
                graphics.DrawLine(pen, rect.Right, rect.Top, rect.Left, rect.Bottom);
                break;
            case NexaIconKind.ChevronDown:
                graphics.DrawLines(pen, new[]
                {
                    new PointF(rect.Left, rect.Top + rect.Height * 0.25F),
                    new PointF(rect.Left + rect.Width / 2, rect.Bottom - pen.Width),
                    new PointF(rect.Right, rect.Top + rect.Height * 0.25F)
                });
                break;
            case NexaIconKind.ChevronRight:
                graphics.DrawLines(pen, new[]
                {
                    new PointF(rect.Left + rect.Width * 0.25F, rect.Top),
                    new PointF(rect.Right - pen.Width, rect.Top + rect.Height / 2),
                    new PointF(rect.Left + rect.Width * 0.25F, rect.Bottom)
                });
                break;
            case NexaIconKind.Search:
                graphics.DrawEllipse(pen, rect.Left, rect.Top, rect.Width * 0.7F, rect.Height * 0.7F);
                graphics.DrawLine(pen, rect.Left + rect.Width * 0.55F, rect.Top + rect.Height * 0.55F, rect.Right, rect.Bottom);
                break;
            case NexaIconKind.Settings:
                graphics.DrawEllipse(pen, rect.Left + rect.Width * 0.25F, rect.Top + rect.Height * 0.25F, rect.Width * 0.5F, rect.Height * 0.5F);
                graphics.DrawEllipse(pen, rect.Left + rect.Width * 0.4F, rect.Top + rect.Height * 0.4F, rect.Width * 0.2F, rect.Height * 0.2F);
                break;
            case NexaIconKind.User:
                graphics.DrawEllipse(pen, rect.Left + rect.Width * 0.3F, rect.Top, rect.Width * 0.4F, rect.Height * 0.4F);
                graphics.DrawArc(pen, rect.Left, rect.Top + rect.Height * 0.45F, rect.Width, rect.Height * 0.7F, 180, 180);
                break;
            case NexaIconKind.Info:
                graphics.DrawEllipse(pen, rect.Left, rect.Top, rect.Width, rect.Height);
                graphics.FillEllipse(brush, rect.Left + rect.Width * 0.45F - 1, rect.Top + rect.Height * 0.2F, 2 + pen.Width, 2 + pen.Width);
                graphics.FillRectangle(brush, rect.Left + rect.Width * 0.45F - 1, rect.Top + rect.Height * 0.4F, 2 + pen.Width, rect.Height * 0.3F);
                break;
            case NexaIconKind.Warning:
                var warnPts = new[]
                {
                    new PointF(rect.Left + rect.Width / 2, rect.Top),
                    new PointF(rect.Right, rect.Bottom),
                    new PointF(rect.Left, rect.Bottom)
                };
                graphics.DrawPolygon(pen, warnPts);
                graphics.FillEllipse(brush, rect.Left + rect.Width / 2 - pen.Width, rect.Top + rect.Height * 0.55F, 2 + pen.Width, 2 + pen.Width);
                graphics.FillRectangle(brush, rect.Left + rect.Width / 2 - pen.Width, rect.Top + rect.Height * 0.25F, 2 + pen.Width, rect.Height * 0.25F);
                break;
            case NexaIconKind.Error:
                graphics.DrawEllipse(pen, rect.Left, rect.Top, rect.Width, rect.Height);
                graphics.DrawLine(pen, rect.Left + rect.Width * 0.25F, rect.Top + rect.Height * 0.25F, rect.Right - rect.Width * 0.25F, rect.Bottom - rect.Height * 0.25F);
                graphics.DrawLine(pen, rect.Right - rect.Width * 0.25F, rect.Top + rect.Height * 0.25F, rect.Left + rect.Width * 0.25F, rect.Bottom - rect.Height * 0.25F);
                break;
            case NexaIconKind.Sun:
                graphics.DrawEllipse(pen, rect.Left + rect.Width * 0.25F, rect.Top + rect.Height * 0.25F, rect.Width * 0.5F, rect.Height * 0.5F);
                graphics.DrawLine(pen, rect.Left + rect.Width / 2, rect.Top, rect.Left + rect.Width / 2, rect.Top + rect.Height * 0.2F);
                graphics.DrawLine(pen, rect.Left + rect.Width / 2, rect.Bottom - rect.Height * 0.2F, rect.Left + rect.Width / 2, rect.Bottom);
                graphics.DrawLine(pen, rect.Left, rect.Top + rect.Height / 2, rect.Left + rect.Width * 0.2F, rect.Top + rect.Height / 2);
                graphics.DrawLine(pen, rect.Right - rect.Width * 0.2F, rect.Top + rect.Height / 2, rect.Right, rect.Top + rect.Height / 2);
                break;
            case NexaIconKind.Moon:
                graphics.FillEllipse(brush, rect.Left, rect.Top, rect.Width * 0.8F, rect.Height);
                graphics.FillEllipse(new SolidBrush(Color.Transparent), rect.Left + rect.Width * 0.3F, rect.Top - rect.Height * 0.1F, rect.Width * 0.7F, rect.Height * 1.1F);
                break;
            case NexaIconKind.Question:
                graphics.DrawEllipse(pen, rect.Left, rect.Top, rect.Width, rect.Height);
                graphics.FillEllipse(brush, rect.Left + rect.Width * 0.45F - 1, rect.Top + rect.Height * 0.15F, 2 + pen.Width, 2 + pen.Width);
                graphics.FillRectangle(brush, rect.Left + rect.Width * 0.45F - 1, rect.Top + rect.Height * 0.35F, 2 + pen.Width, rect.Height * 0.2F);
                graphics.FillEllipse(brush, rect.Left + rect.Width * 0.45F - 1, rect.Top + rect.Height * 0.65F, 2 + pen.Width, 2 + pen.Width);
                break;
            case NexaIconKind.Exclamation:
                graphics.DrawEllipse(pen, rect.Left, rect.Top, rect.Width, rect.Height);
                graphics.FillEllipse(brush, rect.Left + rect.Width * 0.45F - 1, rect.Top + rect.Height * 0.15F, 2 + pen.Width, rect.Height * 0.55F);
                graphics.FillEllipse(brush, rect.Left + rect.Width * 0.45F - 1, rect.Top + rect.Height * 0.75F, 2 + pen.Width, 2 + pen.Width);
                break;
            case NexaIconKind.Close:
                graphics.DrawLine(pen, rect.Left + rect.Width * 0.2F, rect.Top + rect.Height * 0.2F, rect.Right - rect.Width * 0.2F, rect.Bottom - rect.Height * 0.2F);
                graphics.DrawLine(pen, rect.Right - rect.Width * 0.2F, rect.Top + rect.Height * 0.2F, rect.Left + rect.Width * 0.2F, rect.Bottom - rect.Height * 0.2F);
                break;
            case NexaIconKind.Minimize:
                graphics.DrawLine(pen, rect.Left + rect.Width * 0.2F, rect.Top + rect.Height * 0.6F, rect.Right - rect.Width * 0.2F, rect.Top + rect.Height * 0.6F);
                break;
            case NexaIconKind.Maximize:
                graphics.DrawRectangle(pen, rect.Left + rect.Width * 0.2F, rect.Top + rect.Height * 0.2F, rect.Width * 0.6F, rect.Height * 0.6F);
                break;
            case NexaIconKind.Restore:
                graphics.DrawRectangle(pen, rect.Left + rect.Width * 0.15F, rect.Top + rect.Height * 0.15F, rect.Width * 0.55F, rect.Height * 0.55F);
                graphics.DrawRectangle(pen, rect.Left + rect.Width * 0.35F, rect.Top + rect.Height * 0.35F, rect.Width * 0.3F, rect.Height * 0.3F);
                break;
            case NexaIconKind.ChevronLeft:
                graphics.DrawLines(pen, new[]
                {
                    new PointF(rect.Right - rect.Width * 0.25F, rect.Top),
                    new PointF(rect.Left + pen.Width, rect.Top + rect.Height / 2),
                    new PointF(rect.Right - rect.Width * 0.25F, rect.Bottom)
                });
                break;
            case NexaIconKind.ChevronUp:
                graphics.DrawLines(pen, new[]
                {
                    new PointF(rect.Left, rect.Bottom - rect.Height * 0.25F),
                    new PointF(rect.Left + rect.Width / 2, rect.Top + pen.Width),
                    new PointF(rect.Right, rect.Bottom - rect.Height * 0.25F)
                });
                break;
        }

        return bmp;
    }
}