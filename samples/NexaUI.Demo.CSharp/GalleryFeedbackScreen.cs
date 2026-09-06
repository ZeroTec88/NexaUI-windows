using System.Drawing;
using System.Windows.Forms;
using NexaUI.Controls;
using NexaUI.Core;
using NexaUI.Themes;

namespace NexaUI.Demo.CSharp;

/// <summary>
/// Phase 10 gallery: NexaProgressBar, NexaCircularProgress, NexaSpinner,
/// NexaBadge, NexaAlert, NexaStatusIndicator — plus an Operation Feedback
/// composition example that drives them together with simulated progress.
/// </summary>
public sealed class GalleryFeedbackScreen : UserControl
{
    private readonly TableLayoutPanel _root;

    // Operation Feedback
    private NexaCard _operationCard = null!;
    private NexaLabel _operationTitle = null!;
    private NexaProgressBar _operationProgress = null!;
    private NexaStatusIndicator _operationStatus = null!;
    private NexaSpinner _operationSpinner = null!;
    private NexaLabel _operationDetail = null!;
    private readonly System.Windows.Forms.Timer _operationTimer;
    private bool _operationRunning;
    private bool _operationPaused;

    // Auto-hide alert
    private NexaAlert _infoAlert = null!;
    private NexaAlert _successAlert = null!;
    private NexaAlert _warningAlert = null!;
    private NexaAlert _errorAlert = null!;
    private NexaAlert _autoCloseAlert = null!;

