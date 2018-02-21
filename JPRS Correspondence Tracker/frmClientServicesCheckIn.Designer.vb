<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmClientServicesCheckIn
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmClientServicesCheckIn))
        Me.lblConnectIndicator = New System.Windows.Forms.Label()
        Me.btnCheckIn = New System.Windows.Forms.Button()
        Me.tmrWait = New System.Windows.Forms.Timer(Me.components)
        Me.txtTimeReceived = New System.Windows.Forms.MaskedTextBox()
        Me.lblTimePicker = New System.Windows.Forms.Label()
        Me.lblTotalItems = New System.Windows.Forms.Label()
        Me.txtTotalItems = New System.Windows.Forms.TextBox()
        Me.SuspendLayout()
        '
        'lblConnectIndicator
        '
        Me.lblConnectIndicator.ForeColor = System.Drawing.Color.Red
        Me.lblConnectIndicator.Location = New System.Drawing.Point(-1, 2)
        Me.lblConnectIndicator.Name = "lblConnectIndicator"
        Me.lblConnectIndicator.Size = New System.Drawing.Size(210, 16)
        Me.lblConnectIndicator.TabIndex = 10
        Me.lblConnectIndicator.Text = "Not Connected"
        '
        'btnCheckIn
        '
        Me.btnCheckIn.Location = New System.Drawing.Point(266, 37)
        Me.btnCheckIn.Name = "btnCheckIn"
        Me.btnCheckIn.Size = New System.Drawing.Size(75, 40)
        Me.btnCheckIn.TabIndex = 9
        Me.btnCheckIn.Text = "Check In"
        Me.btnCheckIn.UseVisualStyleBackColor = True
        '
        'tmrWait
        '
        Me.tmrWait.Interval = 1000
        '
        'txtTimeReceived
        '
        Me.txtTimeReceived.Location = New System.Drawing.Point(155, 32)
        Me.txtTimeReceived.Mask = "90:00 \A\M"
        Me.txtTimeReceived.Name = "txtTimeReceived"
        Me.txtTimeReceived.Size = New System.Drawing.Size(100, 20)
        Me.txtTimeReceived.TabIndex = 11
        Me.txtTimeReceived.ValidatingType = GetType(Date)
        '
        'lblTimePicker
        '
        Me.lblTimePicker.AutoSize = True
        Me.lblTimePicker.Location = New System.Drawing.Point(12, 35)
        Me.lblTimePicker.Name = "lblTimePicker"
        Me.lblTimePicker.Size = New System.Drawing.Size(126, 13)
        Me.lblTimePicker.TabIndex = 12
        Me.lblTimePicker.Text = "Time mail was picked up:"
        '
        'lblTotalItems
        '
        Me.lblTotalItems.AutoSize = True
        Me.lblTotalItems.Location = New System.Drawing.Point(12, 57)
        Me.lblTotalItems.Name = "lblTotalItems"
        Me.lblTotalItems.Size = New System.Drawing.Size(117, 13)
        Me.lblTotalItems.TabIndex = 13
        Me.lblTotalItems.Text = "Total items to check in:"
        '
        'txtTotalItems
        '
        Me.txtTotalItems.Location = New System.Drawing.Point(155, 57)
        Me.txtTotalItems.MaxLength = 4
        Me.txtTotalItems.Name = "txtTotalItems"
        Me.txtTotalItems.Size = New System.Drawing.Size(100, 20)
        Me.txtTotalItems.TabIndex = 14
        '
        'frmClientServicesCheckIn
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(353, 95)
        Me.Controls.Add(Me.txtTotalItems)
        Me.Controls.Add(Me.lblTotalItems)
        Me.Controls.Add(Me.lblTimePicker)
        Me.Controls.Add(Me.txtTimeReceived)
        Me.Controls.Add(Me.lblConnectIndicator)
        Me.Controls.Add(Me.btnCheckIn)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmClientServicesCheckIn"
        Me.Text = "Correspondence Tracker | JPRS 2017"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblConnectIndicator As Label
    Friend WithEvents btnCheckIn As Button
    Friend WithEvents tmrWait As Timer
    Friend WithEvents txtTimeReceived As MaskedTextBox
    Friend WithEvents lblTimePicker As Label
    Friend WithEvents lblTotalItems As Label
    Friend WithEvents txtTotalItems As TextBox
End Class
