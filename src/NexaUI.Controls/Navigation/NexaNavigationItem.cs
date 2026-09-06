using System.ComponentModel;
using NexaUI.Core;
using NexaUI.Icons;

namespace NexaUI.Controls;

/// <summary>
/// Represents a single navigation item in a <see cref="NexaNavigationBar"/>.
/// </summary>
public class NexaNavigationItem : Component
{
    private string _text = string.Empty;
    private string _key = string.Empty;
    private NexaIconKind _iconKind = NexaIconKind.None;
    private bool _enabled = true;
    private bool _visible = true;
    private bool _selected = false;
    private string _badgeText = string.Empty;
    private bool _badgeVisible = false;
    private object? _tag;

    /// <summary>Initializes a new instance of the <see cref="NexaNavigationItem"/> class.</summary>
    public NexaNavigationItem() { }

    /// <summary>Initializes a new instance with key and text.</summary>
    public NexaNavigationItem(string key, string text)
    {
        _key = key ?? string.Empty;
        _text = text ?? string.Empty;
    }

    /// <summary>Unique key identifying the item.</summary>
    [Category("NexaUI")]
    [DefaultValue("")]
    [Description("Unique key identifying the item.")]
    public string Key
    {
        get => _key;
        set { _key = value ?? string.Empty; }
    }

    /// <summary>Display text for the item.</summary>
    [Category("NexaUI")]
    [DefaultValue("")]
    [Description("Display text for the item.")]
    public string Text
    {
        get => _text;
        set { _text = value ?? string.Empty; }
    }

    /// <summary>Icon to display alongside the text.</summary>
    [Category("NexaUI")]
    [DefaultValue(NexaIconKind.None)]
    [Description("Icon to display alongside the text.")]
    public NexaIconKind IconKind
    {
        get => _iconKind;
        set { _iconKind = value; }
    }

    /// <summary>Whether the item is enabled.</summary>
    [Category("NexaUI")]
    [DefaultValue(true)]
    [Description("Whether the item is enabled.")]
    public bool Enabled
    {
        get => _enabled;
        set { _enabled = value; }
    }

    /// <summary>Whether the item is visible.</summary>
    [Category("NexaUI")]
    [DefaultValue(true)]
    [Description("Whether the item is visible.")]
    public bool Visible
    {
        get => _visible;
        set { _visible = value; }
    }

    /// <summary>Whether the item is currently selected.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Selected
    {
        get => _selected;
        internal set { _selected = value; }
    }

    /// <summary>Optional badge text (e.g., notification count).</summary>
    [Category("NexaUI")]
    [DefaultValue("")]
    [Description("Optional badge text (e.g., notification count).")]
    public string BadgeText
    {
        get => _badgeText;
        set { _badgeText = value ?? string.Empty; }
    }

    /// <summary>Whether the badge is visible.</summary>
    [Category("NexaUI")]
    [DefaultValue(false)]
    [Description("Whether the badge is visible.")]
    public bool BadgeVisible
    {
        get => _badgeVisible;
        set { _badgeVisible = value; }
    }

    /// <summary>User-defined data associated with the item.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public object? Tag
    {
        get => _tag;
        set { _tag = value; }
    }
}