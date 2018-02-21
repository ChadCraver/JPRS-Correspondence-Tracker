<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.radNewRecord = New System.Windows.Forms.RadioButton()
        Me.radCloseRecord = New System.Windows.Forms.RadioButton()
        Me.grpRecordControls = New System.Windows.Forms.GroupBox()
        Me.txtCorrespondenceNumber = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lblReceivedDate = New System.Windows.Forms.Label()
        Me.dateReceived = New System.Windows.Forms.DateTimePicker()
        Me.lblRecordID = New System.Windows.Forms.Label()
        Me.txtRecordID = New System.Windows.Forms.TextBox()
        Me.btnPunch = New System.Windows.Forms.Button()
        Me.grpEndRecordForm = New System.Windows.Forms.GroupBox()
        Me.cmbCorrespondent = New System.Windows.Forms.ComboBox()
        Me.lblCorrespondent = New System.Windows.Forms.Label()
        Me.btnSubmit = New System.Windows.Forms.Button()
        Me.lblComments = New System.Windows.Forms.Label()
        Me.txtComments = New System.Windows.Forms.TextBox()
        Me.chkDefect = New System.Windows.Forms.CheckBox()
        Me.lblType = New System.Windows.Forms.Label()
        Me.cmbType = New System.Windows.Forms.ComboBox()
        Me.lblConnectIndicator = New System.Windows.Forms.Label()
        Me.tmrWait = New System.Windows.Forms.Timer(Me.components)
        Me.tmrRecheckConnection = New System.Windows.Forms.Timer(Me.components)
        Me.ComboBox1 = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.grpRecordControls.SuspendLayout()
        Me.grpEndRecordForm.SuspendLayout()
        Me.SuspendLayout()
        '
        'radNewRecord
        '
        Me.radNewRecord.AutoSize = True
        Me.radNewRecord.Checked = True
        Me.radNewRecord.Location = New System.Drawing.Point(56, 19)
        Me.radNewRecord.Name = "radNewRecord"
        Me.radNewRecord.Size = New System.Drawing.Size(149, 17)
        Me.radNewRecord.TabIndex = 1
        Me.radNewRecord.TabStop = True
        Me.radNewRecord.Text = "Check In Correspondence"
        Me.radNewRecord.UseVisualStyleBackColor = True
        '
        'radCloseRecord
        '
        Me.radCloseRecord.AutoSize = True
        Me.radCloseRecord.Location = New System.Drawing.Point(56, 42)
        Me.radCloseRecord.Name = "radCloseRecord"
        Me.radCloseRecord.Size = New System.Drawing.Size(157, 17)
        Me.radCloseRecord.TabIndex = 2
        Me.radCloseRecord.Text = "Check Out Correspondence"
        Me.radCloseRecord.UseVisualStyleBackColor = True
        '
        'grpRecordControls
        '
        Me.grpRecordControls.Controls.Add(Me.txtCorrespondenceNumber)
        Me.grpRecordControls.Controls.Add(Me.Label1)
        Me.grpRecordControls.Controls.Add(Me.lblReceivedDate)
        Me.grpRecordControls.Controls.Add(Me.dateReceived)
        Me.grpRecordControls.Controls.Add(Me.lblRecordID)
        Me.grpRecordControls.Controls.Add(Me.txtRecordID)
        Me.grpRecordControls.Location = New System.Drawing.Point(12, 65)
        Me.grpRecordControls.Name = "grpRecordControls"
        Me.grpRecordControls.Size = New System.Drawing.Size(328, 126)
        Me.grpRecordControls.TabIndex = 3
        Me.grpRecordControls.TabStop = False
        '
        'txtCorrespondenceNumber
        '
        Me.txtCorrespondenceNumber.Location = New System.Drawing.Point(132, 47)
        Me.txtCorrespondenceNumber.MaxLength = 6
        Me.txtCorrespondenceNumber.Name = "txtCorrespondenceNumber"
        Me.txtCorrespondenceNumber.Size = New System.Drawing.Size(180, 20)
        Me.txtCorrespondenceNumber.TabIndex = 10
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(6, 47)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(98, 13)
        Me.Label1.TabIndex = 9
        Me.Label1.Text = "Correspondence #:"
        '
        'lblReceivedDate
        '
        Me.lblReceivedDate.AutoSize = True
        Me.lblReceivedDate.Location = New System.Drawing.Point(6, 19)
        Me.lblReceivedDate.Name = "lblReceivedDate"
        Me.lblReceivedDate.Size = New System.Drawing.Size(82, 13)
        Me.lblReceivedDate.TabIndex = 8
        Me.lblReceivedDate.Text = "Date Received:"
        '
        'dateReceived
        '
        Me.dateReceived.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dateReceived.Location = New System.Drawing.Point(132, 19)
        Me.dateReceived.Name = "dateReceived"
        Me.dateReceived.Size = New System.Drawing.Size(180, 20)
        Me.dateReceived.TabIndex = 7
        '
        'lblRecordID
        '
        Me.lblRecordID.AutoSize = True
        Me.lblRecordID.Location = New System.Drawing.Point(6, 84)
        Me.lblRecordID.Name = "lblRecordID"
        Me.lblRecordID.Size = New System.Drawing.Size(102, 13)
        Me.lblRecordID.TabIndex = 5
        Me.lblRecordID.Text = "Correspondence ID:"
        '
        'txtRecordID
        '
        Me.txtRecordID.BackColor = System.Drawing.Color.White
        Me.txtRecordID.Font = New System.Drawing.Font("Arial", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtRecordID.Location = New System.Drawing.Point(132, 73)
        Me.txtRecordID.MaxLength = 12
        Me.txtRecordID.Multiline = True
        Me.txtRecordID.Name = "txtRecordID"
        Me.txtRecordID.ReadOnly = True
        Me.txtRecordID.Size = New System.Drawing.Size(180, 39)
        Me.txtRecordID.TabIndex = 3
        Me.txtRecordID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.txtRecordID.WordWrap = False
        '
        'btnPunch
        '
        Me.btnPunch.Location = New System.Drawing.Point(233, 19)
        Me.btnPunch.Name = "btnPunch"
        Me.btnPunch.Size = New System.Drawing.Size(75, 40)
        Me.btnPunch.TabIndex = 4
        Me.btnPunch.Text = "Punch"
        Me.btnPunch.UseVisualStyleBackColor = True
        '
        'grpEndRecordForm
        '
        Me.grpEndRecordForm.Controls.Add(Me.cmbCorrespondent)
        Me.grpEndRecordForm.Controls.Add(Me.lblCorrespondent)
        Me.grpEndRecordForm.Controls.Add(Me.btnSubmit)
        Me.grpEndRecordForm.Controls.Add(Me.lblComments)
        Me.grpEndRecordForm.Controls.Add(Me.txtComments)
        Me.grpEndRecordForm.Controls.Add(Me.chkDefect)
        Me.grpEndRecordForm.Controls.Add(Me.Label2)
        Me.grpEndRecordForm.Controls.Add(Me.lblType)
        Me.grpEndRecordForm.Controls.Add(Me.ComboBox1)
        Me.grpEndRecordForm.Controls.Add(Me.cmbType)
        Me.grpEndRecordForm.Location = New System.Drawing.Point(12, 198)
        Me.grpEndRecordForm.Name = "grpEndRecordForm"
        Me.grpEndRecordForm.Size = New System.Drawing.Size(463, 292)
        Me.grpEndRecordForm.TabIndex = 4
        Me.grpEndRecordForm.TabStop = False
        Me.grpEndRecordForm.Text = "Record Description Form"
        '
        'cmbCorrespondent
        '
        Me.cmbCorrespondent.FormattingEnabled = True
        Me.cmbCorrespondent.Location = New System.Drawing.Point(140, 31)
        Me.cmbCorrespondent.Name = "cmbCorrespondent"
        Me.cmbCorrespondent.Size = New System.Drawing.Size(317, 21)
        Me.cmbCorrespondent.TabIndex = 10
        '
        'lblCorrespondent
        '
        Me.lblCorrespondent.AutoSize = True
        Me.lblCorrespondent.Location = New System.Drawing.Point(7, 34)
        Me.lblCorrespondent.Name = "lblCorrespondent"
        Me.lblCorrespondent.Size = New System.Drawing.Size(79, 13)
        Me.lblCorrespondent.TabIndex = 9
        Me.lblCorrespondent.Text = "Correspondent:"
        '
        'btnSubmit
        '
        Me.btnSubmit.Location = New System.Drawing.Point(196, 261)
        Me.btnSubmit.Name = "btnSubmit"
        Me.btnSubmit.Size = New System.Drawing.Size(75, 23)
        Me.btnSubmit.TabIndex = 8
        Me.btnSubmit.Text = "Submit"
        Me.btnSubmit.UseVisualStyleBackColor = True
        '
        'lblComments
        '
        Me.lblComments.AutoSize = True
        Me.lblComments.Location = New System.Drawing.Point(10, 153)
        Me.lblComments.Name = "lblComments"
        Me.lblComments.Size = New System.Drawing.Size(107, 13)
        Me.lblComments.TabIndex = 4
        Me.lblComments.Text = "Comments (Optional):"
        '
        'txtComments
        '
        Me.txtComments.Location = New System.Drawing.Point(10, 172)
        Me.txtComments.MaxLength = 255
        Me.txtComments.Multiline = True
        Me.txtComments.Name = "txtComments"
        Me.txtComments.Size = New System.Drawing.Size(447, 82)
        Me.txtComments.TabIndex = 7
        '
        'chkDefect
        '
        Me.chkDefect.AutoSize = True
        Me.chkDefect.Location = New System.Drawing.Point(10, 126)
        Me.chkDefect.Name = "chkDefect"
        Me.chkDefect.Size = New System.Drawing.Size(294, 17)
        Me.chkDefect.TabIndex = 6
        Me.chkDefect.Text = "This correspondence took longer than ususal to process."
        Me.chkDefect.UseVisualStyleBackColor = True
        '
        'lblType
        '
        Me.lblType.AutoSize = True
        Me.lblType.Location = New System.Drawing.Point(7, 61)
        Me.lblType.Name = "lblType"
        Me.lblType.Size = New System.Drawing.Size(68, 13)
        Me.lblType.TabIndex = 1
        Me.lblType.Text = "Parent Type:"
        '
        'cmbType
        '
        Me.cmbType.FormattingEnabled = True
        Me.cmbType.Location = New System.Drawing.Point(140, 58)
        Me.cmbType.Name = "cmbType"
        Me.cmbType.Size = New System.Drawing.Size(317, 21)
        Me.cmbType.TabIndex = 5
        '
        'lblConnectIndicator
        '
        Me.lblConnectIndicator.ForeColor = System.Drawing.Color.Red
        Me.lblConnectIndicator.Location = New System.Drawing.Point(-1, 0)
        Me.lblConnectIndicator.Name = "lblConnectIndicator"
        Me.lblConnectIndicator.Size = New System.Drawing.Size(210, 16)
        Me.lblConnectIndicator.TabIndex = 5
        Me.lblConnectIndicator.Text = "Not Connected"
        '
        'tmrWait
        '
        Me.tmrWait.Interval = 1000
        '
        'tmrRecheckConnection
        '
        Me.tmrRecheckConnection.Interval = 1000
        '
        'ComboBox1
        '
        Me.ComboBox1.FormattingEnabled = True
        Me.ComboBox1.Location = New System.Drawing.Point(140, 85)
        Me.ComboBox1.Name = "ComboBox1"
        Me.ComboBox1.Size = New System.Drawing.Size(317, 21)
        Me.ComboBox1.TabIndex = 5
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(10, 88)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(56, 13)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Sub Type:"
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(487, 502)
        Me.Controls.Add(Me.lblConnectIndicator)
        Me.Controls.Add(Me.grpEndRecordForm)
        Me.Controls.Add(Me.grpRecordControls)
        Me.Controls.Add(Me.btnPunch)
        Me.Controls.Add(Me.radNewRecord)
        Me.Controls.Add(Me.radCloseRecord)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmMain"
        Me.Text = "Correspondence Tracker | JPRS 2017"
        Me.grpRecordControls.ResumeLayout(False)
        Me.grpRecordControls.PerformLayout()
        Me.grpEndRecordForm.ResumeLayout(False)
        Me.grpEndRecordForm.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents radNewRecord As RadioButton
    Friend WithEvents radCloseRecord As RadioButton
    Friend WithEvents grpRecordControls As GroupBox
    Friend WithEvents lblRecordID As Label
    Friend WithEvents txtRecordID As TextBox
    Friend WithEvents btnPunch As Button
    Friend WithEvents grpEndRecordForm As GroupBox
    Friend WithEvents lblComments As Label
    Friend WithEvents txtComments As TextBox
    Friend WithEvents chkDefect As CheckBox
    Friend WithEvents lblType As Label
    Friend WithEvents cmbType As ComboBox
    Friend WithEvents btnSubmit As Button
    Friend WithEvents lblConnectIndicator As Label
    Friend WithEvents tmrWait As Timer
    Friend WithEvents txtCorrespondenceNumber As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents lblReceivedDate As Label
    Friend WithEvents dateReceived As DateTimePicker
    Friend WithEvents cmbCorrespondent As ComboBox
    Friend WithEvents lblCorrespondent As Label
    Friend WithEvents tmrRecheckConnection As Timer
    Friend WithEvents Label2 As Label
    Friend WithEvents ComboBox1 As ComboBox
End Class
