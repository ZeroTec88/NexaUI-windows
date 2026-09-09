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

**Phase 13 - Data Presentation Controls.**

Implemented controls:

- `NexaButton` — themed button with styles, sizes, icons, loading
- `NexaLabel` — themed text label with `NexaLabelStyle`
- `NexaLinkLabel` — themed hyperlink derived from native `LinkLabel`
- `NexaSeparator` — themed horizontal/vertical divider
- `NexaTextBox` — themed single/multi-line text input
- `NexaMaskedTextBox` — themed masked text input
- `NexaSearchBox` — themed search field with debounce + clear
- `NexaCheckBox` — themed check box (native base) with two-state and tri-state
- `NexaRadioButton` — themed radio button (native base) with parent-scoped grouping
- `NexaComboBox` — themed drop-down list (native base) with styles, data binding, and placeholder
- `NexaToggleSwitch` — animated iOS-style on/off switch with sizes and optional labels
- `NexaPanel` — themed panel with surface styles, rounded corners, and optional shadow
- `NexaCard` — modern card container with header, content, footer, title, and subtitle
- `NexaGroupBox` — themed group box (native base) with modern border and rounded corners
- `NexaFlowPanel` — themed flow layout panel (native base) with optional border
- `NexaTablePanel` — themed table layout panel (native base) with optional border
- `NexaProgressBar` — themed horizontal progress bar with styles, percentage label, and indeterminate mode
- `NexaCircularProgress` — themed circular progress indicator with center text and indeterminate mode
- `NexaSpinner` — animated loading spinner (Ring and Dots styles) with configurable speed
- `NexaBadge` — compact themed badge with styles, sizes, and 99+ truncation
- `NexaAlert` — inline notification panel with title, message, icon, and close button
- `NexaStatusIndicator` — compact status dot with text and semantic theme colors
- `NexaTabControl` — themed tab control with styles (Default, Underline, Pill), native base
- `NexaTabPage` — enhanced tab page with optional icon and badge support
- `NexaNavigationBar` — vertical application navigation bar with Expanded/Compact modes
- `NexaNavigationItem` — navigation item with icon, badge, and selection state
- `NexaBreadcrumb` — hierarchical path navigation with customizable separators
- `NexaBreadcrumbItem` — breadcrumb item with text, key, and enabled state
- `NexaStepper` — multi-step workflow control with horizontal/vertical orientations
- `NexaStep` — step item with title, description, state (Pending/Current/Completed/Error/Disabled)
- `NexaDataGridView` — theme-aware DataGridView with row numbers, grid styles, header styles, and alternate rows
- `NexaListView` — theme-aware ListView with Details view styling, alternating rows, and custom headers
- `NexaPropertyGrid` — theme-aware PropertyGrid with styled categories, help, and command areas
- `NexaTreeView` — theme-aware TreeView with custom node drawing, lines, checkboxes, and expand/collapse indicators

Both demo applications (`NexaUI.Demo.CSharp`, `NexaUI.Demo.VB`) include a
gallery shell with Getting Started, Themes, Basic Controls, Input Controls,
Selection Controls, Layout Controls, Feedback Controls, Navigation Controls,
and Data Presentation pages. The Navigation Controls page includes a complete
Application Shell example combining NavigationBar, Breadcrumb, TabControl, and
content area.

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
A Bootstrap-style themed text input. Hosts a native `System.Windows.Forms.TextBox`
so all native behavior (typing, selection, copy/paste, undo/redo, IME, accessibility,
keyboard navigation) is preserved. Adds themed borders, focus glow, validation icons,
a built-in `Label`, helper / error text, an optional left `Icon`, an optional
`ShowClearButton`, and a live `ShowCounter`. Forwarded to the native editor: `MaxLength`,
`Multiline`, `ReadOnly`, `UseSystemPasswordChar`, `PasswordChar`, `CharacterCasing`,
`ScrollBars`, `WordWrap`, `AcceptsReturn`, `AcceptsTab`, selection, `HideSelection`,
and clipboard (`Copy`/`Cut`/`Paste`/`Undo`/`SelectAll`).

```csharp
var name = new NexaTextBox
{
    Label = "Full name",
    PlaceholderText = "Your full name",
    HelperText = "First and last name as it appears on your ID.",
    Style = NexaInputStyle.Outline,
    InputSize = NexaInputSize.Medium,
    Icon = NexaIconKind.User,
    ShowClearButton = true,
    Width = 320
};
name.TextChanged += (s, e) => Console.WriteLine(name.Text);
```

```vb
Dim name As New NexaTextBox With {
    .Label = "Full name",
    .PlaceholderText = "Your full name",
    .HelperText = "First and last name as it appears on your ID.",
    .Style = NexaInputStyle.Outline,
    .InputSize = NexaInputSize.Medium,
    .Icon = NexaIconKind.User,
    .ShowClearButton = True
}
AddHandler name.TextChanged, Sub(s, e) Console.WriteLine(name.Text)
```

`NexaInputStyle`: `Outline` (default), `Filled`, `Underline`.
`NexaInputSize`: `Small` (32 px), `Medium` (40 px, default), `Large` (48 px).
`NexaTextValidationState`: `None`, `Error`, `Success`. Validation state colors the
border and shows a ✓ / ✗ icon inside the input. When `ErrorText` is set, the helper
text turns red and shows the error message.

### NexaMaskedTextBox
A Bootstrap-style themed masked text input. Hosts a native `MaskedTextBox` so all
native masking behavior, validation, and accessibility remain intact. Adds themed
borders, focus glow, validation icons, a built-in `Label`, and helper / error text.

