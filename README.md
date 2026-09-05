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

**Phase 0 - Solution scaffolding.**

The solution, projects, project references, and demo shells are in
place. No UI controls have been implemented yet.

See `NEXAUI_RULES.md` for the permanent development rules that govern
this project.