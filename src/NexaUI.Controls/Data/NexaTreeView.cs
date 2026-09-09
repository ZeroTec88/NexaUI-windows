using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using NexaUI.Core;
using NexaUI.Themes;

namespace NexaUI.Controls;

/// <summary>
/// A theme-aware TreeView that preserves all native functionality while adding
/// modern NexaUI visual styling. Inherits from the native <see cref="TreeView"/>
/// so nodes, parent/child relationships, selection, checkboxes, image lists,
/// label editing, sorting, drag/drop, and all native behaviors work exactly as expected.
/// </summary>
[DefaultEvent(nameof(AfterSelect))]
[DefaultProperty(nameof(Nodes))]
[ToolboxBitmap(typeof(TreeView))]
public class NexaTreeView : TreeView
{
    private NexaTreeStyle _treeStyle = NexaTreeStyle.Default;
    private bool _showRootLines = true;
    private bool _showNodeLines = true;
    private bool _ownerDrawInitialized = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="NexaTreeView"/> class.
    /// </summary>
    public NexaTreeView()
    {
        HideSelection = false;
        BorderStyle = BorderStyle.None;
        ShowLines = true;
        ShowPlusMinus = true;
        ShowRootLines = true;
        FullRowSelect = true;
        HotTracking = true;
        DrawMode = TreeViewDrawMode.OwnerDrawText;

        ThemeManager.ThemeChanged += OnThemeChanged;
        Disposed += (_, _) => ThemeManager.ThemeChanged -= OnThemeChanged;

        RefreshTheme();
    }

    /// <summary>
    /// Gets or sets the visual density style of the tree view.
    /// </summary>
    [Category("NexaUI")]
    [DefaultValue(NexaTreeStyle.Default)]
    [Description("Visual density style of the tree view.")]
    public NexaTreeStyle TreeStyle
    {
        get => _treeStyle;
        set
        {
            if (_treeStyle == value) return;
            _treeStyle = value;
            ApplyTreeStyle();
            Invalidate();
        }
    }

    /// <summary>
    /// Gets or sets whether root lines are shown.
    /// </summary>
    [Category("NexaUI")]
    [DefaultValue(true)]
    [Description("Show lines connecting root nodes.")]
    public new bool ShowRootLines
    {
        get => _showRootLines;
        set
        {
            if (_showRootLines == value) return;
            _showRootLines = value;
            base.ShowRootLines = value;
            Invalidate();
        }
    }

    /// <summary>
    /// Gets or sets whether node lines are shown.
    /// </summary>
    [Category("NexaUI")]
    [DefaultValue(true)]
    [Description("Show lines connecting child nodes.")]
    public new bool ShowNodeLines
    {
        get => _showNodeLines;
        set
        {
            if (_showNodeLines == value) return;
            _showNodeLines = value;
            base.ShowLines = value;
            Invalidate();
        }
    }

    private void ApplyTreeStyle()
    {
        var dpi = GetCurrentDpi();
        var indent = _treeStyle switch
        {
            NexaTreeStyle.Compact => NexaDpi.Scale(12, dpi),
            NexaTreeStyle.Spacious => NexaDpi.Scale(24, dpi),
            _ => NexaDpi.Scale(19, dpi)
        };
        Indent = indent;
        ItemHeight = Math.Max(ItemHeight, NexaDpi.Scale(22, dpi));
    }

    private void RefreshTheme()
    {
        if (IsDisposed || Disposing) return;
        var theme = ThemeManager.Current;
        var palette = theme.Palette;
        var dpi = GetCurrentDpi();

        BackColor = (Color)palette[NexaColorRole.Surface].Value;
        ForeColor = (Color)palette[NexaColorRole.TextPrimary].Value;

        LineColor = (Color)palette[NexaColorRole.Border].Value;

        DrawMode = TreeViewDrawMode.OwnerDrawText;
        if (!_ownerDrawInitialized)
        {
            DrawNode += OnDrawNode;
            _ownerDrawInitialized = true;
        }

        ApplyTreeStyle();
        Invalidate();
    }

    private void OnDrawNode(object? sender, DrawTreeNodeEventArgs e)
    {
        var theme = ThemeManager.Current;
        var palette = theme.Palette;
        var dpi = GetCurrentDpi();

        var node = e.Node;
        var isSelected = (e.State & TreeNodeStates.Selected) == TreeNodeStates.Selected;
        var isHot = (e.State & TreeNodeStates.Hot) == TreeNodeStates.Hot;

        Color fg;
        if (isSelected && Focused)
        {
            fg = (Color)palette[NexaColorRole.TextOnAccent].Value;
        }
        else if (isSelected && !Focused)
        {
            fg = (Color)palette[NexaColorRole.Primary].Value;
        }
        else if (isHot)
        {
            fg = (Color)palette[NexaColorRole.TextPrimary].Value;
        }
        else
        {
            fg = (Color)palette[NexaColorRole.TextPrimary].Value;
        }

        using var font = theme.Typography.ToFont(NexaTypographyRole.Body, dpi);
        TextRenderer.DrawText(
            e.Graphics,
            node.Text,
            font,
            e.Bounds,
            fg,
            TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine);
    }

