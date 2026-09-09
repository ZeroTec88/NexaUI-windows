Imports System.Drawing
Imports System.Windows.Forms
Imports System.Threading.Tasks
Imports NexaUI.Controls
Imports NexaUI.Core
Imports NexaUI.Icons
Imports NexaUI.Themes

Public NotInheritable Class GalleryDialogsScreen
    Inherits UserControl

    Private ReadOnly _root As TableLayoutPanel
    Private _toolTip As NexaToolTip = Nothing
    Private _toastManager As NexaToastManager = Nothing
    Private _modalBackground As NexaModalBackground = Nothing

    Public Sub New()
        Dock = DockStyle.Fill
        AutoScroll = True
        BackColor = CType(ThemeManager.Current.Palette(NexaColorRole.Background).Value, Color)

        ' Initialize shared components FIRST, before building UI
        InitializeSharedComponents()

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
            .Text = "Dialogs & Overlays",
            .LabelStyle = NexaLabelStyle.Heading,
            .AutoSize = True,
            .Margin = New Padding(0, 0, 0, 8)
        }
        _root.Controls.Add(heading)
        _root.SetColumnSpan(heading, 2)

        Dim intro = New NexaLabel With {
            .Text = "NexaMessageBox, NexaInputDialog, NexaToast, NexaToolTip, NexaPopover, NexaLoadingOverlay, NexaModalBackground. Complete dialog and overlay system.",
            .LabelStyle = NexaLabelStyle.Muted,
            .AutoSize = True,
            .Margin = New Padding(0, 0, 0, 16)
        }
        _root.Controls.Add(intro)
        _root.SetColumnSpan(intro, 2)

        ' MessageBox
        _root.Controls.Add(SectionTitle("NexaMessageBox — Standard Dialogs"))
        _root.Controls.Add(BuildMessageBoxCard())

        _root.Controls.Add(SectionTitle("NexaMessageBox — Confirmation Dialogs"))
        _root.Controls.Add(BuildConfirmationCard())

        ' InputDialog
        _root.Controls.Add(SectionTitle("NexaInputDialog — Text Input"))
        _root.Controls.Add(BuildInputDialogCard())

        ' Toast
        _root.Controls.Add(SectionTitle("NexaToast — Toast Notifications"))
        _root.Controls.Add(BuildToastCard())

        ' ToolTip
        _root.Controls.Add(SectionTitle("NexaToolTip — Hover Tooltips"))
        _root.Controls.Add(BuildToolTipCard())

        ' Popover
        _root.Controls.Add(SectionTitle("NexaPopover — Rich Popovers"))
        _root.Controls.Add(BuildPopoverCard())

        ' LoadingOverlay
        _root.Controls.Add(SectionTitle("NexaLoadingOverlay — Loading States"))
        _root.Controls.Add(BuildLoadingOverlayCard())

        ' ModalBackground
        _root.Controls.Add(SectionTitle("NexaModalBackground — Modal Overlays"))
        _root.Controls.Add(BuildModalBackgroundCard())

        ' Application Modal Example
        _root.Controls.Add(SectionTitle("Real-World: Application Modal Workflow"))
        Dim appModalCard = BuildApplicationModalCard()
        _root.Controls.Add(appModalCard)
        _root.SetColumnSpan(appModalCard, 2)

        Controls.Add(_root)

        AddHandler ThemeManager.ThemeChanged, AddressOf OnThemeChanged
        AddHandler Disposed, AddressOf OnSelfDisposed
    End Sub

    Private Sub OnSelfDisposed(sender As Object, e As EventArgs)
        _toolTip?.Dispose()
        _toastManager?.Dispose()
        _modalBackground?.Dispose()
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

    Private Sub InitializeSharedComponents()
        _toolTip = New NexaToolTip With {
            .InitialDelay = 300,
            .AutoPopDelay = 5000,
            .Position = NexaTooltipPosition.Top
        }
        _toastManager = New NexaToastManager(Me)
    End Sub

    ' ---------- MessageBox Cards ----------

    Private Function BuildMessageBoxCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Standard message boxes with icons, custom buttons, and theme-aware styling."))

        Dim grid = New TableLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .ColumnCount = 2,
            .RowCount = 4
        }
        For i = 0 To 1
            grid.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50.0F))
        Next
        For i = 0 To 3
            grid.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        Next

        Dim infoBtn = New NexaButton With {.Text = "Show Information", .Width = 180, .Style = NexaButtonStyle.Primary}
        AddHandler infoBtn.Click, Sub() NexaMessageBox.ShowInformation(Me, "This is an information message.", "Information")
        grid.Controls.Add(infoBtn, 0, 0)
        grid.Controls.Add(New NexaLabel With {.Text = "NexaMessageBox.ShowInformation()", .LabelStyle = NexaLabelStyle.Caption, .AutoSize = True, .Margin = New Padding(8, 8, 0, 0)}, 1, 0)

        Dim successBtn = New NexaButton With {.Text = "Show Success", .Width = 180, .Style = NexaButtonStyle.Success}
        AddHandler successBtn.Click, Sub() NexaMessageBox.ShowSuccess(Me, "Operation completed successfully!", "Success")
        grid.Controls.Add(successBtn, 0, 1)
        grid.Controls.Add(New NexaLabel With {.Text = "NexaMessageBox.ShowSuccess()", .LabelStyle = NexaLabelStyle.Caption, .AutoSize = True, .Margin = New Padding(8, 8, 0, 0)}, 1, 1)

        Dim warningBtn = New NexaButton With {.Text = "Show Warning", .Width = 180, .Style = NexaButtonStyle.Warning}
        AddHandler warningBtn.Click, Sub() NexaMessageBox.ShowWarning(Me, "This action may affect existing data.", "Warning")
        grid.Controls.Add(warningBtn, 0, 2)
        grid.Controls.Add(New NexaLabel With {.Text = "NexaMessageBox.ShowWarning()", .LabelStyle = NexaLabelStyle.Caption, .AutoSize = True, .Margin = New Padding(8, 8, 0, 0)}, 1, 2)

        Dim errorBtn = New NexaButton With {.Text = "Show Error", .Width = 180, .Style = NexaButtonStyle.Danger}
        AddHandler errorBtn.Click, Sub() NexaMessageBox.ShowError(Me, "The requested operation could not be completed.", "Error")
        grid.Controls.Add(errorBtn, 0, 3)
        grid.Controls.Add(New NexaLabel With {.Text = "NexaMessageBox.ShowError()", .LabelStyle = NexaLabelStyle.Caption, .AutoSize = True, .Margin = New Padding(8, 8, 0, 0)}, 1, 3)

        card.Controls.Add(grid)
        Return card
    End Function

    Private Function BuildConfirmationCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Confirmation dialogs with Yes/No, Yes/No/Cancel, and custom button layouts."))

        Dim grid = New TableLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .ColumnCount = 2,
            .RowCount = 3
        }
        For i = 0 To 1
            grid.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50.0F))
        Next
        For i = 0 To 2
            grid.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        Next

        Dim questionBtn = New NexaButton With {.Text = "Show Question (Yes/No)", .Width = 200, .Style = NexaButtonStyle.Primary}
        AddHandler questionBtn.Click, Sub()
                                          Dim result = NexaMessageBox.ShowQuestion(Me, "Are you sure you want to continue?", "Confirm Action")
                                          ShowResult(result)
                                      End Sub
        grid.Controls.Add(questionBtn, 0, 0)
        grid.Controls.Add(New NexaLabel With {.Text = "NexaMessageBox.ShowQuestion()", .LabelStyle = NexaLabelStyle.Caption, .AutoSize = True, .Margin = New Padding(8, 8, 0, 0)}, 1, 0)

        Dim confirmBtn = New NexaButton With {.Text = "Show Confirm (Yes/No/Cancel)", .Width = 200, .Style = NexaButtonStyle.Secondary}
        AddHandler confirmBtn.Click, Sub()
                                         Dim result = NexaMessageBox.ShowConfirm(Me, "Save changes before closing?", "Unsaved Changes")
                                         ShowResult(result)
                                     End Sub
        grid.Controls.Add(confirmBtn, 0, 1)
        grid.Controls.Add(New NexaLabel With {.Text = "NexaMessageBox.ShowConfirm()", .LabelStyle = NexaLabelStyle.Caption, .AutoSize = True, .Margin = New Padding(8, 8, 0, 0)}, 1, 1)

        Dim customBtn = New NexaButton With {.Text = "Custom Buttons", .Width = 200, .Style = NexaButtonStyle.Outline}
        AddHandler customBtn.Click, Sub()
                                        Dim result = NexaMessageBox.Show(
                                            Me,
                                            "Choose an action for the selected items.",
                                            "Bulk Actions",
                                            NexaDialogStyle.Standard,
                                            {("Apply All", NexaDialogResult.Yes, NexaButtonStyle.Primary, True),
                                             ("Apply Selected", NexaDialogResult.OK, NexaButtonStyle.Secondary, False),
                                             ("Skip", NexaDialogResult.No, NexaButtonStyle.Ghost, False),
                                             ("Cancel", NexaDialogResult.Cancel, NexaButtonStyle.Ghost, False)}
                                        )
                                        ShowResult(result)
                                    End Sub
        grid.Controls.Add(customBtn, 0, 2)
        grid.Controls.Add(New NexaLabel With {.Text = "NexaMessageBox.Show() with custom buttons", .LabelStyle = NexaLabelStyle.Caption, .AutoSize = True, .Margin = New Padding(8, 8, 0, 0)}, 1, 2)

        card.Controls.Add(grid)
        Return card
    End Function

    Private Sub ShowResult(result As NexaDialogResult)
        Dim style = If(result = NexaDialogResult.OK OrElse result = NexaDialogResult.Yes, NexaAlertStyle.Success,
                   If(result = NexaDialogResult.No OrElse result = NexaDialogResult.Cancel, NexaAlertStyle.Warning, NexaAlertStyle.Information))
        _toastManager.Show($"Dialog result: {result}", "Dialog Result", style)
    End Sub

    ' ---------- InputDialog Card ----------

    Private Function BuildInputDialogCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Input dialogs for single-line and multi-line text entry with validation."))

        Dim grid = New TableLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .ColumnCount = 2,
            .RowCount = 3
        }
        For i = 0 To 1
            grid.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50.0F))
        Next
        For i = 0 To 2
            grid.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        Next