```csharp
var phone = new NexaMaskedTextBox
{
    Label = "Phone number",
    Mask = "(000) 000-0000",
    Width = 240
};
phone.MaskInputRejected += (s, e) => { /* handle rejection */ };
```

```vb
Dim phone As New NexaMaskedTextBox With {
    .Label = "Phone number",
    .Mask = "(000) 000-0000"
}
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

### NexaCheckBox
A themed check box that **inherits from `System.Windows.Forms.CheckBox`** so all
native behavior (click, focus, keyboard, accessibility, data binding, `Checked`,
`CheckState`, `ThreeState`, `AutoCheck`, `TextAlign`, `RightToLeft`, `TabStop`,
designer support) is preserved. The native control is owner-painted with a
sharp 2 px themed glyph, optional indeterminate dash, and focus ring. Adds
`Style`, `ValidationState`, `ErrorText`, `HelperText`, `ShowHelperText`, and a
read/write `IsIndeterminate` shortcut.

```csharp
var agree = new NexaCheckBox { Text = "I agree to the terms", Checked = true };
agree.CheckedChanged += (s, e) => Console.WriteLine(agree.Checked);

var permissions = new NexaCheckBox
{
    Text = "Select all permissions",
    ThreeState = true,
    IsIndeterminate = true
};
```

```vb
Dim agree As New NexaCheckBox With {.Text = "I agree to the terms", .Checked = True}
AddHandler agree.CheckedChanged, Sub(s, e) Console.WriteLine(agree.Checked)