    private void DrawTreeLines(Graphics g, TreeNode node, Rectangle bounds, int x, int lineWidth, Color lineColor, float dpi)
    {
        using var pen = new Pen(lineColor, lineWidth);

        var level = GetNodeLevel(node);
        var indentSize = Indent;
        var nodeTop = bounds.Top;
        var nodeBottom = bounds.Bottom;
        var nodeMiddle = bounds.Top + bounds.Height / 2;

        if (_showRootLines || level > 0)
        {
            for (int i = 0; i < level; i++)
            {
                var lineX = bounds.Left + i * indentSize + Indent / 2;
                var isLast = IsLastNode(node, i);
                if (!isLast || i == level - 1)
                {
                    g.DrawLine(pen, lineX, nodeTop, lineX, nodeBottom);
                }
            }
        }

        if (node.Nodes.Count > 0)
        {
            var plusMinusX = x - Indent / 2;
            g.DrawLine(pen, plusMinusX, nodeMiddle, plusMinusX + Indent / 4, nodeMiddle);
            if (!node.IsExpanded)
            {
                g.DrawLine(pen, plusMinusX + Indent / 4, nodeTop, plusMinusX + Indent / 4, nodeMiddle);
            }
        }
        else if (level > 0)
        {
            var lineX = x - Indent / 2;
            g.DrawLine(pen, lineX, nodeMiddle, lineX + Indent / 4, nodeMiddle);
        }
    }

    private void DrawPlusMinus(Graphics g, Rectangle rect, bool expanded, Color color, float dpi)
    {
        using var pen = new Pen(color, NexaDpi.Scale(2, dpi));
        var cx = rect.X + rect.Width / 2;
        var cy = rect.Y + rect.Height / 2;
        var half = rect.Width / 3;

        g.DrawLine(pen, cx - half, cy, cx + half, cy);
        if (!expanded)
        {
            g.DrawLine(pen, cx, cy - half, cx, cy + half);
        }
    }

    private void DrawCheckBox(Graphics g, Rectangle rect, bool checked_, Color color)
    {
        using var pen = new Pen(color, NexaDpi.Scale(2, GetCurrentDpi()));
        var r = rect;
        r.Width -= 1; r.Height -= 1;
        var radius = NexaDpi.Scale(2, GetCurrentDpi());

        using var path = CreateRoundedRect(r, radius);
        g.DrawPath(pen, path);

        if (checked_)
        {
            var cx = rect.X + rect.Width / 2;
            var cy = rect.Y + rect.Height / 2;
            var checkPoints = new Point[]
            {
                new(cx - rect.Width / 4, cy),
                new(cx - rect.Width / 8, cy + rect.Height / 4),
                new(cx + rect.Width / 3, cy - rect.Height / 4)
            };
            g.DrawLines(pen, checkPoints);
        }
    }

    private int GetNodeLevel(TreeNode node)
    {
        int level = 0;
        while (node.Parent != null)
        {
            node = node.Parent;
            level++;
        }
        return level;
    }

    private bool IsLastNode(TreeNode node, int ancestorLevel)
    {
        var ancestors = new System.Collections.Generic.List<TreeNode>();
        while (node.Parent != null)
        {
            ancestors.Add(node);
            node = node.Parent;
        }
        ancestors.Reverse();

        if (ancestorLevel >= ancestors.Count) return true;

        var targetAncestor = ancestors[ancestorLevel];
        return targetAncestor.Parent == null || targetAncestor.Parent.Nodes[^1] == targetAncestor;
    }

    private void OnThemeChanged(object? sender, ThemeChangedEventArgs e)
    {
        if (IsDisposed || Disposing) return;
        if (InvokeRequired)
        {
            BeginInvoke(() => { if (!IsDisposed && !Disposing) RefreshTheme(); });
            return;
        }
        RefreshTheme();
    }

    private float GetCurrentDpi()
    {
        if (!IsHandleCreated || DesignMode) return NexaDpi.BaseDpi;
        using var g = Graphics.FromHwnd(Handle);
        return g.DpiY;
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        RefreshTheme();
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        Invalidate();
    }

    protected override void OnAfterExpand(TreeViewEventArgs e)
    {
        base.OnAfterExpand(e);
        Invalidate();
    }

    protected override void OnAfterCollapse(TreeViewEventArgs e)
    {
        base.OnAfterCollapse(e);
        Invalidate();
    }

    private static GraphicsPath CreateRoundedRect(Rectangle rect, int radius)
    {
        var path = new GraphicsPath();
        if (radius <= 0 || rect.Width <= 0 || rect.Height <= 0)
        {
            path.AddRectangle(rect);
            return path;
        }

        var d = Math.Min(radius * 2, Math.Min(rect.Width, rect.Height));
        path.AddArc(rect.X, rect.Y, d, d, 180, 90);
        path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
        path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
        path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }
}

/// <summary>
/// Visual density styles for the NexaTreeView.
/// </summary>
public enum NexaTreeStyle
{
    /// <summary>Standard density with normal spacing.</summary>
    Default,
    /// <summary>Compact density with minimal padding.</summary>
    Compact,
    /// <summary>Spacious density with generous padding.</summary>
    Spacious
}