Dim singleBtn = New NexaButton With {.Text = "Single-line Input", .Width = 200, .Style = NexaButtonStyle.Primary}
        AddHandler singleBtn.Click, Async Sub()
            Dim resultTuple = Await Task.Run(Function() NexaInputDialog.ShowInput(Me, "Enter your name:", "Enter Name", "John Doe"))
            Dim result = resultTuple.Result
            Dim text = resultTuple.Text
            If result = NexaDialogResult.OK Then
                _toastManager.Show($"Entered: {text}", "Input Received", NexaToastStyle.Success)
            End If
        End Sub
        grid.Controls.Add(singleBtn, 0, 0)
        grid.Controls.Add(New NexaLabel With {.Text = "NexaInputDialog.ShowInput()", .LabelStyle = NexaLabelStyle.Caption, .AutoSize = True, .Margin = New Padding(8, 8, 0, 0)}, 1, 0)

        Dim multiBtn = New NexaButton With {.Text = "Multi-line Input", .Width = 200, .Style = NexaButtonStyle.Secondary}
        AddHandler multiBtn.Click, Async Sub()
            Dim resultTuple = Await Task.Run(Function() NexaInputDialog.ShowMultilineInput(Me, "Enter your feedback:", "Feedback", "Type here..."))
            Dim result = resultTuple.Result
            Dim text = resultTuple.Text
            If result = NexaDialogResult.OK Then
                _toastManager.Show($"Feedback received ({text.Length} chars)", "Input Received", NexaToastStyle.Success)
            End If
        End Sub
        grid.Controls.Add(multiBtn, 0, 1)
        grid.Controls.Add(New NexaLabel With {.Text = "NexaInputDialog.ShowMultilineInput()", .LabelStyle = NexaLabelStyle.Caption, .AutoSize = True, .Margin = New Padding(8, 8, 0, 0)}, 1, 1)

        Dim placeholderBtn = New NexaButton With {.Text = "With Placeholder", .Width = 200, .Style = NexaButtonStyle.Outline}
        AddHandler placeholderBtn.Click, Async Sub()
            Dim resultTuple = Await Task.Run(Function() NexaInputDialog.Show(Me, "Search for items...", "Search", "", "Type to search..."))
            Dim result = resultTuple.Result
            Dim text = resultTuple.Text
            If result = NexaDialogResult.OK Then
                _toastManager.Show($"Searching for: {text}", "Search", NexaToastStyle.Info)
            End If
        End Sub
        grid.Controls.Add(placeholderBtn, 0, 2)
        grid.Controls.Add(New NexaLabel With {.Text = "NexaInputDialog.Show() with placeholder", .LabelStyle = NexaLabelStyle.Caption, .AutoSize = True, .Margin = New Padding(8, 8, 0, 0)}, 1, 2)

        card.Controls.Add(grid)
        Return card
    End Function

    ' ---------- Toast Card ----------

    Private Function BuildToastCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Toast notifications with auto-dismiss, stacking, and multiple styles."))

        Dim grid = New TableLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .ColumnCount = 4,
            .RowCount = 2
        }
        For i = 0 To 3
            grid.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 25.0F))
        Next
        For i = 0 To 1
            grid.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        Next

        Dim toastTypes = {
            ("Info", NexaToastStyle.Info, "Information toast with blue accent"),
            ("Success", NexaToastStyle.Success, "Operation completed successfully"),
            ("Warning", NexaToastStyle.Warning, "Please review your changes"),
            ("Error", NexaToastStyle.Error, "Failed to save the document"),
            ("Info (Top-Left)", NexaToastStyle.Info, "Positioned at top-left"),
            ("Success (Bottom)", NexaToastStyle.Success, "Positioned at bottom-center"),
            ("Warning (Custom)", NexaToastStyle.Warning, "Custom auto-close delay: 10s"),
            ("Error (Persistent)", NexaToastStyle.Error, "No auto-close (0 = stay open)")
        }

        For i = 0 To toastTypes.Length - 1
            Dim label = toastTypes(i).Item1
            Dim style = toastTypes(i).Item2
            Dim text = toastTypes(i).Item3
            Dim btn = New NexaButton With {
                .Text = label,
                .Width = 140,
                .Height = 40,
                .Style = If(style = NexaToastStyle.Success, NexaButtonStyle.Success,
                       If(style = NexaToastStyle.Warning, NexaButtonStyle.Warning,
                       If(style = NexaToastStyle.Error, NexaButtonStyle.Danger, NexaButtonStyle.Primary)))
            }
            Dim s = style
            Dim t = text
            AddHandler btn.Click, Sub()
                                      Dim pos = If(label.Contains("Top-Left"), NexaToastPosition.TopLeft,
                                             If(label.Contains("Bottom"), NexaToastPosition.BottomCenter,
                                             NexaToastPosition.TopRight))
                                      Dim delay = If(label.Contains("10s"), 10000, If(label.Contains("Persistent"), 0, 5000))
                                      _toastManager.Show(t, label, s, pos, delay)
                                  End Sub
            grid.Controls.Add(btn, i Mod 4, i \ 4)
        Next

        card.Controls.Add(grid)
        Return card
    End Function

    ' ---------- ToolTip Card ----------

    Private Function BuildToolTipCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Hover over the buttons below to see tooltips with different positions."))

        Dim flow = New FlowLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .FlowDirection = FlowDirection.LeftToRight,
            .WrapContents = True
        }

        Dim positions = {
            ("Top", NexaTooltipPosition.Top),
            ("Bottom", NexaTooltipPosition.Bottom),
            ("Left", NexaTooltipPosition.Left),
            ("Right", NexaTooltipPosition.Right)
        }

        For Each pair In positions
            Dim label = pair.Item1
            Dim pos = pair.Item2
            Dim btn = New NexaButton With {
                .Text = $"Hover me ({label})",
                .Width = 160,
                .Height = 50,
                .Style = NexaButtonStyle.Secondary,
                .Margin = New Padding(8)
            }
            _toolTip.SetToolTip(btn, $"This tooltip appears on the {label.ToLower()} side of the control. It fades in and out smoothly.")
            flow.Controls.Add(btn)
        Next

        card.Controls.Add(flow)
        Return card
    End Function

    ' ---------- Popover Card ----------

    Private Function BuildPopoverCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Rich popovers with titles, content, and flexible positioning. Click buttons to open."))

        Dim flow = New FlowLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .FlowDirection = FlowDirection.LeftToRight,
            .WrapContents = True
        }

        Dim positions = {
            ("Bottom", NexaPopoverPosition.Bottom),
            ("Top", NexaPopoverPosition.Top),
            ("Left", NexaPopoverPosition.Left),
            ("Right", NexaPopoverPosition.Right),
            ("Bottom-Right", NexaPopoverPosition.BottomRight)
        }

        For Each pair In positions
            Dim label = pair.Item1
            Dim pos = pair.Item2
            Dim btn = New NexaButton With {
                .Text = $"Open ({label})",
                .Width = 130,
                .Style = NexaButtonStyle.Secondary,
                .Margin = New Padding(8)
            }
            Dim p = pos
            AddHandler btn.Click, Sub()
                                      Dim popover = New NexaPopover With {
                                          .Title = $"Popover ({label})",
                                          .Position = p,
                                          .Width = 280,
                                          .Height = 160,
                                          .ShowCloseButton = True
                                      }
                                      popover.ContentPanel.Controls.Add(New NexaLabel With {
                                          .Text = $"This popover is anchored to the {label.ToLower()} of the button. It supports rich content including forms, lists, and other controls.",
                                          .LabelStyle = NexaLabelStyle.Muted,
                                          .AutoSize = True,
                                          .Dock = DockStyle.Top,
                                          .Margin = New Padding(0, 0, 0, 12)
                                      })
                                      popover.ContentPanel.Controls.Add(New NexaButton With {
                                          .Text = "Action Button",
                                          .Style = NexaButtonStyle.Primary,
                                          .Dock = DockStyle.Bottom,
                                          .Margin = New Padding(0, 12, 0, 0)
                                      })
                                      popover.Show(btn, p)
                                  End Sub
            flow.Controls.Add(btn)
        Next

        card.Controls.Add(flow)
        Return card
    End Function

    ' ---------- LoadingOverlay Card ----------

    Private Function BuildLoadingOverlayCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Full-screen or container-scoped loading overlays with spinner, title, and message."))

        Dim overlay = New NexaLoadingOverlay With {
            .Title = "Processing Data",
            .Message = "Please wait while we fetch the data...",
            .OverlayStyle = NexaOverlayStyle.Standard,
            .Visible = False
        }

        Dim btnShow = New NexaButton With {.Text = "Show Loading (3 sec)", .Width = 200, .Style = NexaButtonStyle.Primary}
        AddHandler btnShow.Click, Async Sub()
                                      overlay.Show()
                                      Await Task.Delay(3000)
                                      overlay.Hide()
                                  End Sub

        Dim btnStyle = New NexaButton With {.Text = "Show Light Style", .Width = 200, .Style = NexaButtonStyle.Secondary, .Margin = New Padding(0, 8, 0, 0)}
        AddHandler btnStyle.Click, Async Sub()
                                      overlay.OverlayStyle = NexaOverlayStyle.Light
                                      overlay.Title = "Light Overlay"
                                      overlay.Message = "Subtle loading indicator"
                                      overlay.Show()
                                      Await Task.Delay(2000)
                                      overlay.Hide()
                                  End Sub

        Dim overlayContainer = New Panel With {
            .Dock = DockStyle.Top,
            .Height = 200,
            .BorderStyle = BorderStyle.FixedSingle,
            .Margin = New Padding(0, 8, 0, 0)
        }
        overlayContainer.Controls.Add(overlay)

        Dim flow = New FlowLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .FlowDirection = FlowDirection.TopDown,
            .WrapContents = False
        }
        flow.Controls.Add(btnShow)
        flow.Controls.Add(btnStyle)
        flow.Controls.Add(overlayContainer)

        card.Controls.Add(flow)
        Return card
    End Function

    ' ---------- ModalBackground Card ----------

    Private Function BuildModalBackgroundCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Modal background overlay with centered content. Click background to close (when enabled)."))

        Dim btnModal = New NexaButton With {.Text = "Open Modal Dialog", .Width = 200, .Style = NexaButtonStyle.Primary}
        AddHandler btnModal.Click, Sub()
                                       _modalBackground = New NexaModalBackground With {
                                           .OverlayStyle = NexaOverlayStyle.Standard,
                                           .ClickToClose = True,
                                           .Dock = DockStyle.Fill
                                       }

                                       Dim modalContent = New NexaCard With {
                                           .Title = "Confirm Action",
                                           .Subtitle = "This action cannot be undone",
                                           .Width = 400,
                                           .Height = 220,
                                           .CornerRadius = 8,
                                           .ShadowEnabled = True,
                                           .ShadowDepth = 8
                                       }
                                       modalContent.ContentPanel.Controls.Add(New NexaLabel With {
                                           .Text = "Are you sure you want to delete the selected items? This action cannot be undone.",
                                           .LabelStyle = NexaLabelStyle.Muted,
                                           .AutoSize = True,
                                           .Dock = DockStyle.Top,
                                           .Margin = New Padding(0, 0, 0, 24)
                                       })

                                       Dim btnRow = New FlowLayoutPanel With {
                                           .Dock = DockStyle.Bottom,
                                           .AutoSize = True,
                                           .AutoSizeMode = AutoSizeMode.GrowAndShrink,
                                           .FlowDirection = FlowDirection.RightToLeft
                                       }
                                       Dim btnCancel = New NexaButton With {.Text = "Cancel", .Style = NexaButtonStyle.Ghost, .Width = 90, .Margin = New Padding(0, 0, 8, 0)}
                                       Dim btnDelete = New NexaButton With {.Text = "Delete", .Style = NexaButtonStyle.Danger, .Width = 90}
                                       AddHandler btnCancel.Click, Sub() _modalBackground.CloseModal()
                                       AddHandler btnDelete.Click, Sub()
                                                                       _modalBackground.CloseModal()
                                                                       _toastManager.Show("Items deleted successfully", "Deleted", NexaToastStyle.Success)
                                                                   End Sub
                                       btnRow.Controls.AddRange(New Control() {btnDelete, btnCancel})
                                       modalContent.ContentPanel.Controls.Add(btnRow)

                                       Controls.Add(_modalBackground)
                                       _modalBackground.ShowWithContent(modalContent)
                                   End Sub

        Dim btnNoClose = New NexaButton With {.Text = "Modal (No Background Click)", .Width = 200, .Style = NexaButtonStyle.Secondary, .Margin = New Padding(0, 8, 0, 0)}
        AddHandler btnNoClose.Click, Sub()
                                         _modalBackground = New NexaModalBackground With {
                                             .OverlayStyle = NexaOverlayStyle.Standard,
                                             .ClickToClose = False,
                                             .Dock = DockStyle.Fill
                                         }

                                         Dim modalContent = New NexaCard With {
                                             .Title = "Important Notice",
                                             .Subtitle = "Please read carefully",
                                             .Width = 400,
                                             .Height = 200,
                                             .CornerRadius = 8,
                                             .ShadowEnabled = True,
                                             .ShadowDepth = 8
                                         }
                                         modalContent.ContentPanel.Controls.Add(New NexaLabel With {
                                             .Text = "This modal cannot be closed by clicking the background. Use the button below.",
                                             .LabelStyle = NexaLabelStyle.Muted,
                                             .AutoSize = True,
                                             .Dock = DockStyle.Top,
                                             .Margin = New Padding(0, 0, 0, 24)
                                         })

                                         Dim btnOk = New NexaButton With {.Text = "OK", .Style = NexaButtonStyle.Primary, .Width = 90, .Dock = DockStyle.Bottom}
                                         AddHandler btnOk.Click, Sub() _modalBackground.CloseModal()
                                         modalContent.ContentPanel.Controls.Add(btnOk)

                                         Controls.Add(_modalBackground)
                                         _modalBackground.ShowWithContent(modalContent)
                                     End Sub

        Dim flow = New FlowLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .FlowDirection = FlowDirection.TopDown,
            .WrapContents = False
        }
        flow.Controls.Add(btnModal)
        flow.Controls.Add(btnNoClose)
        card.Controls.Add(flow)
        Return card
    End Function

    ' ---------- Application Modal Example ----------

    Private Function BuildApplicationModalCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Complete application modal workflow demonstrating modal background, card, buttons, and toast integration."))

        Dim btnStart = New NexaButton With {.Text = "Start File Import", .Style = NexaButtonStyle.Primary, .Width = 180, .Height = 44}
        AddHandler btnStart.Click, Async Sub()
                                       ' Show loading overlay
                                       Dim overlay = New NexaLoadingOverlay With {
                                           .Title = "Importing Files",
                                           .Message = "Preparing import...",
                                           .OverlayStyle = NexaOverlayStyle.Standard,
                                           .Dock = DockStyle.Fill
                                       }
                                       Controls.Add(overlay)
                                       overlay.Show()

                                       Await Task.Delay(1500)

                                       overlay.Title = "Processing"
                                       overlay.Message = "Importing file 1 of 3..."
                                       Await Task.Delay(1000)

                                       overlay.Message = "Importing file 2 of 3..."
                                       Await Task.Delay(1000)

                                       overlay.Message = "Importing file 3 of 3..."
                                       Await Task.Delay(1000)

                                       overlay.Message = "Finalizing..."
                                       Await Task.Delay(500)

                                       overlay.Hide()
                                       Controls.Remove(overlay)
                                       overlay.Dispose()

                                       ' Show success modal
                                       _modalBackground = New NexaModalBackground With {
                                           .OverlayStyle = NexaOverlayStyle.Standard,
                                           .ClickToClose = True,
                                           .Dock = DockStyle.Fill
                                       }

                                       Dim modalContent = New NexaCard With {
                                           .Title = "Import Complete",
                                           .Subtitle = "All files processed successfully",
                                           .Width = 400,
                                           .Height = 220,
                                           .CornerRadius = 8,
                                           .ShadowEnabled = True,
                                           .ShadowDepth = 8
                                       }
                                       modalContent.ContentPanel.Controls.Add(New NexaLabel With {
                                           .Text = "All 3 files were imported successfully.",
                                           .LabelStyle = NexaLabelStyle.Muted,
                                           .AutoSize = True,
                                           .Dock = DockStyle.Top,
                                           .Margin = New Padding(0, 0, 0, 24)
                                       })

                                       Dim btnOk = New NexaButton With {.Text = "Close", .Style = NexaButtonStyle.Primary, .Width = 90, .Dock = DockStyle.Bottom}
                                       AddHandler btnOk.Click, Sub() _modalBackground.CloseModal()
                                       modalContent.ContentPanel.Controls.Add(btnOk)

                                       Controls.Add(_modalBackground)
                                       _modalBackground.ShowWithContent(modalContent)

                                       _toastManager.Show("Import completed successfully", "Import", NexaToastStyle.Success)
                                   End Sub

        card.Controls.Add(btnStart)
        Return card
    End Function

    ' ---------- Helpers ----------

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
        Dim card = New Panel With {
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