Dim permissions As New NexaCheckBox With {
    .Text = "Select all permissions",
    .ThreeState = True,
    .IsIndeterminate = True
}
```

`NexaCheckBoxStyle`: `Default`, `Filled`, `Minimal`.
`NexaTextValidationState`: `None`, `Error`, `Success`.

### NexaRadioButton
A themed radio button that **inherits from `System.Windows.Forms.RadioButton`**
so all native behavior (click, focus, keyboard, accessibility, data binding,
`Checked`, `AutoCheck`, `TextAlign`, `RightToLeft`, `TabStop`, designer support)
is preserved. Native parent-scoped grouping remains in effect — radio buttons
placed in the same parent are mutually exclusive automatically. The native
control is owner-painted with a circular themed indicator and focus ring. Adds
`Style`, `ValidationState`, `ErrorText`, `HelperText`, and `ShowHelperText`.

```csharp
var panel = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 40 };
var gradeA = new NexaRadioButton { Text = "Grade A", Parent = panel, Checked = true };
var gradeB = new NexaRadioButton { Text = "Grade B", Parent = panel };
var gradeC = new NexaRadioButton { Text = "Grade C", Parent = panel };
```

```vb
Dim panel As New FlowLayoutPanel With {.Dock = DockStyle.Top, .Height = 40}
Dim gradeA As New NexaRadioButton With {.Text = "Grade A", .Parent = panel, .Checked = True}
Dim gradeB As New NexaRadioButton With {.Text = "Grade B", .Parent = panel}
Dim gradeC As New NexaRadioButton With {.Text = "Grade C", .Parent = panel}
```

### NexaComboBox
A themed drop-down list that **inherits from `System.Windows.Forms.ComboBox`**
so all native behavior (drop-down, keyboard navigation, `SelectedIndex`,
`SelectedItem`, `SelectedValue`, `DataSource`, `DisplayMember`, `ValueMember`,
`DropDownStyle`, `AutoCompleteMode`, `AutoCompleteSource`, `MaxDropDownItems`,
`Sorted`, `Items` collection, designer support) is preserved. The native
control is owner-drawn with a themed border/background (Outlined/Filled/Flat),
focus ring, chevron-down glyph, and themed drop-down rows. Adds
`Style`, `ValidationState`, `ErrorText`, `HelperText`, `ShowHelperText`,
and a `PlaceholderText` shown when no item is selected.

```csharp
var course = new NexaComboBox
{
    Style = NexaComboBoxStyle.Outlined,
    PlaceholderText = "Select a course...",
    Width = 280
};
course.Items.AddRange(new object[] { "Mathematics", "Physics", "Chemistry", "Biology" });
course.SelectedIndexChanged += (s, e) => Console.WriteLine(course.SelectedItem);
```

```vb
Dim course As New NexaComboBox With {
    .Style = NexaComboBoxStyle.Outlined,
    .PlaceholderText = "Select a course...",
    .Width = 280
}
course.Items.AddRange(New Object() {"Mathematics", "Physics", "Chemistry", "Biology"})
AddHandler course.SelectedIndexChanged, Sub(s, e) Console.WriteLine(course.SelectedItem)
```

`NexaComboBoxStyle`: `Outlined` (default), `Filled`, `Flat`.

### NexaToggleSwitch
An animated iOS-style on/off switch. No native WinForms equivalent exists, so
`NexaToggleSwitch` is a lightweight custom control with owner-painted track
and thumb. Supports a smooth animated transition between states, optional
`OnText`/`OffText` labels, three sizes, and a safe reusable `System.Windows.Forms.Timer`
(started only during animation, stopped on completion and on `Disposed`).
Keyboard accessible (Space/Enter to toggle, Tab to move focus).

```csharp
var notifications = new NexaToggleSwitch
{
    OnText = "ON",
    OffText = "OFF",
    ShowText = true,
    ToggleSize = NexaToggleSize.Medium,
    AnimationEnabled = true,
    AnimationDurationMs = 140
};
notifications.CheckedChanged += (s, e) => Console.WriteLine(notifications.Checked);
```

```vb
Dim notifications As New NexaToggleSwitch With {
    .OnText = "ON",
    .OffText = "OFF",
    .ShowText = True,
    .ToggleSize = NexaToggleSize.Medium,
    .AnimationEnabled = True,
    .AnimationDurationMs = 140
}
AddHandler notifications.CheckedChanged, Sub(s, e) Console.WriteLine(notifications.Checked)
```

`NexaToggleSize`: `Small` (32×16), `Medium` (44×22, default), `Large` (56×28).
Note: the property is named `ToggleSize` (not `Size`) to avoid hiding
`Control.Size` from the base class.

### NexaPanel
A themed WinForms panel that **inherits from `System.Windows.Forms.Panel`**
so all native layout, docking, scrolling, anchoring, tab order, and designer
behavior is preserved. Adds a themed background, an optional border with
rounded corners, and an optional subtle drop shadow. All dimensions are stored
in device-independent pixels (DIPs) and DPI-scaled at paint time.

```csharp
var panel = new NexaPanel
{
    SurfaceStyle = NexaPanelSurfaceStyle.Elevated,
    BorderStyleEx = NexaBorderStyleEx.Solid,
    CornerRadius = 8,
    ShadowEnabled = true,
    ShadowDepth = 4,
    Dock = DockStyle.Fill,
    Padding = new Padding(16)
};
panel.Controls.Add(childControl);
```

```vb
Dim panel As New NexaPanel With {
    .SurfaceStyle = NexaPanelSurfaceStyle.Elevated,
    .BorderStyleEx = NexaBorderStyleEx.Solid,
    .CornerRadius = 8,
    .ShadowEnabled = True,
    .ShadowDepth = 4,
    .Dock = DockStyle.Fill,
    .Padding = New Padding(16)
}
panel.Controls.Add(childControl)
```

`NexaPanelSurfaceStyle`: `Default`, `Surface`, `Elevated`, `Transparent`.
`NexaBorderStyleEx`: `None`, `Solid` (named `BorderStyleEx` to avoid clashing
with the native `Panel.BorderStyle`).

### NexaCard
A modern card container. Composes a themed background with an optional
header (title + subtitle), a content area, and an optional footer. Add child
controls to `card.ContentPanel` (or to `card.Controls`, which routes into the
content area). Inherits from `UserControl` so docking, anchoring, tab order,
and Visual Studio designer behavior are all preserved.

```csharp
var card = new NexaCard
{
    Title = "System Information",
    Subtitle = "Current system status",
    Dock = DockStyle.Fill,
    CornerRadius = 4,
    ShadowEnabled = true,
    ShadowDepth = 4
};
card.ContentPanel.Controls.Add(new NexaLabel { Text = "OS: Windows 11" });
card.FooterPanel.Controls.Add(new NexaButton { Text = "Refresh" });
```

```vb
Dim card As New NexaCard With {
    .Title = "System Information",
    .Subtitle = "Current system status",
    .Dock = DockStyle.Fill,
    .CornerRadius = 4,
    .ShadowEnabled = True,
    .ShadowDepth = 4
}
card.ContentPanel.Controls.Add(New NexaLabel With {.Text = "OS: Windows 11"})
card.FooterPanel.Controls.Add(New NexaButton With {.Text = "Refresh"})
```

`ContentPanel` and `FooterPanel` are `[Browsable(false)]` implementation
details — the primary developer API is `card.Controls.Add(...)` (which adds
to `ContentPanel` via the standard `UserControl.Controls` collection) plus
`Title`, `Subtitle`, `HeaderVisible`, `FooterVisible`, `CornerRadius`,
`BorderThickness`, `ShadowEnabled`, and `ShadowDepth`.

### NexaGroupBox
A themed WinForms group box that **inherits from `System.Windows.Forms.GroupBox`**
so all native layout, docking, child-controls collection, `AutoSize`, and
designer behavior is preserved. Adds a themed title color, a modern border,
and rounded corners. The native `Text` property remains the primary title.

```csharp
var gb = new NexaGroupBox { Text = "Account Settings", Width = 300, Height = 200 };
gb.Controls.Add(new NexaTextBox { Dock = DockStyle.Top, Text = "username" });
gb.Controls.Add(new NexaTextBox { Dock = DockStyle.Top, Text = "••••••" });
```

```vb
Dim gb As New NexaGroupBox With {.Text = "Account Settings", .Width = 300, .Height = 200}
gb.Controls.Add(New NexaTextBox With {.Dock = DockStyle.Top, .Text = "username"})
gb.Controls.Add(New NexaTextBox With {.Dock = DockStyle.Top, .Text = "••••••"})
```

### NexaFlowPanel
A themed WinForms flow layout panel that **inherits from
`System.Windows.Forms.FlowLayoutPanel`** so all native flow layout behavior
(`FlowDirection`, `WrapContents`, `AutoScroll`, `AutoSize`, docking,
anchoring, designer support) is preserved. Adds a themed background, an
optional border, rounded corners, and full theme integration.

```csharp
var flow = new NexaFlowPanel
{
    FlowDirection = FlowDirection.LeftToRight,
    WrapContents = true,
    BorderEnabled = true,
    CornerRadius = 4,
    AutoSize = true,
    AutoSizeMode = AutoSizeMode.GrowAndShrink
};
flow.Controls.Add(new NexaButton { Text = "One" });
flow.Controls.Add(new NexaButton { Text = "Two" });
```

```vb
Dim flow As New NexaFlowPanel With {
    .FlowDirection = FlowDirection.LeftToRight,
    .WrapContents = True,
    .BorderEnabled = True,
    .CornerRadius = 4,
    .AutoSize = True,
    .AutoSizeMode = AutoSizeMode.GrowAndShrink
}
flow.Controls.Add(New NexaButton With {.Text = "One"})
flow.Controls.Add(New NexaButton With {.Text = "Two"})
```

### NexaTablePanel
A themed WinForms table layout panel that **inherits from
`System.Windows.Forms.TableLayoutPanel`** so all native table layout behavior
(`RowCount`, `ColumnCount`, `RowStyles`, `ColumnStyles`, `GrowStyle`,
`CellBorderStyle`, docking, anchoring, designer support) is preserved. Adds a
themed background, an optional border, rounded corners, and full theme
integration. The native TableLayoutPanel layout engine is not replaced.

```csharp
var table = new NexaTablePanel { ColumnCount = 2, AutoSize = true };
table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
table.Controls.Add(new NexaLabel { Text = "Name" }, 0, 0);
table.Controls.Add(new NexaTextBox(), 1, 0);
table.Controls.Add(new NexaLabel { Text = "Email" }, 0, 1);
table.Controls.Add(new NexaTextBox(), 1, 1);
```

```vb
Dim table As New NexaTablePanel With {.ColumnCount = 2, .AutoSize = True}
table.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 100.0F))
table.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
table.Controls.Add(New NexaLabel With {.Text = "Name"}, 0, 0)
table.Controls.Add(New NexaTextBox(), 1, 0)
table.Controls.Add(New NexaLabel With {.Text = "Email"}, 0, 1)
table.Controls.Add(New NexaTextBox(), 1, 1)
```

### Theme Switching

All NexaUI controls update automatically when the active theme changes:

```csharp
ThemeManager.SetTheme(new DarkTheme());
```

### NexaProgressBar
A themed horizontal progress bar with rounded corners, theme-aware track and
fill, an optional centered percentage label, and an animated indeterminate
mode. All dimensions are stored in device-independent pixels (DIPs) and
DPI-scaled at paint time. Accessible via `AccessibleRole.ProgressBar`.

```csharp
var bar = new NexaProgressBar
{
    Minimum = 0,
    Maximum = 100,
    Value = 35,
    ProgressStyle = NexaProgressStyle.Success,
    ShowPercentage = true,
    BarHeight = 12,
    Width = 320
};
bar.ValueChanged += (s, e) => Console.WriteLine(bar.Value);
```

```vb
Dim bar As New NexaProgressBar With {
    .Minimum = 0,
    .Maximum = 100,
    .Value = 35,
    .ProgressStyle = NexaProgressStyle.Success,
    .ShowPercentage = True,
    .BarHeight = 12,
    .Width = 320
}
AddHandler bar.ValueChanged, Sub(s, e) Console.WriteLine(bar.Value)
```

`NexaProgressStyle`: `Default`, `Success`, `Warning`, `Danger`, `Info`.

### NexaCircularProgress
A themed circular progress indicator drawn with anti-aliased GDI+. Supports
configurable line thickness, an optional centered label (percentage or custom
text), and an animated indeterminate mode. All dimensions are DPI-scaled.

```csharp
var c = new NexaCircularProgress
{
    Value = 72,
    LineThickness = 6,
    ProgressStyle = NexaProgressStyle.Default,
    ShowPercentage = true,
    CenterText = string.Empty, // set to override the percentage
    Size = new Size(96, 96)
};
```

```vb
Dim c As New NexaCircularProgress With {
    .Value = 72,
    .LineThickness = 6,
    .ProgressStyle = NexaProgressStyle.Default,
    .ShowPercentage = True,
    .CenterText = "",
    .Size = New Size(96, 96)
}
```

### NexaSpinner
An animated loading spinner with a single control-owned `System.Windows.Forms.Timer`
that is stopped when the control is hidden, disabled, or disposed. Supports
`Ring` (rotating arc) and `Dots` (pulsing dot sequence) styles with a
configurable `AnimationSpeed` (interval in ms).

```csharp
var spinner = new NexaSpinner
{
    SpinnerStyle = NexaSpinnerStyle.Ring,
    SpinnerSize = 32,
    AnimationSpeed = 80,
    AnimationEnabled = true
};
```

```vb
Dim spinner As New NexaSpinner With {
    .SpinnerStyle = NexaSpinnerStyle.Ring,
    .SpinnerSize = 32,
    .AnimationSpeed = 80,
    .AnimationEnabled = True
}
```

### NexaBadge
A compact themed badge with pill-shaped rounded corners, theme-aware colors,
three size variants, and optional 99+ truncation for numeric content. When
`AutoSize` is true (default) the badge sizes itself to fit the text.

```csharp
var badge = new NexaBadge
{
    Text = "1234",
    BadgeStyle = NexaBadgeStyle.Danger,
    BadgeSize = NexaBadgeSize.Medium,
    MaximumCharacters = 2 // "1234" becomes "99+"
};
```

```vb
Dim badge As New NexaBadge With {
    .Text = "1234",
    .BadgeStyle = NexaBadgeStyle.Danger,
    .BadgeSize = NexaBadgeSize.Medium,
    .MaximumCharacters = 2
}
```

`NexaBadgeStyle`: `Default`, `Primary`, `Success`, `Warning`, `Danger`, `Info`, `Muted`.
`NexaBadgeSize`: `Small`, `Medium`, `Large`.

### NexaAlert
A themed inline notification panel with an optional icon, title, message,
and close button. Supports optional auto-close after a configurable delay
using a control-owned timer. Raises `Closed` when dismissed by the user or
auto-closed.

```csharp
var alert = new NexaAlert
{
    AlertStyle = NexaAlertStyle.Success,
    Title = "Operation complete",
    Message = "Your files were saved successfully.",
    ShowIcon = true,
    Closable = true,
    AutoClose = false,
    AutoCloseDelayMs = 3000,
    Dock = DockStyle.Top
};
alert.Closed += (s, e) => Console.WriteLine("Alert closed");
```

```vb
Dim alert As New NexaAlert With {
    .AlertStyle = NexaAlertStyle.Success,
    .Title = "Operation complete",
    .Message = "Your files were saved successfully.",
    .ShowIcon = True,
    .Closable = True,
    .AutoClose = False,
    .AutoCloseDelayMs = 3000,
    .Dock = DockStyle.Top
}
AddHandler alert.Closed, Sub(s, e) Console.WriteLine("Alert closed")
```

`NexaAlertStyle`: `Information`, `Success`, `Warning`, `Error`.

### NexaStatusIndicator
A compact status dot with an optional text label. Uses semantic theme colors
for each `NexaStatus` value. The text is exposed via `AccessibleName` so
screen readers convey the status meaningfully.

```csharp
var status = new NexaStatusIndicator
{
    Status = NexaStatus.Online,
    Text = "System online",
    ShowText = true,
    IndicatorSize = 10
};
status.StatusChanged += (s, e) => Console.WriteLine(status.Status);
```

```vb
Dim status As New NexaStatusIndicator With {
    .Status = NexaStatus.Online,
    .Text = "System online",
    .ShowText = True,
    .IndicatorSize = 10
}
AddHandler status.StatusChanged, Sub(s, e) Console.WriteLine(status.Status)
```

`NexaStatus`: `None`, `Online`, `Offline`, `Busy`, `Warning`, `Success`, `Error`.

### NexaTabControl
A themed tab control that inherits from `System.Windows.Forms.TabControl` so all
native behavior (tab pages, selection, keyboard/mouse navigation, ImageList,
ItemSize, Padding, alignment, appearance, HotTrack, events) is preserved.
Adds NexaUI theming with three visual styles, configurable header height,
active indicator, and optional close button support.

```csharp
var tabs = new NexaTabControl
{
    TabStyle = NexaTabStyle.Underline,
    TabHeaderHeight = 40,
    ActiveIndicatorThickness = 3,
    Dock = DockStyle.Fill
};
tabs.TabPages.Add(new TabPage { Text = "Overview" });
tabs.TabPages.Add(new TabPage { Text = "Details" });
tabs.TabPages.Add(new TabPage { Text = "Settings" });
```

```vb
Dim tabs As New NexaTabControl With {
    .TabStyle = NexaTabStyle.Underline,
    .TabHeaderHeight = 40,
    .ActiveIndicatorThickness = 3,
    .Dock = DockStyle.Fill
}
tabs.TabPages.Add(New TabPage With {.Text = "Overview"})
tabs.TabPages.Add(New TabPage With {.Text = "Details"})
tabs.TabPages.Add(New TabPage With {.Text = "Settings"})
```

`NexaTabStyle`: `Default`, `Underline`, `Pill`.

### NexaTabPage
An enhanced tab page that inherits from `System.Windows.Forms.TabPage` and adds
optional icon and badge support for richer tab headers.

```csharp
var page = new NexaTabPage
{
    Text = "Inbox",
    IconKind = NexaIconKind.Check,
    BadgeText = "5",
    BadgeVisible = true
};
page.Controls.Add(new NexaLabel { Text = "Inbox content" });
```

```vb
Dim page As New NexaTabPage With {
    .Text = "Inbox",
    .IconKind = NexaIconKind.Check,
    .BadgeText = "5",
    .BadgeVisible = True
}
page.Controls.Add(New NexaLabel With {.Text = "Inbox content"})
```

### NexaNavigationBar
A vertical application navigation bar supporting Expanded (icon + text) and
Compact (icon only) modes. Items support icons, badges, selection state,
enabled/disabled, and click/selection events.

```csharp
var nav = new NexaNavigationBar
{
    Mode = NexaNavigationMode.Expanded,
    Width = 260,
    ItemHeight = 40,
    Dock = DockStyle.Fill
};
nav.Items.Add(new NexaNavigationItem { Key = "dashboard", Text = "Dashboard", IconKind = NexaIconKind.User });
nav.Items.Add(new NexaNavigationItem { Key = "reports", Text = "Reports", IconKind = NexaIconKind.Info, BadgeText = "3", BadgeVisible = true });
nav.SelectedIndex = 0;
nav.SelectedItemChanged += (s, e) => Console.WriteLine($"Selected: {e.Item.Text}");
```

```vb
Dim nav As New NexaNavigationBar With {
    .Mode = NexaNavigationMode.Expanded,
    .Width = 260,
    .ItemHeight = 40,
    .Dock = DockStyle.Fill
}
nav.Items.Add(New NexaNavigationItem With {.Key = "dashboard", .Text = "Dashboard", .IconKind = NexaIconKind.User})
nav.Items.Add(New NexaNavigationItem With {.Key = "reports", .Text = "Reports", .IconKind = NexaIconKind.Info, .BadgeText = "3", .BadgeVisible = True})
nav.SelectedIndex = 0
AddHandler nav.SelectedItemChanged, Sub(s, e) Console.WriteLine($"Selected: {e.Item.Text}")
```

`NexaNavigationMode`: `Expanded`, `Compact`.

### NexaNavigationItem
A navigation item for use with `NexaNavigationBar`. Supports key, text, icon,
enabled/visible state, badge, and user tag.

```csharp
var item = new NexaNavigationItem("settings", "Settings")
{
    IconKind = NexaIconKind.Settings,
    BadgeText = "3",
    BadgeVisible = true
};
```

```vb
Dim item As New NexaNavigationItem("settings", "Settings") With {
    .IconKind = NexaIconKind.Settings,
    .BadgeText = "3",
    .BadgeVisible = True
}
```

### NexaBreadcrumb
A horizontal breadcrumb navigation showing hierarchical path. Supports
customizable separators, clickable items, and current/active item styling.

```csharp
var bc = new NexaBreadcrumb
{
    Separator = NexaBreadcrumbSeparator.Chevron,
    ItemSpacing = 8,
    PaddingDips = 12,
    Dock = DockStyle.Top
};
bc.Items.Add(new NexaBreadcrumbItem { Key = "home", Text = "Home" });
bc.Items.Add(new NexaBreadcrumbItem { Key = "students", Text = "Students" });
bc.Items.Add(new NexaBreadcrumbItem { Key = "batch2026", Text = "Batch 2026" });
bc.ItemClick += (s, e) => Console.WriteLine($"Clicked: {e.Item.Text}");
```

```vb
Dim bc As New NexaBreadcrumb With {
    .Separator = NexaBreadcrumbSeparator.Chevron,
    .ItemSpacing = 8,
    .PaddingDips = 12,
    .Dock = DockStyle.Top
}
bc.Items.Add(New NexaBreadcrumbItem With {.Key = "home", .Text = "Home"})
bc.Items.Add(New NexaBreadcrumbItem With {.Key = "students", .Text = "Students"})
bc.Items.Add(New NexaBreadcrumbItem With {.Key = "batch2026", .Text = "Batch 2026"})
AddHandler bc.ItemClick, Sub(s, e) Console.WriteLine($"Clicked: {e.Item.Text}")
```

`NexaBreadcrumbSeparator`: `Chevron`, `Slash`, `GreaterThan`, `Custom`.

### NexaBreadcrumbItem
A breadcrumb item with text, key, enabled/visible state, and user tag.

```csharp
var item = new NexaBreadcrumbItem("home", "Home") { Enabled = true };
```

```vb
Dim item As New NexaBreadcrumbItem("home", "Home") With {.Enabled = True}
```

### NexaStepper
A multi-step workflow control supporting horizontal and vertical orientations,
clickable steps, and step states (Pending, Current, Completed, Error, Disabled).

```csharp
var stepper = new NexaStepper
{
    Orientation = NexaOrientation.Horizontal,
    Dock = DockStyle.Top,
    Height = 120,
    ShowDescriptions = true,
    ShowStepNumbers = true,
    AllowNavigation = true
};
stepper.Steps.Add(new NexaStep { Key = "account", Title = "Account", Description = "Create account", State = NexaStepState.Completed });
stepper.Steps.Add(new NexaStep { Key = "profile", Title = "Profile", Description = "Set up profile", State = NexaStepState.Current });
stepper.Steps.Add(new NexaStep { Key = "confirm", Title = "Confirm", Description = "Verify details", State = NexaStepState.Pending });
stepper.CurrentStepChanged += (s, e) => Console.WriteLine($"Step: {e.Step.Title}");
```

```vb
Dim stepper As New NexaStepper With {
    .Orientation = NexaOrientation.Horizontal,
    .Dock = DockStyle.Top,
    .Height = 120,
    .ShowDescriptions = True,
    .ShowStepNumbers = True,
    .AllowNavigation = True
}
stepper.Steps.Add(New NexaStep With {.Key = "account", .Title = "Account", .Description = "Create account", .State = NexaStepState.Completed})
stepper.Steps.Add(New NexaStep With {.Key = "profile", .Title = "Profile", .Description = "Set up profile", .State = NexaStepState.Current})
stepper.Steps.Add(New NexaStep With {.Key = "confirm", .Title = "Confirm", .Description = "Verify details", .State = NexaStepState.Pending})
AddHandler stepper.CurrentStepChanged, Sub(s, e) Console.WriteLine($"Step: {e.Step.Title}")
```

`NexaOrientation`: `Horizontal`, `Vertical`.
`NexaStepState`: `Pending`, `Current`, `Completed`, `Error`, `Disabled`.

### NexaStep
A step item with key, title, description, state, enabled/visible, and user tag.

```csharp
var step = new NexaStep("account", "Account", "Create your account")
{
    State = NexaStepState.Current,
    Enabled = true
};
```

```vb
Dim step As New NexaStep("account", "Account", "Create your account") With {
    .State = NexaStepState.Current,
    .Enabled = True
}
```

`NexaStepState`: `Pending`, `Current`, `Completed`, `Error`, `Disabled`.

### NexaMessageBox
A themed message box dialog with customizable buttons, icons, and styling.
Replaces the standard `MessageBox` with a fully themeable alternative.

```csharp
var result = NexaMessageBox.ShowInformation(null, "Operation completed successfully!", "Success");
result = NexaMessageBox.ShowSuccess(null, "Data saved successfully", "Saved");
result = NexaMessageBox.ShowWarning(null, "This action may affect existing data.", "Warning");
result = NexaMessageBox.ShowError(null, "The requested operation could not be completed.", "Error");
result = NexaMessageBox.ShowQuestion(null, "Are you sure you want to continue?", "Confirm");
result = NexaMessageBox.ShowConfirm(null, "Save changes before closing?", "Unsaved Changes");

