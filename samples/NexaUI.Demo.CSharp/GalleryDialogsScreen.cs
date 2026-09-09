using System.Drawing;
using System.Windows.Forms;
using NexaUI.Controls;
using NexaUI.Core;
using NexaUI.Icons;
using NexaUI.Themes;

namespace NexaUI.Demo.CSharp;

/// <summary>
/// Gallery page for Phase 12 Dialogs & Overlay Controls.
/// Demonstrates NexaMessageBox, NexaInputDialog, NexaToast, NexaToolTip,
/// NexaPopover, NexaLoadingOverlay, and NexaModalBackground.
/// </summary>
public sealed class GalleryDialogsScreen : UserControl
{
    private readonly TableLayoutPanel _root;
    private NexaToolTip? _toolTip;
    private NexaToastManager? _toastManager;
    private NexaModalBackground? _modalBackground;

    public GalleryDialogsScreen()
    {
        Dock = DockStyle.Fill;
        AutoScroll = true;
        BackColor = (Color)ThemeManager.Current.Palette[NexaColorRole.Background].Value;

        // Initialize shared components FIRST, before building UI
        InitializeSharedComponents();

        _root = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 2,
            Padding = new Padding(32, 24, 32, 32)
        };
        _root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        _root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

        var heading = new NexaLabel
        {
            Text = "Dialogs & Overlays",
            LabelStyle = NexaLabelStyle.Heading,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 8)
        };
        _root.Controls.Add(heading);
        _root.SetColumnSpan(heading, 2);

        var intro = new NexaLabel
        {
            Text = "NexaMessageBox, NexaInputDialog, NexaToast, NexaToolTip, NexaPopover, NexaLoadingOverlay, NexaModalBackground. Complete dialog and overlay system.",
            LabelStyle = NexaLabelStyle.Muted,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 16)
        };
        _root.Controls.Add(intro);
        _root.SetColumnSpan(intro, 2);

        // MessageBox
        _root.Controls.Add(SectionTitle("NexaMessageBox — Standard Dialogs"));
        _root.Controls.Add(BuildMessageBoxCard());

        _root.Controls.Add(SectionTitle("NexaMessageBox — Confirmation Dialogs"));
        _root.Controls.Add(BuildConfirmationCard());

        // InputDialog
        _root.Controls.Add(SectionTitle("NexaInputDialog — Text Input"));
        _root.Controls.Add(BuildInputDialogCard());

        // Toast
        _root.Controls.Add(SectionTitle("NexaToast — Toast Notifications"));
        _root.Controls.Add(BuildToastCard());

        // ToolTip
        _root.Controls.Add(SectionTitle("NexaToolTip — Hover Tooltips"));
        _root.Controls.Add(BuildToolTipCard());

        // Popover
        _root.Controls.Add(SectionTitle("NexaPopover — Rich Popovers"));
        _root.Controls.Add(BuildPopoverCard());

        // LoadingOverlay
        _root.Controls.Add(SectionTitle("NexaLoadingOverlay — Loading States"));
        _root.Controls.Add(BuildLoadingOverlayCard());

        // ModalBackground
        _root.Controls.Add(SectionTitle("NexaModalBackground — Modal Overlays"));
        _root.Controls.Add(BuildModalBackgroundCard());

        // Application Modal Example
        _root.Controls.Add(SectionTitle("Real-World: Application Modal Workflow"));
        _root.Controls.Add(BuildApplicationModalCard());
        _root.SetColumnSpan(BuildApplicationModalCard(), 2);

        Controls.Add(_root);

        ThemeManager.ThemeChanged += OnThemeChanged;
        Disposed += OnSelfDisposed;
    }

    private void OnSelfDisposed(object? sender, EventArgs e)
    {
        _toolTip?.Dispose();
        _toastManager?.Dispose();
        _modalBackground?.Dispose();
        ThemeManager.ThemeChanged -= OnThemeChanged;
    }

    private void OnThemeChanged(object? sender, ThemeChangedEventArgs e)
    {
        if (IsDisposed || Disposing) return;
        if (InvokeRequired)
        {
            BeginInvoke(() => OnThemeChanged(sender, e));
            return;
        }
        BackColor = (Color)e.Current.Palette[NexaColorRole.Background].Value;
    }

    private void InitializeSharedComponents()
    {
        _toolTip = new NexaToolTip
        {
            InitialDelay = 300,
            AutoPopDelay = 5000,
            Position = NexaTooltipPosition.Top
        };

        _toastManager = new NexaToastManager(this);
    }

    // ---------- MessageBox Cards ----------

    private Control BuildMessageBoxCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Standard message boxes with icons, custom buttons, and theme-aware styling."));

        var grid = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 2,
            RowCount = 4
        };
        for (var i = 0; i < 2; i++) grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        for (var i = 0; i < 4; i++) grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var infoBtn = new NexaButton { Text = "Show Information", Width = 180, Style = NexaButtonStyle.Primary };
        infoBtn.Click += (_, _) => NexaMessageBox.ShowInformation(this, "This is an information message.", "Information");
        grid.Controls.Add(infoBtn, 0, 0);
        grid.Controls.Add(new NexaLabel { Text = "NexaMessageBox.ShowInformation()", LabelStyle = NexaLabelStyle.Caption, AutoSize = true, Margin = new Padding(8, 8, 0, 0) }, 1, 0);

        var successBtn = new NexaButton { Text = "Show Success", Width = 180, Style = NexaButtonStyle.Success };
        successBtn.Click += (_, _) => NexaMessageBox.ShowSuccess(this, "Operation completed successfully!", "Success");
        grid.Controls.Add(successBtn, 0, 1);
        grid.Controls.Add(new NexaLabel { Text = "NexaMessageBox.ShowSuccess()", LabelStyle = NexaLabelStyle.Caption, AutoSize = true, Margin = new Padding(8, 8, 0, 0) }, 1, 1);

        var warningBtn = new NexaButton { Text = "Show Warning", Width = 180, Style = NexaButtonStyle.Warning };
        warningBtn.Click += (_, _) => NexaMessageBox.ShowWarning(this, "This action may affect existing data.", "Warning");
        grid.Controls.Add(warningBtn, 0, 2);
        grid.Controls.Add(new NexaLabel { Text = "NexaMessageBox.ShowWarning()", LabelStyle = NexaLabelStyle.Caption, AutoSize = true, Margin = new Padding(8, 8, 0, 0) }, 1, 2);

        var errorBtn = new NexaButton { Text = "Show Error", Width = 180, Style = NexaButtonStyle.Danger };
        errorBtn.Click += (_, _) => NexaMessageBox.ShowError(this, "The requested operation could not be completed.", "Error");
        grid.Controls.Add(errorBtn, 0, 3);
        grid.Controls.Add(new NexaLabel { Text = "NexaMessageBox.ShowError()", LabelStyle = NexaLabelStyle.Caption, AutoSize = true, Margin = new Padding(8, 8, 0, 0) }, 1, 3);

        card.Controls.Add(grid);
        return card;
    }

    private Control BuildConfirmationCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Confirmation dialogs with Yes/No, Yes/No/Cancel, and custom button layouts."));

        var grid = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 2,
            RowCount = 3
        };
        for (var i = 0; i < 2; i++) grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        for (var i = 0; i < 3; i++) grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var questionBtn = new NexaButton { Text = "Show Question (Yes/No)", Width = 200, Style = NexaButtonStyle.Primary };
        questionBtn.Click += (_, _) =>
        {
            var result = NexaMessageBox.ShowQuestion(this, "Are you sure you want to continue?", "Confirm Action");
            ShowResult(result);
        };
        grid.Controls.Add(questionBtn, 0, 0);
        grid.Controls.Add(new NexaLabel { Text = "NexaMessageBox.ShowQuestion()", LabelStyle = NexaLabelStyle.Caption, AutoSize = true, Margin = new Padding(8, 8, 0, 0) }, 1, 0);

        var confirmBtn = new NexaButton { Text = "Show Confirm (Yes/No/Cancel)", Width = 200, Style = NexaButtonStyle.Secondary };
        confirmBtn.Click += (_, _) =>
        {
            var result = NexaMessageBox.ShowConfirm(this, "Save changes before closing?", "Unsaved Changes");
            ShowResult(result);
        };
        grid.Controls.Add(confirmBtn, 0, 1);
        grid.Controls.Add(new NexaLabel { Text = "NexaMessageBox.ShowConfirm()", LabelStyle = NexaLabelStyle.Caption, AutoSize = true, Margin = new Padding(8, 8, 0, 0) }, 1, 1);

        var customBtn = new NexaButton { Text = "Custom Buttons", Width = 200, Style = NexaButtonStyle.Outline };
        customBtn.Click += (_, _) =>
        {
            var result = NexaMessageBox.Show(
                this,
                "Choose an action for the selected items.",
                "Bulk Actions",
                NexaDialogStyle.Standard,
                ("Apply All", NexaDialogResult.Yes, NexaButtonStyle.Primary, true),
                ("Apply Selected", NexaDialogResult.OK, NexaButtonStyle.Secondary, false),
                ("Skip", NexaDialogResult.No, NexaButtonStyle.Ghost, false),
                ("Cancel", NexaDialogResult.Cancel, NexaButtonStyle.Ghost, false)
            );
            ShowResult(result);
        };
        grid.Controls.Add(customBtn, 0, 2);
        grid.Controls.Add(new NexaLabel { Text = "NexaMessageBox.Show() with custom buttons", LabelStyle = NexaLabelStyle.Caption, AutoSize = true, Margin = new Padding(8, 8, 0, 0) }, 1, 2);

        card.Controls.Add(grid);
        return card;
    }

    private void ShowResult(NexaDialogResult result)
    {
        var style = result switch
        {
            NexaDialogResult.OK or NexaDialogResult.Yes => NexaToastStyle.Success,
            NexaDialogResult.No or NexaDialogResult.Cancel => NexaToastStyle.Warning,
            _ => NexaToastStyle.Info
        };
        _toastManager!.Show($"Dialog result: {result}", "Dialog Result", style);
    }

    // ---------- InputDialog Card ----------

    private Control BuildInputDialogCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Input dialogs for single-line and multi-line text entry with validation."));

        var grid = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 2,
            RowCount = 3
        };
        for (var i = 0; i < 2; i++) grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        for (var i = 0; i < 3; i++) grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var singleBtn = new NexaButton { Text = "Single-line Input", Width = 200, Style = NexaButtonStyle.Primary };
        singleBtn.Click += async (_, _) =>
        {
            var (result, text) = await Task.Run(() => NexaInputDialog.ShowInput(this, "Enter your name:", "Enter Name", "John Doe"));
            if (result == NexaDialogResult.OK)
            {
                _toastManager!.Show($"Entered: {text}", "Input Received", NexaToastStyle.Success);
            }
        };
        grid.Controls.Add(singleBtn, 0, 0);
        grid.Controls.Add(new NexaLabel { Text = "NexaInputDialog.ShowInput()", LabelStyle = NexaLabelStyle.Caption, AutoSize = true, Margin = new Padding(8, 8, 0, 0) }, 1, 0);

        var multiBtn = new NexaButton { Text = "Multi-line Input", Width = 200, Style = NexaButtonStyle.Secondary };
        multiBtn.Click += async (_, _) =>
        {
            var (result, text) = await Task.Run(() => NexaInputDialog.ShowMultilineInput(this, "Enter your feedback:", "Feedback", "Type here..."));
            if (result == NexaDialogResult.OK)
            {
                _toastManager!.Show($"Feedback received ({text.Length} chars)", "Input Received", NexaToastStyle.Success);
            }
        };
        grid.Controls.Add(multiBtn, 0, 1);
        grid.Controls.Add(new NexaLabel { Text = "NexaInputDialog.ShowMultilineInput()", LabelStyle = NexaLabelStyle.Caption, AutoSize = true, Margin = new Padding(8, 8, 0, 0) }, 1, 1);

        var placeholderBtn = new NexaButton { Text = "With Placeholder", Width = 200, Style = NexaButtonStyle.Outline };
        placeholderBtn.Click += async (_, _) =>
        {
            var (result, text) = await Task.Run(() => NexaInputDialog.Show(this, "Search for items...", "Search", "", "Type to search..."));
            if (result == NexaDialogResult.OK)
            {
                _toastManager!.Show($"Searching for: {text}", "Search", NexaToastStyle.Info);
            }
        };
        grid.Controls.Add(placeholderBtn, 0, 2);
        grid.Controls.Add(new NexaLabel { Text = "NexaInputDialog.Show() with placeholder", LabelStyle = NexaLabelStyle.Caption, AutoSize = true, Margin = new Padding(8, 8, 0, 0) }, 1, 2);

        card.Controls.Add(grid);
        return card;
    }

    // ---------- Toast Card ----------

    private Control BuildToastCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Toast notifications with auto-dismiss, stacking, and multiple styles."));

        var grid = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 4,
            RowCount = 2
        };
        for (var i = 0; i < 4; i++) grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        for (var i = 0; i < 2; i++) grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var toastTypes = new (string Label, NexaToastStyle Style, string Text)[]
        {
            ("Info", NexaToastStyle.Info, "Information toast with blue accent"),
            ("Success", NexaToastStyle.Success, "Operation completed successfully"),
            ("Warning", NexaToastStyle.Warning, "Please review your changes"),
            ("Error", NexaToastStyle.Error, "Failed to save the document"),
            ("Info (Top-Left)", NexaToastStyle.Info, "Positioned at top-left"),
            ("Success (Bottom)", NexaToastStyle.Success, "Positioned at bottom-center"),
            ("Warning (Custom)", NexaToastStyle.Warning, "Custom auto-close delay: 10s"),
            ("Error (Persistent)", NexaToastStyle.Error, "No auto-close (0 = stay open)")
        };

        for (var i = 0; i < toastTypes.Length; i++)
        {
            var (label, toastStyle, text) = toastTypes[i];
            var btn = new NexaButton { Text = label, Width = 140, Height = 40, Style = toastStyle switch { NexaToastStyle.Success => NexaButtonStyle.Success, NexaToastStyle.Warning => NexaButtonStyle.Warning, NexaToastStyle.Error => NexaButtonStyle.Danger, _ => NexaButtonStyle.Primary } };
            var t = text;
            btn.Click += (_, _) =>
            {
                var pos = label.Contains("Top-Left") ? NexaToastPosition.TopLeft :
                          label.Contains("Bottom") ? NexaToastPosition.BottomCenter :
                          NexaToastPosition.TopRight;
                var delay = label.Contains("10s") ? 10000 : label.Contains("Persistent") ? 0 : 5000;
                _toastManager!.Show(t, label, toastStyle, pos, delay);
            };
            grid.Controls.Add(btn, i % 4, i / 4);
        }

        card.Controls.Add(grid);
        return card;
    }

    // ---------- ToolTip Card ----------

    private Control BuildToolTipCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Hover over the buttons below to see tooltips with different positions."));

        var flow = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true
        };

        var positions = new (string Label, NexaTooltipPosition Pos)[]
        {
            ("Top", NexaTooltipPosition.Top),
            ("Bottom", NexaTooltipPosition.Bottom),
            ("Left", NexaTooltipPosition.Left),
            ("Right", NexaTooltipPosition.Right)
        };

        foreach (var (label, pos) in positions)
        {
            var btn = new NexaButton
            {
                Text = $"Hover me ({label})",
                Width = 160,
                Height = 50,
                Style = NexaButtonStyle.Secondary,
                Margin = new Padding(8)
            };
            _toolTip!.SetToolTip(btn, $"This tooltip appears on the {label.ToLower()} side of the control. It fades in and out smoothly.");
            flow.Controls.Add(btn);
        }

        card.Controls.Add(flow);
        return card;
    }

    // ---------- Popover Card ----------

    private Control BuildPopoverCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Rich popovers with titles, content, and flexible positioning. Click buttons to open."));

        var flow = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true
        };

        var positions = new (string Label, NexaPopoverPosition Pos)[]
        {
            ("Bottom", NexaPopoverPosition.Bottom),
            ("Top", NexaPopoverPosition.Top),
            ("Left", NexaPopoverPosition.Left),
            ("Right", NexaPopoverPosition.Right),
            ("Bottom-Right", NexaPopoverPosition.BottomRight)
        };

        foreach (var (label, pos) in positions)
        {
            var btn = new NexaButton
            {
                Text = $"Open ({label})",
                Width = 130,
                Style = NexaButtonStyle.Secondary,
                Margin = new Padding(8)
            };

            var position = pos;
            btn.Click += (_, _) =>
            {
                var popover = new NexaPopover
                {
                    Title = $"Popover ({label})",
                    Position = position,
                    Width = 280,
                    Height = 160,
                    ShowCloseButton = true
                };
                popover.ContentPanel.Controls.Add(new NexaLabel
                {
                    Text = $"This popover is anchored to the {label.ToLower()} of the button. It supports rich content including forms, lists, and other controls.",
                    LabelStyle = NexaLabelStyle.Muted,
                    AutoSize = true,
                    Dock = DockStyle.Top,
                    Margin = new Padding(0, 0, 0, 12)
                });
                popover.ContentPanel.Controls.Add(new NexaButton
                {
                    Text = "Action Button",
                    Style = NexaButtonStyle.Primary,
                    Dock = DockStyle.Bottom,
                    Margin = new Padding(0, 12, 0, 0)
                });
                popover.Show(btn, pos);
            };
            flow.Controls.Add(btn);
        }

        card.Controls.Add(flow);
        return card;
    }

    // ---------- LoadingOverlay Card ----------

    private Control BuildLoadingOverlayCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Full-screen or container-scoped loading overlays with spinner, title, and message."));

        var overlay = new NexaLoadingOverlay
        {
            Title = "Processing Data",
            Message = "Please wait while we fetch the data...",
            OverlayStyle = NexaOverlayStyle.Standard,
            Visible = false
        };

        var btnShow = new NexaButton { Text = "Show Loading (3 sec)", Width = 200, Style = NexaButtonStyle.Primary };
        btnShow.Click += async (_, _) =>
        {
            overlay.Show();
            await Task.Delay(3000);
            overlay.Hide();
        };

        var btnStyle = new NexaButton { Text = "Show Light Style", Width = 200, Style = NexaButtonStyle.Secondary, Margin = new Padding(0, 8, 0, 0) };
        btnStyle.Click += async (_, _) =>
        {
            overlay.OverlayStyle = NexaOverlayStyle.Light;
            overlay.Title = "Light Overlay";
            overlay.Message = "Subtle loading indicator";
            overlay.Show();
            await Task.Delay(2000);
            overlay.Hide();
        };

        var overlayContainer = new Panel
        {
            Dock = DockStyle.Top,
            Height = 200,
            BorderStyle = BorderStyle.FixedSingle,
            Margin = new Padding(0, 8, 0, 0)
        };
        overlayContainer.Controls.Add(overlay);

        var flow = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false
        };
        flow.Controls.Add(btnShow);
        flow.Controls.Add(btnStyle);
        flow.Controls.Add(overlayContainer);

        card.Controls.Add(flow);
        return card;
    }

    // ---------- ModalBackground Card ----------

    private Control BuildModalBackgroundCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Modal background overlay with centered content. Click background to close (when enabled)."));

        var btnModal = new NexaButton { Text = "Open Modal Dialog", Width = 200, Style = NexaButtonStyle.Primary };
        btnModal.Click += (_, _) =>
        {
            _modalBackground = new NexaModalBackground
            {
                OverlayStyle = NexaOverlayStyle.Standard,
                ClickToClose = true,
                Dock = DockStyle.Fill
            };

            var modalContent = new NexaCard
            {
                Title = "Confirm Action",
                Subtitle = "This action cannot be undone",
                Width = 400,
                Height = 220,
                CornerRadius = 8,
                ShadowEnabled = true,
                ShadowDepth = 8
            };
            modalContent.ContentPanel.Controls.Add(new NexaLabel
            {
                Text = "Are you sure you want to delete the selected items? This action cannot be undone.",
                LabelStyle = NexaLabelStyle.Muted,
                AutoSize = true,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 0, 0, 24)
            });

            var btnRow = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.RightToLeft
            };
            var btnCancel = new NexaButton { Text = "Cancel", Style = NexaButtonStyle.Ghost, Width = 90, Margin = new Padding(0, 0, 8, 0) };
            var btnDelete = new NexaButton { Text = "Delete", Style = NexaButtonStyle.Danger, Width = 90 };
            btnCancel.Click += (_, _) => _modalBackground!.CloseModal();
            btnDelete.Click += (_, _) =>
            {
                _modalBackground!.CloseModal();
                _toastManager!.Show("Items deleted successfully", "Deleted", NexaToastStyle.Success);
            };
            btnRow.Controls.AddRange(new Control[] { btnDelete, btnCancel });
            modalContent.ContentPanel.Controls.Add(btnRow);

            Controls.Add(_modalBackground);
            _modalBackground.ShowWithContent(modalContent);
        };

        var btnNoClose = new NexaButton { Text = "Modal (No Background Click)", Width = 200, Style = NexaButtonStyle.Secondary, Margin = new Padding(0, 8, 0, 0) };
        btnNoClose.Click += (_, _) =>
        {
            _modalBackground = new NexaModalBackground
            {
                OverlayStyle = NexaOverlayStyle.Standard,
                ClickToClose = false,
                Dock = DockStyle.Fill
            };

            var modalContent = new NexaCard
            {
                Title = "Important Notice",
                Subtitle = "Please read carefully",
                Width = 400,
                Height = 200,
                CornerRadius = 8,
                ShadowEnabled = true,
                ShadowDepth = 8
            };
            modalContent.ContentPanel.Controls.Add(new NexaLabel
            {
                Text = "This modal cannot be closed by clicking the background. Use the button below.",
                LabelStyle = NexaLabelStyle.Muted,
                AutoSize = true,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 0, 0, 24)
            });

            var btnOk = new NexaButton { Text = "OK", Style = NexaButtonStyle.Primary, Width = 90, Dock = DockStyle.Bottom };
            btnOk.Click += (_, _) => _modalBackground!.CloseModal();
            modalContent.ContentPanel.Controls.Add(btnOk);

            Controls.Add(_modalBackground);
            _modalBackground.ShowWithContent(modalContent);
        };

        var flow = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false
        };
        flow.Controls.Add(btnModal);
        flow.Controls.Add(btnNoClose);
        card.Controls.Add(flow);
        return card;
    }

    // ---------- Application Modal Example ----------

    private Control BuildApplicationModalCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Complete application modal workflow demonstrating modal background, card, buttons, and toast integration."));

        var btnStart = new NexaButton { Text = "Start File Import", Style = NexaButtonStyle.Primary, Width = 180, Height = 44 };
        btnStart.Click += async (_, _) =>
        {
            // Show loading overlay
            var overlay = new NexaLoadingOverlay
            {
                Title = "Importing Files",
                Message = "Preparing import...",
                OverlayStyle = NexaOverlayStyle.Standard,
                Dock = DockStyle.Fill
            };
            Controls.Add(overlay);
            overlay.Show();

            await Task.Delay(1500);

            overlay.Title = "Processing";
            overlay.Message = "Importing file 1 of 3...";
            await Task.Delay(1000);

            overlay.Message = "Importing file 2 of 3...";
            await Task.Delay(1000);

            overlay.Message = "Importing file 3 of 3...";
            await Task.Delay(1000);

            overlay.Message = "Finalizing...";
            await Task.Delay(500);

            overlay.Hide();
            Controls.Remove(overlay);
            overlay.Dispose();

            // Show success modal
            _modalBackground = new NexaModalBackground
            {
                OverlayStyle = NexaOverlayStyle.Standard,
                ClickToClose = true,
                Dock = DockStyle.Fill
            };

            var modalContent = new NexaCard
            {
                Title = "Import Complete",
                Subtitle = "All files processed successfully",
                Width = 400,
                Height = 220,
                CornerRadius = 8,
                ShadowEnabled = true,
                ShadowDepth = 8
            };
            modalContent.ContentPanel.Controls.Add(new NexaLabel
            {
                Text = "All 3 files were imported successfully.",
                LabelStyle = NexaLabelStyle.Muted,
                AutoSize = true,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 0, 0, 24)
            });

            var btnOk = new NexaButton { Text = "Close", Style = NexaButtonStyle.Primary, Width = 90, Dock = DockStyle.Bottom };
            btnOk.Click += (_, _) => _modalBackground!.CloseModal();
            modalContent.ContentPanel.Controls.Add(btnOk);

            Controls.Add(_modalBackground);
            _modalBackground.ShowWithContent(modalContent);

            _toastManager!.Show("Import completed successfully", "Import", NexaToastStyle.Success);
        };

        card.Controls.Add(btnStart);
        return card;
    }

    // ---------- Helpers ----------

    private static NexaLabel SectionTitle(string title) => new()
    {
        Text = title,
        AutoSize = true,
        LabelStyle = NexaLabelStyle.Subheading,
        Margin = new Padding(0, 8, 0, 8)
    };

    private static Panel CreateDemoCard()
    {
        var palette = ThemeManager.Current.Palette;
        var card = new Panel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(16),
            Margin = new Padding(0, 0, 0, 12),
            BackColor = (Color)palette[NexaColorRole.Surface].Value
        };
        card.Paint += (s, e) =>
        {
            using var pen = new Pen((Color)palette[NexaColorRole.Border].Value, 1F);
            var r = card.ClientRectangle;
            r.Width -= 1; r.Height -= 1;
            e.Graphics.DrawRectangle(pen, r);
        };
        return card;
    }

    private static NexaLabel DescriptionLabel(string text) => new()
    {
        Text = text,
        Dock = DockStyle.Top,
        AutoSize = true,
        LabelStyle = NexaLabelStyle.Muted,
        Margin = new Padding(0, 0, 0, 8)
    };
}