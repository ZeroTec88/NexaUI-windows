Imports System.Drawing
Imports System.Windows.Forms
Imports NexaUI.Controls
Imports NexaUI.Core
Imports NexaUI.Themes

Public NotInheritable Class GalleryFeedbackScreen
    Inherits UserControl

    Private ReadOnly _root As TableLayoutPanel

    ' Operation Feedback
    Private _operationCard As NexaCard
    Private _operationTitle As NexaLabel
    Private _operationProgress As NexaProgressBar
    Private _operationStatus As NexaStatusIndicator
    Private _operationSpinner As NexaSpinner
    Private _operationDetail As NexaLabel
    Private _operationTimer As System.Windows.Forms.Timer
    Private _operationRunning As Boolean
    Private _operationPaused As Boolean

    Private _infoAlert As NexaAlert = Nothing
    Private _successAlert As NexaAlert = Nothing
    Private _warningAlert As NexaAlert = Nothing
    Private _errorAlert As NexaAlert = Nothing
    Private _autoCloseAlert As NexaAlert = Nothing

    Public Sub New()
        Dock = DockStyle.Fill
        AutoScroll = True
        BackColor = CType(ThemeManager.Current.Palette(NexaColorRole.Background).Value, Color)

        _root = New TableLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .ColumnCount = 2,
            .Padding = New Padding(32, 24, 32, 32)
        }
        _root.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50.0F))
        _root.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50.0F))

        Dim heading = New NexaLabel With {
            .Text = "Feedback Controls",
            .LabelStyle = NexaLabelStyle.Heading,
            .AutoSize = True,
            .Margin = New Padding(0, 0, 0, 8)
        }
        _root.Controls.Add(heading)
        _root.SetColumnSpan(heading, 2)

        Dim intro = New NexaLabel With {
            .Text = "NexaProgressBar, NexaCircularProgress, NexaSpinner, NexaBadge, NexaAlert, NexaStatusIndicator. Modern visual feedback for desktop applications.",
            .LabelStyle = NexaLabelStyle.Muted,
            .AutoSize = True,
            .Margin = New Padding(0, 0, 0, 16)
        }
        _root.Controls.Add(intro)
        _root.SetColumnSpan(intro, 2)

        _root.Controls.Add(SectionTitle("NexaProgressBar — interactive with controls"))
        _root.Controls.Add(BuildProgressBarCard())

        _root.Controls.Add(SectionTitle("NexaProgressBar — styles"))
        _root.Controls.Add(BuildProgressBarStylesCard())

        _root.Controls.Add(SectionTitle("NexaCircularProgress — sizes and styles"))
        _root.Controls.Add(BuildCircularProgressCard())

        _root.Controls.Add(SectionTitle("NexaSpinner — Ring and Dots"))
        _root.Controls.Add(BuildSpinnerCard())

        _root.Controls.Add(SectionTitle("NexaBadge — styles and sizes"))
        _root.Controls.Add(BuildBadgeCard())

        _root.Controls.Add(SectionTitle("NexaAlert — Information / Success / Warning / Error"))
        _root.Controls.Add(BuildAlertCard())

        _root.Controls.Add(SectionTitle("NexaStatusIndicator — Online / Offline / Busy / etc."))
        _root.Controls.Add(BuildStatusIndicatorCard())

        _root.Controls.Add(SectionTitle("Operation Feedback — real-world composition"))
        _operationCard = BuildOperationFeedbackCard()
        _root.Controls.Add(_operationCard)
        _root.SetColumnSpan(_operationCard, 2)

        Controls.Add(_root)

        _operationTimer = New System.Windows.Forms.Timer With {.Interval = 80}
        AddHandler _operationTimer.Tick, AddressOf OnOperationTimerTick

        AddHandler ThemeManager.ThemeChanged, AddressOf OnThemeChanged
        AddHandler Disposed, AddressOf OnSelfDisposed
    End Sub

    Private Sub OnSelfDisposed(sender As Object, e As EventArgs)
        _operationTimer.Stop()
        _operationTimer.Dispose()
        RemoveHandler ThemeManager.ThemeChanged, AddressOf OnThemeChanged
    End Sub

    Private Sub OnThemeChanged(sender As Object, e As ThemeChangedEventArgs)
        If IsDisposed OrElse Disposing Then Return
        If InvokeRequired Then
            BeginInvoke(Sub() OnThemeChanged(sender, e))
            Return
        End If
        BackColor = CType(e.Current.Palette(NexaColorRole.Background).Value, Color)
    End Sub

    ' ---------- Cards ----------

    Private Function BuildProgressBarCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Interactive progress bar with increment/decrement/set controls."))

        Dim bar As New NexaProgressBar With {
            .Minimum = 0,
            .Maximum = 100,
            .Value = 35,
            .Width = 320,
            .Dock = DockStyle.Top
        }
        Dim status As New NexaLabel With {
            .Text = "Current: 35%",
            .AutoSize = True,
            .Dock = DockStyle.Top,
            .LabelStyle = NexaLabelStyle.Caption
        }
        Dim row = New FlowLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .Margin = New Padding(0, 8, 0, 0)
        }
        Dim minus As New NexaButton With {.Text = "-10", .Width = 60}
        Dim plus As New NexaButton With {.Text = "+10", .Width = 60}
        Dim set50 As New NexaButton With {.Text = "Set 50", .Width = 70}
        Dim set100 As New NexaButton With {.Text = "Set 100", .Width = 70}
        AddHandler minus.Click, Sub() bar.Value = Math.Max(0, bar.Value - 10)
        AddHandler plus.Click, Sub() bar.Value = Math.Min(100, bar.Value + 10)
        AddHandler set50.Click, Sub() bar.Value = 50
        AddHandler set100.Click, Sub() bar.Value = 100
        AddHandler bar.ValueChanged, Sub() status.Text = $"Current: {bar.Value}%"
        row.Controls.AddRange(New Control() {minus, plus, set50, set100})
        card.Controls.Add(bar)
        card.Controls.Add(status)
        card.Controls.Add(row)
        Return card
    End Function

    Private Function BuildProgressBarStylesCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Default, Success, Warning, Danger, Info. Value = 60%."))

        Dim grid = New TableLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .ColumnCount = 2,
            .RowCount = 5
        }
        For i = 0 To 1
            grid.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50.0F))
        Next
        For i = 0 To 4
            grid.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        Next

        Dim labels = {"Default", "Success", "Warning", "Danger", "Info"}
        Dim styleValues = {NexaProgressStyle.Default, NexaProgressStyle.Success, NexaProgressStyle.Warning, NexaProgressStyle.Danger, NexaProgressStyle.Info}
        For i = 0 To labels.Length - 1
            grid.Controls.Add(New NexaLabel With {
                .Text = labels(i),
                .AutoSize = True,
                .Margin = New Padding(0, 8, 8, 4)
            }, 0, i)
            grid.Controls.Add(New NexaProgressBar With {
                .Value = 60,
                .ProgressStyle = styleValues(i),
                .Width = 220,
                .Margin = New Padding(0, 4, 0, 4)
            }, 1, i)
        Next
        card.Controls.Add(grid)
        Return card
    End Function

    Private Function BuildCircularProgressCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Various sizes, styles, and center text. Indeterminate mode available."))

        Dim row = New FlowLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .FlowDirection = FlowDirection.LeftToRight,
            .WrapContents = True
        }
        row.Controls.Add(MakeCircular(25, NexaProgressStyle.Default, 56, 4, Nothing))
        row.Controls.Add(MakeCircular(50, NexaProgressStyle.Success, 64, 5, Nothing))
        row.Controls.Add(MakeCircular(75, NexaProgressStyle.Warning, 72, 6, Nothing))
        row.Controls.Add(MakeCircular(100, NexaProgressStyle.Danger, 80, 6, Nothing))
        row.Controls.Add(MakeCircular(0, NexaProgressStyle.Info, 88, 7, "Loading"))
        card.Controls.Add(row)
        Return card
    End Function

    Private Shared Function MakeCircular(value As Integer, style As NexaProgressStyle, size As Integer, thickness As Integer, centerText As String) As NexaCircularProgress
        Dim c As New NexaCircularProgress With {
            .Value = value,
            .ProgressStyle = style,
            .Size = New Size(size, size),
            .LineThickness = thickness,
            .Margin = New Padding(0, 0, 12, 0)
        }
        If centerText IsNot Nothing Then c.CenterText = centerText
        Return c
    End Function

    Private Function BuildSpinnerCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Animated loading indicators. Ring (default) and Dots styles."))

        Dim row = New FlowLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .FlowDirection = FlowDirection.LeftToRight,
            .WrapContents = True
        }
        row.Controls.Add(New NexaSpinner With {.SpinnerSize = 32, .AnimationSpeed = 80, .SpinnerStyle = NexaSpinnerStyle.Ring, .Margin = New Padding(0, 0, 16, 0)})
        row.Controls.Add(New NexaSpinner With {.SpinnerSize = 40, .AnimationSpeed = 30, .SpinnerStyle = NexaSpinnerStyle.Ring, .Margin = New Padding(0, 0, 16, 0)})
        row.Controls.Add(New NexaSpinner With {.SpinnerSize = 32, .AnimationSpeed = 200, .SpinnerStyle = NexaSpinnerStyle.Ring, .Margin = New Padding(0, 0, 16, 0)})
        row.Controls.Add(New NexaSpinner With {.SpinnerSize = 48, .AnimationSpeed = 100, .SpinnerStyle = NexaSpinnerStyle.Dots, .Margin = New Padding(0, 0, 16, 0)})
        row.Controls.Add(New NexaSpinner With {.SpinnerSize = 32, .AnimationEnabled = False, .Margin = New Padding(0, 0, 16, 0)})

        Dim labels = New FlowLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .FlowDirection = FlowDirection.LeftToRight,
            .WrapContents = True
        }
        labels.Controls.Add(New NexaLabel With {.Text = "Ring (80ms)", .AutoSize = True, .Width = 120, .LabelStyle = NexaLabelStyle.Caption})
        labels.Controls.Add(New NexaLabel With {.Text = "Ring (30ms fast)", .AutoSize = True, .Width = 120, .LabelStyle = NexaLabelStyle.Caption})
        labels.Controls.Add(New NexaLabel With {.Text = "Ring (200ms slow)", .AutoSize = True, .Width = 140, .LabelStyle = NexaLabelStyle.Caption})
        labels.Controls.Add(New NexaLabel With {.Text = "Dots", .AutoSize = True, .Width = 80, .LabelStyle = NexaLabelStyle.Caption})
        labels.Controls.Add(New NexaLabel With {.Text = "Paused", .AutoSize = True, .Width = 80, .LabelStyle = NexaLabelStyle.Caption})

        card.Controls.Add(row)
        card.Controls.Add(labels)
        Return card
    End Function

    Private Function BuildBadgeCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Compact status/count indicators. Next to labels, buttons, and cards."))

        Dim grid = New TableLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .ColumnCount = 2,
            .RowCount = 6
        }
        grid.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 40.0F))
        grid.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 60.0F))
        For i = 0 To 5
            grid.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        Next

        AddBadgeRow(grid, 0, "Default", New NexaBadge With {.Text = "NEW", .BadgeStyle = NexaBadgeStyle.Default, .BadgeSize = NexaBadgeSize.Medium})
        AddBadgeRow(grid, 1, "Primary", New NexaBadge With {.Text = "5", .BadgeStyle = NexaBadgeStyle.Primary, .BadgeSize = NexaBadgeSize.Medium})
        AddBadgeRow(grid, 2, "Success", New NexaBadge With {.Text = "ACTIVE", .BadgeStyle = NexaBadgeStyle.Success, .BadgeSize = NexaBadgeSize.Medium})
        AddBadgeRow(grid, 3, "Warning", New NexaBadge With {.Text = "BETA", .BadgeStyle = NexaBadgeStyle.Warning, .BadgeSize = NexaBadgeSize.Medium})
        AddBadgeRow(grid, 4, "Danger (99+)", New NexaBadge With {.Text = "1234", .BadgeStyle = NexaBadgeStyle.Danger, .BadgeSize = NexaBadgeSize.Medium, .MaximumCharacters = 2})
        AddBadgeRow(grid, 5, "Muted", New NexaBadge With {.Text = "v1.0", .BadgeStyle = NexaBadgeStyle.Muted, .BadgeSize = NexaBadgeSize.Small})

        card.Controls.Add(grid)
        Return card
    End Function

    Private Shared Sub AddBadgeRow(grid As TableLayoutPanel, row As Integer, label As String, badge As NexaBadge)
        grid.Controls.Add(New NexaLabel With {
            .Text = label,
            .AutoSize = True,
            .Margin = New Padding(0, 8, 8, 4)
        }, 0, row)
        grid.Controls.Add(badge, 1, row)
    End Sub

    Private Function BuildAlertCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Inline notification panels with icon, title, message, and close button."))

        _infoAlert = New NexaAlert With {
            .AlertStyle = NexaAlertStyle.Information,
            .Title = "Information",
            .Message = "Your account settings have been updated.",
            .Dock = DockStyle.Top,
            .Margin = New Padding(0, 0, 0, 8)
        }
        _successAlert = New NexaAlert With {
            .AlertStyle = NexaAlertStyle.Success,
            .Title = "Success",
            .Message = "The operation completed successfully.",
            .Dock = DockStyle.Top,
            .Margin = New Padding(0, 0, 0, 8)
        }
        _warningAlert = New NexaAlert With {
            .AlertStyle = NexaAlertStyle.Warning,
            .Title = "Warning",
            .Message = "This action may affect existing data.",
            .Dock = DockStyle.Top,
            .Margin = New Padding(0, 0, 0, 8)
        }
        _errorAlert = New NexaAlert With {
            .AlertStyle = NexaAlertStyle.Error,
            .Title = "Error",
            .Message = "The requested operation could not be completed.",
            .Dock = DockStyle.Top,
            .Margin = New Padding(0, 0, 0, 8)
        }
        _autoCloseAlert = New NexaAlert With {
            .AlertStyle = NexaAlertStyle.Information,
            .Title = "Auto-close",
            .Message = "This alert auto-closes after 3 seconds. Click to re-trigger.",
            .Dock = DockStyle.Top,
            .AutoClose = True,
            .AutoCloseDelayMs = 3000,
            .Margin = New Padding(0, 0, 0, 8),
            .Cursor = Cursors.Hand
        }
        AddHandler _autoCloseAlert.Click, Sub()
                                              _autoCloseAlert.Visible = True
                                              _autoCloseAlert.AutoClose = True
                                          End Sub

        card.Controls.Add(_infoAlert)
        card.Controls.Add(_successAlert)
        card.Controls.Add(_warningAlert)
        card.Controls.Add(_errorAlert)
        card.Controls.Add(_autoCloseAlert)
        Return card
    End Function

    Private Function BuildStatusIndicatorCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Compact status indicators with semantic theme colors."))

        Dim grid = New TableLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .ColumnCount = 2,
            .RowCount = 7
        }
        grid.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 40.0F))
        grid.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 60.0F))
        For i = 0 To 6
            grid.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        Next

        AddStatusRow(grid, 0, "Online", New NexaStatusIndicator With {.Status = NexaStatus.Online, .Text = "System online"})
        AddStatusRow(grid, 1, "Offline", New NexaStatusIndicator With {.Status = NexaStatus.Offline, .Text = "Disconnected"})
        AddStatusRow(grid, 2, "Busy", New NexaStatusIndicator With {.Status = NexaStatus.Busy, .Text = "Processing..."})
        AddStatusRow(grid, 3, "Warning", New NexaStatusIndicator With {.Status = NexaStatus.Warning, .Text = "Low disk space"})
        AddStatusRow(grid, 4, "Success", New NexaStatusIndicator With {.Status = NexaStatus.Success, .Text = "Backup complete"})
        AddStatusRow(grid, 5, "Error", New NexaStatusIndicator With {.Status = NexaStatus.Error, .Text = "Connection failed"})
        AddStatusRow(grid, 6, "No text (dot only)", New NexaStatusIndicator With {.Status = NexaStatus.Online, .ShowText = False, .Text = "(hidden)"})

        card.Controls.Add(grid)
        Return card
    End Function

    Private Shared Sub AddStatusRow(grid As TableLayoutPanel, row As Integer, label As String, indicator As NexaStatusIndicator)
        grid.Controls.Add(New NexaLabel With {
            .Text = label,
            .AutoSize = True,
            .Margin = New Padding(0, 8, 8, 4)
        }, 0, row)
        grid.Controls.Add(indicator, 1, row)
    End Sub

    ' ---------- Operation Feedback ----------

    Private Function BuildOperationFeedbackCard() As NexaCard
        Dim card As New NexaCard With {
            .Title = "File Backup",
            .Subtitle = "Simulated operation using NexaUI feedback controls",
            .Dock = DockStyle.Top,
            .CornerRadius = 4,
            .ShadowEnabled = True,
            .ShadowDepth = 4,
            .Margin = New Padding(0, 8, 0, 0)
        }

        _operationTitle = New NexaLabel With {
            .Text = "Ready to start backup.",
            .LabelStyle = NexaLabelStyle.Default,
            .AutoSize = True,
            .Dock = DockStyle.Top
        }
        _operationDetail = New NexaLabel With {
            .Text = "Use the buttons below to start, pause, complete, fail, or reset.",
            .LabelStyle = NexaLabelStyle.Muted,
            .AutoSize = True,
            .Dock = DockStyle.Top,
            .Margin = New Padding(0, 2, 0, 8)
        }
        _operationProgress = New NexaProgressBar With {
            .Minimum = 0,
            .Maximum = 100,
            .Value = 0,
            .Width = 360,
            .BarHeight = 12,
            .Dock = DockStyle.Top,
            .ProgressStyle = NexaProgressStyle.Info
        }

        Dim statusRow = New FlowLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .FlowDirection = FlowDirection.LeftToRight,
            .WrapContents = False,
            .Margin = New Padding(0, 8, 0, 8)
        }
        _operationStatus = New NexaStatusIndicator With {
            .Status = NexaStatus.None,
            .Text = "Idle",
            .Margin = New Padding(0, 0, 12, 0)
        }
        _operationSpinner = New NexaSpinner With {
            .SpinnerSize = 22,
            .AnimationEnabled = False,
            .Margin = New Padding(0, 0, 0, 0)
        }
        statusRow.Controls.Add(_operationStatus)
        statusRow.Controls.Add(_operationSpinner)

        Dim buttons = New FlowLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .FlowDirection = FlowDirection.LeftToRight,
            .WrapContents = False
        }
        Dim btnStart As New NexaButton With {.Text = "Start", .Style = NexaButtonStyle.Primary, .Width = 80, .Margin = New Padding(0, 0, 8, 0)}
        Dim btnPause As New NexaButton With {.Text = "Pause", .Style = NexaButtonStyle.Secondary, .Width = 80, .Margin = New Padding(0, 0, 8, 0)}
        Dim btnComplete As New NexaButton With {.Text = "Complete", .Style = NexaButtonStyle.Success, .Width = 90, .Margin = New Padding(0, 0, 8, 0)}
        Dim btnFail As New NexaButton With {.Text = "Fail", .Style = NexaButtonStyle.Danger, .Width = 80, .Margin = New Padding(0, 0, 8, 0)}
        Dim btnReset As New NexaButton With {.Text = "Reset", .Style = NexaButtonStyle.Outline, .Width = 80}
        AddHandler btnStart.Click, AddressOf StartOperation
        AddHandler btnPause.Click, AddressOf PauseOperation
        AddHandler btnComplete.Click, AddressOf CompleteOperation
        AddHandler btnFail.Click, AddressOf FailOperation
        AddHandler btnReset.Click, AddressOf ResetOperation
        buttons.Controls.AddRange(New Control() {btnStart, btnPause, btnComplete, btnFail, btnReset})

        Dim alert As New NexaAlert With {
            .AlertStyle = NexaAlertStyle.Information,
            .Title = "Backup status",
            .Message = "Start the operation to see feedback controls working together.",
            .Dock = DockStyle.Top,
            .Margin = New Padding(0, 12, 0, 0)
        }
        alert.Name = "operationAlert"
        card.ContentPanel.Controls.Add(_operationTitle)
        card.ContentPanel.Controls.Add(_operationDetail)
        card.ContentPanel.Controls.Add(_operationProgress)
        card.ContentPanel.Controls.Add(statusRow)
        card.ContentPanel.Controls.Add(buttons)
        card.ContentPanel.Controls.Add(alert)
        Return card
    End Function

    Private Sub StartOperation()
        _operationRunning = True
        _operationPaused = False
        _operationSpinner.AnimationEnabled = True
        _operationStatus.Status = NexaStatus.Busy
        _operationStatus.Text = "Processing"
        _operationTitle.Text = "Backing up files..."
        _operationDetail.Text = "Please wait while files are being processed."
        _operationProgress.ProgressStyle = NexaProgressStyle.Info
        _operationTimer.Start()
        UpdateOperationAlert(NexaAlertStyle.Information, "Backup in progress", "Your files are being backed up. You can pause or cancel.")
    End Sub

    Private Sub PauseOperation()
        If Not _operationRunning Then Return
        _operationPaused = True
        _operationTimer.Stop()
        _operationSpinner.AnimationEnabled = False
        _operationStatus.Status = NexaStatus.Warning
        _operationStatus.Text = "Paused"
        _operationTitle.Text = "Backup paused."
        _operationDetail.Text = "Press Start to resume."
        UpdateOperationAlert(NexaAlertStyle.Warning, "Backup paused", "The backup has been paused. Click Start to resume.")
    End Sub

    Private Sub CompleteOperation()
        _operationTimer.Stop()
        _operationRunning = False
        _operationPaused = False
        _operationProgress.Value = 100
        _operationProgress.ProgressStyle = NexaProgressStyle.Success
        _operationSpinner.AnimationEnabled = False
        _operationStatus.Status = NexaStatus.Success
        _operationStatus.Text = "Completed"
        _operationTitle.Text = "Backup completed successfully."
        _operationDetail.Text = $"Processed at {DateTime.Now:HH:mm:ss}."
        UpdateOperationAlert(NexaAlertStyle.Success, "Backup complete", "All files were backed up successfully.")
    End Sub

    Private Sub FailOperation()
        _operationTimer.Stop()
        _operationRunning = False
        _operationPaused = False
        _operationProgress.ProgressStyle = NexaProgressStyle.Danger
        _operationSpinner.AnimationEnabled = False
        _operationStatus.Status = NexaStatus.Error
        _operationStatus.Text = "Failed"
        _operationTitle.Text = "Backup failed."
        _operationDetail.Text = "An error occurred while processing files."
        UpdateOperationAlert(NexaAlertStyle.Error, "Backup failed", "We could not complete the backup. Please try again.")
    End Sub

    Private Sub ResetOperation()
        _operationTimer.Stop()
        _operationRunning = False
        _operationPaused = False
        _operationProgress.Value = 0
        _operationProgress.ProgressStyle = NexaProgressStyle.Info
        _operationSpinner.AnimationEnabled = False
        _operationStatus.Status = NexaStatus.None
        _operationStatus.Text = "Idle"
        _operationTitle.Text = "Ready to start backup."
        _operationDetail.Text = "Use the buttons below to start, pause, complete, fail, or reset."
        UpdateOperationAlert(NexaAlertStyle.Information, "Backup status", "Start the operation to see feedback controls working together.")
    End Sub

    Private Sub OnOperationTimerTick(sender As Object, e As EventArgs)
        If _operationPaused Then Return
        If _operationProgress.Value >= 100 Then
            CompleteOperation()
            Return
        End If
        _operationProgress.Value = Math.Min(100, _operationProgress.Value + 2)
    End Sub

    Private Sub UpdateOperationAlert(style As NexaAlertStyle, title As String, message As String)
        For Each c As Control In _operationCard.ContentPanel.Controls
            If TypeOf c Is NexaAlert Then
                Dim alert = DirectCast(c, NexaAlert)
                If alert.Name = "operationAlert" Then
                    alert.AlertStyle = style
                    alert.Title = title
                    alert.Message = message
                    Return
                End If
            End If
        Next
    End Sub

    ' ---------- helpers ----------

    Private Shared Function SectionTitle(title As String) As NexaLabel
        Return New NexaLabel With {
            .Text = title,
            .AutoSize = True,
            .LabelStyle = NexaLabelStyle.Subheading,
            .Margin = New Padding(0, 8, 0, 8)
        }
    End Function

    Private Shared Function CreateDemoCard() As Panel
        Dim palette = ThemeManager.Current.Palette
        Dim card As New Panel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .Padding = New Padding(16),
            .Margin = New Padding(0, 0, 0, 12),
            .BackColor = CType(palette(NexaColorRole.Surface).Value, Color)
        }
        AddHandler card.Paint, Sub(s, e)
                                    Using pen = New Pen(CType(palette(NexaColorRole.Border).Value, Color), 1.0F)
                                        Dim r = card.ClientRectangle
                                        r.Width -= 1
                                        r.Height -= 1
                                        e.Graphics.DrawRectangle(pen, r)
                                    End Using
                                End Sub
        Return card
    End Function

    Private Shared Function DescriptionLabel(text As String) As NexaLabel
        Return New NexaLabel With {
            .Text = text,
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .LabelStyle = NexaLabelStyle.Muted,
            .Margin = New Padding(0, 0, 0, 8)
        }
    End Function
End Class