// Custom buttons
result = NexaMessageBox.Show(
    null,
    "Choose an action for the selected items.",
    "Bulk Actions",
    NexaDialogStyle.Standard,
    ("Apply All", NexaDialogResult.Yes, NexaButtonStyle.Primary, true),
    ("Apply Selected", NexaDialogResult.OK, NexaButtonStyle.Secondary, false),
    ("Skip", NexaDialogResult.No, NexaButtonStyle.Ghost, false),
    ("Cancel", NexaDialogResult.Cancel, NexaButtonStyle.Ghost, false)
);
```

```vb
Dim result = NexaMessageBox.ShowInformation(Nothing, "Operation completed successfully!", "Success")
result = NexaMessageBox.ShowSuccess(Nothing, "Data saved successfully", "Saved")
result = NexaMessageBox.ShowWarning(Nothing, "This action may affect existing data.", "Warning")
result = NexaMessageBox.ShowError(Nothing, "The requested operation could not be completed.", "Error")
result = NexaMessageBox.ShowQuestion(Nothing, "Are you sure you want to continue?", "Confirm")
result = NexaMessageBox.ShowConfirm(Nothing, "Save changes before closing?", "Unsaved Changes")

' Custom buttons
result = NexaMessageBox.Show(
    Nothing,
    "Choose an action for the selected items.",
    "Bulk Actions",
    NexaDialogStyle.Standard,
    ("Apply All", NexaDialogResult.Yes, NexaButtonStyle.Primary, True),
    ("Apply Selected", NexaDialogResult.OK, NexaButtonStyle.Secondary, False),
    ("Skip", NexaDialogResult.No, NexaButtonStyle.Ghost, False),
    ("Cancel", NexaDialogResult.Cancel, NexaButtonStyle.Ghost, False)
)
```

`NexaDialogResult`: `None`, `OK`, `Cancel`, `Yes`, `No`, `Retry`, `Abort`, `Ignore`, `Close`.
`NexaDialogStyle`: `Standard`, `Compact`, `FullWidth`, `Card`.

### NexaInputDialog
A themed input dialog for getting text input from the user.

```csharp
var (result, text) = NexaInputDialog.ShowInput(null, "Enter your name:", "Enter Name", "John Doe");
if (result == NexaDialogResult.OK) Console.WriteLine($"Name: {text}");