    public GalleryFeedbackScreen()
    {
        Dock = DockStyle.Fill;
        AutoScroll = true;
        BackColor = (Color)ThemeManager.Current.Palette[NexaColorRole.Background].Value;

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
            Text = "Feedback Controls",
            LabelStyle = NexaLabelStyle.Heading,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 8)
        };
        _root.Controls.Add(heading);
        _root.SetColumnSpan(heading, 2);

        var intro = new NexaLabel
        {
            Text = "NexaProgressBar, NexaCircularProgress, NexaSpinner, NexaBadge, NexaAlert, NexaStatusIndicator. Modern visual feedback for desktop applications.",
            LabelStyle = NexaLabelStyle.Muted,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 16)
        };
        _root.Controls.Add(intro);
        _root.SetColumnSpan(intro, 2);

        // ---- Progress bar section ----
        _root.Controls.Add(SectionTitle("NexaProgressBar — interactive with controls"));
        _root.Controls.Add(BuildProgressBarCard());

        // ---- Progress bar styles ----
        _root.Controls.Add(SectionTitle("NexaProgressBar — styles"));
        _root.Controls.Add(BuildProgressBarStylesCard());

        // ---- Circular progress ----
        _root.Controls.Add(SectionTitle("NexaCircularProgress — sizes and styles"));
        _root.Controls.Add(BuildCircularProgressCard());

        // ---- Spinner ----
        _root.Controls.Add(SectionTitle("NexaSpinner — Ring and Dots"));
        _root.Controls.Add(BuildSpinnerCard());

        // ---- Badge ----
        _root.Controls.Add(SectionTitle("NexaBadge — styles and sizes"));
        _root.Controls.Add(BuildBadgeCard());

        // ---- Alert ----
        _root.Controls.Add(SectionTitle("NexaAlert — Information / Success / Warning / Error"));
        _root.Controls.Add(BuildAlertCard());

        // ---- Status indicator ----
        _root.Controls.Add(SectionTitle("NexaStatusIndicator — Online / Offline / Busy / etc."));
        _root.Controls.Add(BuildStatusIndicatorCard());

        // ---- Operation Feedback (full-width) ----
        _root.Controls.Add(SectionTitle("Operation Feedback — real-world composition"));
        _operationCard = BuildOperationFeedbackCard();
        _root.Controls.Add(_operationCard);
        _root.SetColumnSpan(_operationCard, 2);

        Controls.Add(_root);

        _operationTimer = new System.Windows.Forms.Timer { Interval = 80 };
        _operationTimer.Tick += OnOperationTimerTick;

        ThemeManager.ThemeChanged += OnThemeChanged;
        Disposed += OnSelfDisposed;
    }

    private void OnSelfDisposed(object? sender, EventArgs e)
    {
        _operationTimer.Stop();
        _operationTimer.Dispose();
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

    // ---------- Cards ----------

    private Control BuildProgressBarCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Interactive progress bar with increment/decrement/set controls."));

        var bar = new NexaProgressBar { Minimum = 0, Maximum = 100, Value = 35, Width = 320, Dock = DockStyle.Top };

        var status = new NexaLabel { Text = "Current: 35%", AutoSize = true, Dock = DockStyle.Top, LabelStyle = NexaLabelStyle.Caption };

        var row = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Margin = new Padding(0, 8, 0, 0)
        };
        var minus = new NexaButton { Text = "-10", Width = 60 };
        var plus = new NexaButton { Text = "+10", Width = 60 };
        var set50 = new NexaButton { Text = "Set 50", Width = 70 };
        var set100 = new NexaButton { Text = "Set 100", Width = 70 };
        minus.Click += (_, _) => { bar.Value = Math.Max(0, bar.Value - 10); status.Text = $"Current: {bar.Value}%"; };
        plus.Click += (_, _) => { bar.Value = Math.Min(100, bar.Value + 10); status.Text = $"Current: {bar.Value}%"; };
        set50.Click += (_, _) => { bar.Value = 50; status.Text = $"Current: {bar.Value}%"; };
        set100.Click += (_, _) => { bar.Value = 100; status.Text = $"Current: {bar.Value}%"; };
        bar.ValueChanged += (_, _) => status.Text = $"Current: {bar.Value}%";
        row.Controls.AddRange(new Control[] { minus, plus, set50, set100 });

        card.Controls.Add(bar);
        card.Controls.Add(status);
        card.Controls.Add(row);
        return card;
    }

    private Control BuildProgressBarStylesCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Default, Success, Warning, Danger, Info. Value = 60%."));

        var grid = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 2,
            RowCount = 5
        };
        for (var i = 0; i < 2; i++) grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        for (var i = 0; i < 5; i++) grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var styles = new (string Label, NexaProgressStyle Style)[]
        {
            ("Default", NexaProgressStyle.Default),
            ("Success", NexaProgressStyle.Success),
            ("Warning", NexaProgressStyle.Warning),
            ("Danger", NexaProgressStyle.Danger),
            ("Info", NexaProgressStyle.Info)
        };
        for (var i = 0; i < styles.Length; i++)
        {
            var (label, style) = styles[i];
            grid.Controls.Add(new NexaLabel { Text = label, AutoSize = true, Margin = new Padding(0, 8, 8, 4) }, 0, i);
            grid.Controls.Add(new NexaProgressBar { Value = 60, ProgressStyle = style, Width = 220, Margin = new Padding(0, 4, 0, 4) }, 1, i);
        }
        card.Controls.Add(grid);
        return card;
    }

    private Control BuildCircularProgressCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Various sizes, styles, and center text. Indeterminate mode available."));

        var row = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true
        };
        row.Controls.Add(MakeCircular(25, NexaProgressStyle.Default, 56, 4, null));
        row.Controls.Add(MakeCircular(50, NexaProgressStyle.Success, 64, 5, null));
        row.Controls.Add(MakeCircular(75, NexaProgressStyle.Warning, 72, 6, null));
        row.Controls.Add(MakeCircular(100, NexaProgressStyle.Danger, 80, 6, null));
        row.Controls.Add(MakeCircular(0, NexaProgressStyle.Info, 88, 7, "Loading"));
        card.Controls.Add(row);
        return card;
    }

    private static NexaCircularProgress MakeCircular(int value, NexaProgressStyle style, int size, int thickness, string? centerText)
    {
        var c = new NexaCircularProgress
        {
            Value = value,
            ProgressStyle = style,
            Size = new Size(size, size),
            LineThickness = thickness,
            Margin = new Padding(0, 0, 12, 0)
        };
        if (centerText is not null) c.CenterText = centerText;
        return c;
    }

    private Control BuildSpinnerCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Animated loading indicators. Ring (default) and Dots styles."));

        var row = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true
        };
        var ringDefault = new NexaSpinner { SpinnerSize = 32, AnimationSpeed = 80, SpinnerStyle = NexaSpinnerStyle.Ring, Margin = new Padding(0, 0, 16, 0) };
        var ringFast = new NexaSpinner { SpinnerSize = 40, AnimationSpeed = 30, SpinnerStyle = NexaSpinnerStyle.Ring, Margin = new Padding(0, 0, 16, 0) };
        var ringSlow = new NexaSpinner { SpinnerSize = 32, AnimationSpeed = 200, SpinnerStyle = NexaSpinnerStyle.Ring, Margin = new Padding(0, 0, 16, 0) };
        var dots = new NexaSpinner { SpinnerSize = 48, AnimationSpeed = 100, SpinnerStyle = NexaSpinnerStyle.Dots, Margin = new Padding(0, 0, 16, 0) };
        var paused = new NexaSpinner { SpinnerSize = 32, AnimationEnabled = false, Margin = new Padding(0, 0, 16, 0) };

        var labels = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true
        };
        labels.Controls.Add(new NexaLabel { Text = "Ring (80ms)", AutoSize = true, Width = 120, LabelStyle = NexaLabelStyle.Caption });
        labels.Controls.Add(new NexaLabel { Text = "Ring (30ms fast)", AutoSize = true, Width = 120, LabelStyle = NexaLabelStyle.Caption });
        labels.Controls.Add(new NexaLabel { Text = "Ring (200ms slow)", AutoSize = true, Width = 140, LabelStyle = NexaLabelStyle.Caption });
        labels.Controls.Add(new NexaLabel { Text = "Dots", AutoSize = true, Width = 80, LabelStyle = NexaLabelStyle.Caption });
        labels.Controls.Add(new NexaLabel { Text = "Paused", AutoSize = true, Width = 80, LabelStyle = NexaLabelStyle.Caption });

        row.Controls.AddRange(new Control[] { ringDefault, ringFast, ringSlow, dots, paused });
        card.Controls.Add(row);
        card.Controls.Add(labels);
        return card;
    }

    private Control BuildBadgeCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Compact status/count indicators. Next to labels, buttons, and cards."));

        var grid = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 2,
            RowCount = 6
        };
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
        for (var i = 0; i < 6; i++) grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        AddBadgeRow(grid, 0, "Default", new NexaBadge { Text = "NEW", BadgeStyle = NexaBadgeStyle.Default, BadgeSize = NexaBadgeSize.Medium });
        AddBadgeRow(grid, 1, "Primary", new NexaBadge { Text = "5", BadgeStyle = NexaBadgeStyle.Primary, BadgeSize = NexaBadgeSize.Medium });
        AddBadgeRow(grid, 2, "Success", new NexaBadge { Text = "ACTIVE", BadgeStyle = NexaBadgeStyle.Success, BadgeSize = NexaBadgeSize.Medium });
        AddBadgeRow(grid, 3, "Warning", new NexaBadge { Text = "BETA", BadgeStyle = NexaBadgeStyle.Warning, BadgeSize = NexaBadgeSize.Medium });
        AddBadgeRow(grid, 4, "Danger (99+)", new NexaBadge { Text = "1234", BadgeStyle = NexaBadgeStyle.Danger, BadgeSize = NexaBadgeSize.Medium, MaximumCharacters = 2 });
        AddBadgeRow(grid, 5, "Muted", new NexaBadge { Text = "v1.0", BadgeStyle = NexaBadgeStyle.Muted, BadgeSize = NexaBadgeSize.Small });

        card.Controls.Add(grid);
        return card;
    }

    private static void AddBadgeRow(TableLayoutPanel grid, int row, string label, NexaBadge badge)
    {
        grid.Controls.Add(new NexaLabel { Text = label, AutoSize = true, Margin = new Padding(0, 8, 8, 4) }, 0, row);
        grid.Controls.Add(badge, 1, row);
    }

    private Control BuildAlertCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Inline notification panels with icon, title, message, and close button."));

        _infoAlert = new NexaAlert
        {
            AlertStyle = NexaAlertStyle.Information,
            Title = "Information",
            Message = "Your account settings have been updated.",
            Dock = DockStyle.Top,
            Margin = new Padding(0, 0, 0, 8)
        };
        _successAlert = new NexaAlert
        {
            AlertStyle = NexaAlertStyle.Success,
            Title = "Success",
            Message = "The operation completed successfully.",
            Dock = DockStyle.Top,
            Margin = new Padding(0, 0, 0, 8)
        };
        _warningAlert = new NexaAlert
        {
            AlertStyle = NexaAlertStyle.Warning,
            Title = "Warning",
            Message = "This action may affect existing data.",
            Dock = DockStyle.Top,
            Margin = new Padding(0, 0, 0, 8)
        };
        _errorAlert = new NexaAlert
        {
            AlertStyle = NexaAlertStyle.Error,
            Title = "Error",
            Message = "The requested operation could not be completed.",
            Dock = DockStyle.Top,
            Margin = new Padding(0, 0, 0, 8)
        };
        _autoCloseAlert = new NexaAlert
        {
            AlertStyle = NexaAlertStyle.Information,
            Title = "Auto-close",
            Message = "This alert auto-closes after 3 seconds. Click to re-trigger.",
            Dock = DockStyle.Top,
            AutoClose = true,
            AutoCloseDelayMs = 3000,
            Margin = new Padding(0, 0, 0, 8),
            Cursor = Cursors.Hand
        };
        _autoCloseAlert.Click += (_, _) =>
        {
            _autoCloseAlert.Visible = true;
            _autoCloseAlert.AutoClose = true; // restart timer
        };

        card.Controls.Add(_infoAlert);
        card.Controls.Add(_successAlert);
        card.Controls.Add(_warningAlert);
        card.Controls.Add(_errorAlert);
        card.Controls.Add(_autoCloseAlert);
        return card;
    }

    private Control BuildStatusIndicatorCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Compact status indicators with semantic theme colors."));

        var grid = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 2,
            RowCount = 7
        };
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
        for (var i = 0; i < 7; i++) grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        AddStatusRow(grid, 0, "Online", new NexaStatusIndicator { Status = NexaStatus.Online, Text = "System online" });
        AddStatusRow(grid, 1, "Offline", new NexaStatusIndicator { Status = NexaStatus.Offline, Text = "Disconnected" });
        AddStatusRow(grid, 2, "Busy", new NexaStatusIndicator { Status = NexaStatus.Busy, Text = "Processing..." });
        AddStatusRow(grid, 3, "Warning", new NexaStatusIndicator { Status = NexaStatus.Warning, Text = "Low disk space" });
        AddStatusRow(grid, 4, "Success", new NexaStatusIndicator { Status = NexaStatus.Success, Text = "Backup complete" });
        AddStatusRow(grid, 5, "Error", new NexaStatusIndicator { Status = NexaStatus.Error, Text = "Connection failed" });
        AddStatusRow(grid, 6, "No text (dot only)", new NexaStatusIndicator { Status = NexaStatus.Online, ShowText = false, Text = "(hidden)" });

        card.Controls.Add(grid);
        return card;
    }

    private static void AddStatusRow(TableLayoutPanel grid, int row, string label, NexaStatusIndicator indicator)
    {
        grid.Controls.Add(new NexaLabel { Text = label, AutoSize = true, Margin = new Padding(0, 8, 8, 4) }, 0, row);
        grid.Controls.Add(indicator, 1, row);
    }

    // ---------- Operation Feedback ----------

    private NexaCard BuildOperationFeedbackCard()
    {
        var card = new NexaCard
        {
            Title = "File Backup",
            Subtitle = "Simulated operation using NexaUI feedback controls",
            Dock = DockStyle.Top,
            CornerRadius = 4,
            ShadowEnabled = true,
            ShadowDepth = 4,
            Margin = new Padding(0, 8, 0, 0)
        };

        _operationTitle = new NexaLabel { Text = "Ready to start backup.", LabelStyle = NexaLabelStyle.Default, AutoSize = true, Dock = DockStyle.Top };
        _operationDetail = new NexaLabel { Text = "Use the buttons below to start, pause, complete, fail, or reset.", LabelStyle = NexaLabelStyle.Muted, AutoSize = true, Dock = DockStyle.Top, Margin = new Padding(0, 2, 0, 8) };

        _operationProgress = new NexaProgressBar
        {
            Minimum = 0,
            Maximum = 100,
            Value = 0,
            Width = 360,
            BarHeight = 12,
            Dock = DockStyle.Top,
            ProgressStyle = NexaProgressStyle.Info
        };

        var statusRow = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Margin = new Padding(0, 8, 0, 8)
        };
        _operationStatus = new NexaStatusIndicator { Status = NexaStatus.None, Text = "Idle", Margin = new Padding(0, 0, 12, 0) };
        _operationSpinner = new NexaSpinner { SpinnerSize = 22, AnimationEnabled = false, Margin = new Padding(0, 0, 0, 0) };
        statusRow.Controls.Add(_operationStatus);
        statusRow.Controls.Add(_operationSpinner);

        var buttonsPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false
        };
        var btnStart = new NexaButton { Text = "Start", Style = NexaButtonStyle.Primary, Width = 80, Margin = new Padding(0, 0, 8, 0) };
        var btnPause = new NexaButton { Text = "Pause", Style = NexaButtonStyle.Secondary, Width = 80, Margin = new Padding(0, 0, 8, 0) };
        var btnComplete = new NexaButton { Text = "Complete", Style = NexaButtonStyle.Success, Width = 90, Margin = new Padding(0, 0, 8, 0) };
        var btnFail = new NexaButton { Text = "Fail", Style = NexaButtonStyle.Danger, Width = 80, Margin = new Padding(0, 0, 8, 0) };
        var btnReset = new NexaButton { Text = "Reset", Style = NexaButtonStyle.Outline, Width = 80 };
        btnStart.Click += (_, _) => StartOperation();
        btnPause.Click += (_, _) => PauseOperation();
        btnComplete.Click += (_, _) => CompleteOperation();
        btnFail.Click += (_, _) => FailOperation();
        btnReset.Click += (_, _) => ResetOperation();
        buttonsPanel.Controls.AddRange(new Control[] { btnStart, btnPause, btnComplete, btnFail, btnReset });

        var alert = new NexaAlert
        {
            AlertStyle = NexaAlertStyle.Information,
            Title = "Backup status",
            Message = "Start the operation to see feedback controls working together.",
            Dock = DockStyle.Top,
            Margin = new Padding(0, 12, 0, 0)
        };
        alert.Name = "operationAlert";
        _operationCard = card;
        card.ContentPanel.Controls.Add(_operationTitle);
        card.ContentPanel.Controls.Add(_operationDetail);
        card.ContentPanel.Controls.Add(_operationProgress);
        card.ContentPanel.Controls.Add(statusRow);
        card.ContentPanel.Controls.Add(buttonsPanel);
        card.ContentPanel.Controls.Add(alert);
        return card;
    }

    private void StartOperation()
    {
        _operationRunning = true;
        _operationPaused = false;
        _operationSpinner.AnimationEnabled = true;
        _operationStatus.Status = NexaStatus.Busy;
        _operationStatus.Text = "Processing";
        _operationTitle.Text = "Backing up files...";
        _operationDetail.Text = "Please wait while files are being processed.";
        _operationProgress.ProgressStyle = NexaProgressStyle.Info;
        _operationTimer.Start();
        UpdateOperationAlert(NexaAlertStyle.Information, "Backup in progress", "Your files are being backed up. You can pause or cancel.");
    }

    private void PauseOperation()
    {
        if (!_operationRunning) return;
        _operationPaused = true;
        _operationTimer.Stop();
        _operationSpinner.AnimationEnabled = false;
        _operationStatus.Status = NexaStatus.Warning;
        _operationStatus.Text = "Paused";
        _operationTitle.Text = "Backup paused.";
        _operationDetail.Text = "Press Start to resume.";
        UpdateOperationAlert(NexaAlertStyle.Warning, "Backup paused", "The backup has been paused. Click Start to resume.");
    }

    private void CompleteOperation()
    {
        _operationTimer.Stop();
        _operationRunning = false;
        _operationPaused = false;
        _operationProgress.Value = 100;
        _operationProgress.ProgressStyle = NexaProgressStyle.Success;
        _operationSpinner.AnimationEnabled = false;
        _operationStatus.Status = NexaStatus.Success;
        _operationStatus.Text = "Completed";
        _operationTitle.Text = "Backup completed successfully.";
        _operationDetail.Text = $"Processed at {DateTime.Now:HH:mm:ss}.";
        UpdateOperationAlert(NexaAlertStyle.Success, "Backup complete", "All files were backed up successfully.");
    }

    private void FailOperation()
    {
        _operationTimer.Stop();
        _operationRunning = false;
        _operationPaused = false;
        _operationProgress.ProgressStyle = NexaProgressStyle.Danger;
        _operationSpinner.AnimationEnabled = false;
        _operationStatus.Status = NexaStatus.Error;
        _operationStatus.Text = "Failed";
        _operationTitle.Text = "Backup failed.";
        _operationDetail.Text = "An error occurred while processing files.";
        UpdateOperationAlert(NexaAlertStyle.Error, "Backup failed", "We could not complete the backup. Please try again.");
    }

    private void ResetOperation()
    {
        _operationTimer.Stop();
        _operationRunning = false;
        _operationPaused = false;
        _operationProgress.Value = 0;
        _operationProgress.ProgressStyle = NexaProgressStyle.Info;
        _operationSpinner.AnimationEnabled = false;
        _operationStatus.Status = NexaStatus.None;
        _operationStatus.Text = "Idle";
        _operationTitle.Text = "Ready to start backup.";
        _operationDetail.Text = "Use the buttons below to start, pause, complete, fail, or reset.";
        UpdateOperationAlert(NexaAlertStyle.Information, "Backup status", "Start the operation to see feedback controls working together.");
    }

    private void OnOperationTimerTick(object? sender, EventArgs e)
    {
        if (_operationPaused) return;
        if (_operationProgress.Value >= 100)
        {
            CompleteOperation();
            return;
        }
        _operationProgress.Value = Math.Min(100, _operationProgress.Value + 2);
    }

    private void UpdateOperationAlert(NexaAlertStyle style, string title, string message)
    {
        if (_operationCard is null) return;
        foreach (Control c in _operationCard.ContentPanel.Controls)
        {
            if (c is NexaAlert alert && alert.Name == "operationAlert")
            {
                alert.AlertStyle = style;
                alert.Title = title;
                alert.Message = message;
                return;
            }
        }
    }

    // ---------- helpers ----------

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
