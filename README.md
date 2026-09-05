# NexaUI

NexaUI is a modern Windows Forms UI component library for .NET 10.

It is being developed as an original framework with its own architecture,
API design, visual identity, and theme system. The goal is to provide a
productive, professional set of WinForms components that developers can
consume from both **C#** and **VB.NET**.

## Target Framework

- `net10.0-windows`
- Windows Forms only
- No WPF, no MAUI, no XAML
- No third-party UI dependencies

## Supported Languages

The NexaUI library itself is implemented in **C#**. Consumers may use
either **C#** or **VB.NET**.

## Architecture

The solution is split into focused projects:

| Project              | Type                  | Responsibility                                      |
|----------------------|-----------------------|-----------------------------------------------------|
| `NexaUI.Core`        | Class Library         | Shared abstractions, enums, interfaces, utilities  |
| `NexaUI.Themes`      | Class Library         | Theme system, colors, typography, spacing, radii    |
| `NexaUI.Icons`       | Class Library         | Icon abstractions and icon infrastructure           |
| `NexaUI.Controls`    | Class Library         | Windows Forms UI controls                           |
| `NexaUI.Demo.CSharp` | WinForms App (C#)     | C# demonstration application                        |
| `NexaUI.Demo.VB`     | WinForms App (VB.NET) | VB.NET demonstration application                    |
| `NexaUI.Tests`       | Test Project (MSTest) | Automated tests for non-visual functionality        |

### Project References

```
NexaUI.Themes   -> NexaUI.Core
NexaUI.Icons    -> NexaUI.Core
NexaUI.Controls -> NexaUI.Core, NexaUI.Themes, NexaUI.Icons
NexaUI.Demo.*   -> NexaUI.Core, NexaUI.Themes, NexaUI.Controls, NexaUI.Icons
NexaUI.Tests    -> NexaUI.Core, NexaUI.Themes, NexaUI.Icons, NexaUI.Controls
```

## Design Principles

- Prefer native WinForms controls over custom rendering.
- Preserve native WinForms behavior and Visual Studio Designer compatibility.
- Provide a consistent visual identity through the theme system.
- Keep public APIs simple and friendly to both C# and VB.NET.
- No copying of proprietary implementations from any third-party library.

## Current Status

**Phase 07 - Input Controls.**

Implemented controls:

- `NexaButton` — themed button with styles, sizes, icons, loading
- `NexaLabel` — themed text label with `NexaLabelStyle`
- `NexaLinkLabel` — themed hyperlink derived from native `LinkLabel`
- `NexaSeparator` — themed horizontal/vertical divider
- `NexaTextBox` — themed single/multi-line text input
- `NexaMaskedTextBox` — themed masked text input
- `NexaSearchBox` — themed search field with debounce + clear

Both demo applications (`NexaUI.Demo.CSharp`, `NexaUI.Demo.VB`) include a
gallery shell with Getting Started, Themes, Basic Controls, and Input
Controls pages.

See `NEXAUI_RULES.md` for the permanent development rules that govern
this project.

## Controls

### NexaButton
A themed button that inherits from `System.Windows.Forms.Button`.

```csharp
var save = new NexaButton
{
    Text = "Save",
    Style = NexaButtonStyle.Primary,
    SizeMode = NexaButtonSize.Medium
};
```

```vb
Dim save As New NexaButton With {
    .Text = "Save",
    .Style = NexaButtonStyle.Primary,
    .SizeMode = NexaButtonSize.Medium
}
```

### NexaLabel
A themed text label that inherits from `System.Windows.Forms.Label`.

```csharp
var name = new NexaLabel
{
    Text = "Student Name",
    LabelStyle = NexaLabelStyle.Default
};
```

```vb
Dim name As New NexaLabel With {
    .Text = "Student Name",
    .LabelStyle = NexaLabelStyle.Default
}
```

`NexaLabelStyle`: `Default, Heading, Subheading, Caption, Muted, Success, Warning, Danger, Info`.

### NexaLinkLabel
A themed hyperlink that inherits from `System.Windows.Forms.LinkLabel`. Navigation is the consuming application's responsibility.

```csharp
var link = new NexaLinkLabel { Text = "Open Documentation" };
link.LinkClicked += (s, e) => MessageBox.Show("Open docs");
```

```vb
Dim link As New NexaLinkLabel With {.Text = "Open Documentation"}
AddHandler link.LinkClicked, Sub(s, e)
                                  MessageBox.Show("Open docs")
                              End Sub
```

### NexaSeparator
A themed divider. WinForms has no native modern separator so NexaSeparator is a lightweight custom control.

```csharp
var separator = new NexaSeparator
{
    Orientation = NexaSeparatorOrientation.Horizontal
};
```

```vb
Dim separator As New NexaSeparator With {
    .Orientation = NexaSeparatorOrientation.Horizontal
}
```

`NexaSeparatorOrientation`: `Horizontal, Vertical`.
`NexaSeparatorStyle`: `Solid, Dashed`.

### NexaTextBox
A themed wrapper around `System.Windows.Forms.TextBox` that paints a
rounded themed border, exposes a placeholder cue, and supports four
visual styles. Forwarded to the native editor: `MaxLength`, `Multiline`,
`ReadOnly`, `UseSystemPasswordChar`, `PasswordChar`, `CharacterCasing`,
`ScrollBars`, `WordWrap`, `AcceptsReturn`, `AcceptsTab`, selection,
`HideSelection`, and clipboard (`Copy`/`Cut`/`Paste`/`Undo`/`SelectAll`).

```csharp
var name = new NexaTextBox
{
    Style = NexaTextBoxStyle.Filled,
    PlaceholderText = "Enter student name",
    ShowClearButton = true,
    HelperText = "First name and last name.",
    ValidationState = NexaTextValidationState.None,
    Width = 320
};
name.TextChanged += (s, e) => Console.WriteLine(name.Text);
```

```vb
Dim name As New NexaTextBox With {
    .Style = NexaTextBoxStyle.Filled,
    .PlaceholderText = "Enter student name",
    .ShowClearButton = True,
    .HelperText = "First name and last name."
}
AddHandler name.TextChanged, Sub(s, e) Console.WriteLine(name.Text)
```

`NexaTextBoxStyle`: `Default, Filled, Outlined, Flat`.
`NexaTextValidationState`: `None, Error, Success`.

### NexaMaskedTextBox
A themed wrapper around `System.Windows.Forms.MaskedTextBox`. All
mask-format properties (`Mask`, `PromptChar`, `AsciiOnly`, `BeepOnError`,
`CutCopyMaskFormat`, `HidePromptOnLeave`, `ValidatingType`) are forwarded
to the native editor, and helper / error text and validation colors
follow the active theme.

```csharp
var phone = new NexaMaskedTextBox { Mask = "(000) 000-0000", Width = 220 };
var postal = new NexaMaskedTextBox { Mask = "00000", Width = 120 };
phone.MaskInputRejected += (s, e) => { /* handle rejection */ };
```

```vb
Dim phone As New NexaMaskedTextBox With {.Mask = "(000) 000-0000", .Width = 220}
```

### NexaSearchBox
A themed search field built on top of `NexaTextBox` with a search glyph,
a clear button, and a debounced `SearchChanged` event. The `Cleared`
event also fires when the user presses `Escape`.

```csharp
var search = new NexaSearchBox
{
    PlaceholderText = "Search students...",
    SearchDelayMs = 250,
    Width = 360
};
search.SearchChanged += (s, e) => RunSearch(search.SearchText);
search.Cleared += (s, e) => ClearResults();
```

```vb
Dim search As New NexaSearchBox With {
    .PlaceholderText = "Search students...",
    .SearchDelayMs = 250
}
AddHandler search.SearchChanged, Sub(s, e) RunSearch(search.SearchText)
```

### Theme Switching

All NexaUI controls update automatically when the active theme changes:

```csharp
ThemeManager.SetTheme(new DarkTheme());
```