var (result, text) = NexaInputDialog.ShowMultilineInput(null, "Enter your feedback:", "Feedback", "Type here...");
```

```vb
Dim (result, text) = NexaInputDialog.ShowInput(Nothing, "Enter your name:", "Enter Name", "John Doe")
If result = NexaDialogResult.OK Then Console.WriteLine($"Name: {text}")

Dim (result, text) = NexaInputDialog.ShowMultilineInput(Nothing, "Enter your feedback:", "Feedback", "Type here...")
```

### NexaToast
A toast notification with auto-dismiss, stacking, and multiple styles.

```csharp
var manager = new NexaToastManager(this);
manager.ShowInfo("Information toast");
manager.ShowSuccess("Operation completed successfully");
manager.ShowWarning("Please review your changes");
manager.ShowError("Failed to save the document");

// Or use static method
NexaToast.Show(this, "Message", "Title", NexaToastStyle.Success, NexaToastPosition.TopRight, 5000);
```

```vb
Dim manager = New NexaToastManager(Me)
manager.ShowInfo("Information toast")
manager.ShowSuccess("Operation completed successfully")
manager.ShowWarning("Please review your changes")
manager.ShowError("Failed to save the document")
```

`NexaToastStyle`: `Default`, `Success`, `Warning`, `Error`, `Info`.
`NexaToastPosition`: `TopLeft`, `TopCenter`, `TopRight`, `BottomLeft`, `BottomCenter`, `BottomRight`.

### NexaToolTip
A themed tooltip with fade animations and configurable positioning.

```csharp
var tip = new NexaToolTip
{
    InitialDelay = 300,
    AutoPopDelay = 5000,
    Position = NexaTooltipPosition.Top
};
tip.SetToolTip(btn, "This tooltip appears on top of the button.");
```

```vb
Dim tip = New NexaToolTip With {
    .InitialDelay = 300,
    .AutoPopDelay = 5000,
    .Position = NexaTooltipPosition.Top
}
tip.SetToolTip(btn, "This tooltip appears on top of the button.")
```

`NexaTooltipPosition`: `Top`, `Bottom`, `Left`, `Right`.

### NexaPopover
A rich popover with title, content, and flexible positioning.

```csharp
var popover = new NexaPopover
{
    Title = "Popover Title",
    Position = NexaPopoverPosition.Bottom,
    Width = 280,
    Height = 160,
    ShowCloseButton = true
};
popover.ContentPanel.Controls.Add(new NexaLabel { Text = "Rich content here..." });
popover.ContentPanel.Controls.Add(new NexaButton { Text = "Action" });
popover.Show(btn, NexaPopoverPosition.Bottom);
```

```vb
Dim popover = New NexaPopover With {
    .Title = "Popover Title",
    .Position = NexaPopoverPosition.Bottom,
    .Width = 280,
    .Height = 160,
    .ShowCloseButton = True
}
popover.ContentPanel.Controls.Add(New NexaLabel With {.Text = "Rich content here..."})
popover.ContentPanel.Controls.Add(New NexaButton With {.Text = "Action"})
popover.Show(btn, NexaPopoverPosition.Bottom)
```

`NexaPopoverPosition`: `Top`, `Bottom`, `Left`, `Right`, `TopLeft`, `TopRight`, `BottomLeft`, `BottomRight`.

### NexaLoadingOverlay
A full-screen or container-scoped loading overlay with spinner, title, and message.

```csharp
var overlay = new NexaLoadingOverlay
{
    Title = "Processing Data",
    Message = "Please wait while we fetch the data...",
    OverlayStyle = NexaOverlayStyle.Standard,
    Dock = DockStyle.Fill
};
overlay.Show();
// ... do work ...
overlay.Hide();
```

```vb
Dim overlay = New NexaLoadingOverlay With {
    .Title = "Processing Data",
    .Message = "Please wait while we fetch the data...",
    .OverlayStyle = NexaOverlayStyle.Standard,
    .Dock = DockStyle.Fill
}
overlay.Show()
' ... do work ...
overlay.Hide()
```

`NexaOverlayStyle`: `Standard`, `Light`, `Blur`, `Minimal`.

### NexaModalBackground
A semi-transparent modal background overlay for dialogs and drawers.

```csharp
var modalBg = new NexaModalBackground
{
    OverlayStyle = NexaOverlayStyle.Standard,
    ClickToClose = true,
    Dock = DockStyle.Fill
};
var content = new NexaCard { Title = "Confirm", Width = 400, Height = 200 };
modalBg.ShowWithContent(content);
```

```vb
Dim modalBg = New NexaModalBackground With {
    .OverlayStyle = NexaOverlayStyle.Standard,
    .ClickToClose = True,
    .Dock = DockStyle.Fill
}
Dim content = New NexaCard With {.Title = "Confirm", .Width = 400, .Height = 200}
modalBg.ShowWithContent(content)
```
```

