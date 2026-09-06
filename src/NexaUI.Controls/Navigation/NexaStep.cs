using System.ComponentModel;
using NexaUI.Core;

namespace NexaUI.Controls;

/// <summary>
/// Represents a single step in a <see cref="NexaStepper"/>.
/// </summary>
public class NexaStep : Component
{
    private string _key = string.Empty;
    private string _title = string.Empty;
    private string _description = string.Empty;
    private NexaStepState _state = NexaStepState.Pending;
    private bool _enabled = true;
    private bool _visible = true;
    private object? _tag;

    /// <summary>Initializes a new instance of the <see cref="NexaStep"/> class.</summary>
    public NexaStep() { }

    /// <summary>Initializes a new instance with key, title, and description.</summary>
    public NexaStep(string key, string title, string description = "")
    {
        _key = key ?? string.Empty;
        _title = title ?? string.Empty;
        _description = description ?? string.Empty;
    }

    /// <summary>Unique key identifying the step.</summary>
    [Category("NexaUI")]
    [DefaultValue("")]
    [Description("Unique key identifying the step.")]
    public string Key
    {
        get => _key;
        set { _key = value ?? string.Empty; }
    }

    /// <summary>Title of the step.</summary>
    [Category("NexaUI")]
    [DefaultValue("")]
    [Description("Title of the step.")]
    public string Title
    {
        get => _title;
        set { _title = value ?? string.Empty; }
    }

    /// <summary>Optional description for the step.</summary>
    [Category("NexaUI")]
    [DefaultValue("")]
    [Description("Optional description for the step.")]
    public string Description
    {
        get => _description;
        set { _description = value ?? string.Empty; }
    }

    /// <summary>Current state of the step.</summary>
    [Category("NexaUI")]
    [DefaultValue(NexaStepState.Pending)]
    [Description("Current state of the step.")]
    public NexaStepState State
    {
        get => _state;
        set { _state = value; }
    }

    /// <summary>Whether the step is enabled (interactable).</summary>
    [Category("NexaUI")]
    [DefaultValue(true)]
    [Description("Whether the step is enabled (interactable).")]
    public bool Enabled
    {
        get => _enabled;
        set { _enabled = value; }
    }

    /// <summary>Whether the step is visible.</summary>
    [Category("NexaUI")]
    [DefaultValue(true)]
    [Description("Whether the step is visible.")]
    public bool Visible
    {
        get => _visible;
        set { _visible = value; }
    }

    /// <summary>User-defined data associated with the step.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public object? Tag
    {
        get => _tag;
        set { _tag = value; }
    }
}