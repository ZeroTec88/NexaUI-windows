using System.ComponentModel;
using NexaUI.Core;

namespace NexaUI.Controls;

/// <summary>
/// Represents a single item in a <see cref="NexaBreadcrumb"/>.
/// </summary>
public class NexaBreadcrumbItem : Component
{
    private string _text = string.Empty;
    private string _key = string.Empty;
    private bool _enabled = true;
    private bool _visible = true;
    private object? _tag;

    /// <summary>Initializes a new instance of the <see cref="NexaBreadcrumbItem"/> class.</summary>
    public NexaBreadcrumbItem() { }

    /// <summary>Initializes a new instance with key and text.</summary>
    public NexaBreadcrumbItem(string key, string text)
    {
        _key = key ?? string.Empty;
        _text = text ?? string.Empty;
    }

    /// <summary>Display text for the breadcrumb item.</summary>
    [Category("NexaUI")]
    [DefaultValue("")]
    [Description("Display text for the breadcrumb item.")]
    public string Text
    {
        get => _text;
        set { _text = value ?? string.Empty; }
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

    /// <summary>Whether the item is enabled (clickable).</summary>
    [Category("NexaUI")]
    [DefaultValue(true)]
    [Description("Whether the item is enabled (clickable).")]
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

    /// <summary>User-defined data associated with the item.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public object? Tag
    {
        get => _tag;
        set { _tag = value; }
    }
}