`NexaOverlayStyle`: `Standard`, `Light`, `Blur`, `Minimal`.

### NexaDataGridView

A theme-aware DataGridView that inherits from `System.Windows.Forms.DataGridView`.
Preserves all native functionality (columns, rows, data binding, sorting, editing,
virtual mode) while adding modern NexaUI styling.

```csharp
var grid = new NexaDataGridView
{
    Dock = DockStyle.Fill,
    GridStyle = NexaGridStyle.Default,
    HeaderStyle = NexaHeaderStyle.Standard,
    ShowRowNumbers = true,
    AlternateRowColors = true,
    ReadOnly = true
};
```

```vb
Dim grid = New NexaDataGridView With {
    .Dock = DockStyle.Fill,
    .GridStyle = NexaGridStyle.Default,
    .HeaderStyle = NexaHeaderStyle.Standard,
    .ShowRowNumbers = True,
    .AlternateRowColors = True,
    .ReadOnly = True
}
```

`NexaGridStyle`: `Default`, `Compact`, `Comfortable`.

`NexaHeaderStyle`: `Standard`, `Emphasized`, `Minimal`.

### NexaListView

A theme-aware ListView that inherits from `System.Windows.Forms.ListView`.
Preserves all native functionality (items, groups, columns, views, selection,
checkboxes, image lists) while adding modern NexaUI styling.

```csharp
var list = new NexaListView
{
    Dock = DockStyle.Fill,
    View = View.Details,
    FullRowSelect = true,
    ListStyle = NexaListStyle.Default
};
```

```vb
Dim list = New NexaListView With {
    .Dock = DockStyle.Fill,
    .View = View.Details,
    .FullRowSelect = True,
    .ListStyle = NexaListStyle.Default
}
```

`NexaListStyle`: `Default`, `Compact`, `Spacious`.

### NexaPropertyGrid

A theme-aware PropertyGrid that inherits from `System.Windows.Forms.PropertyGrid`.
Preserves all native functionality (SelectedObject, property tabs, categorization,
help area, commands, editing) while applying NexaUI colors.

```csharp
var propGrid = new NexaPropertyGrid
{
    Dock = DockStyle.Fill,
    GridStyle = NexaPropertyGridStyle.Default,
    HelpVisible = true,
    ToolbarVisible = true,
    PropertySort = PropertySort.Categorized
};
propGrid.SelectedObject = mySettingsObject;
```

```vb
Dim propGrid = New NexaPropertyGrid With {
    .Dock = DockStyle.Fill,
    .GridStyle = NexaPropertyGridStyle.Default,
    .HelpVisible = True,
    .ToolbarVisible = True,
    .PropertySort = PropertySort.Categorized
}
propGrid.SelectedObject = mySettingsObject
```

`NexaPropertyGridStyle`: `Default`, `Compact`, `Comfortable`.

### NexaTreeView

A theme-aware TreeView that inherits from `System.Windows.Forms.TreeView`.
Preserves all native functionality (nodes, parent/child relationships, selection,
checkboxes, image lists, label editing, sorting, drag/drop) while applying
NexaUI styling.

```csharp
var tree = new NexaTreeView
{
    Dock = DockStyle.Fill,
    TreeStyle = NexaTreeStyle.Default,
    ShowRootLines = true,
    ShowNodeLines = true,
    CheckBoxes = true,
    FullRowSelect = true
};
```

```vb
Dim tree = New NexaTreeView With {
    .Dock = DockStyle.Fill,
    .TreeStyle = NexaTreeStyle.Default,
    .ShowRootLines = True,
    .ShowNodeLines = True,
    .CheckBoxes = True,
    .FullRowSelect = True
}
```

`NexaTreeStyle`: `Default`, `Compact`, `Spacious`.