Imports System.ComponentModel
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading
Imports System.IO

Public Class frmMain

    Const JPRS1257IP As String = "192.168.0.48"
    Public Shared IsConnected As Boolean = False
    Private Shared strSelectedDate As String = Nothing
    Private Shared strCorrNum As String = Nothing

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try
            ' Ensure form dimensions are unexpanded.
            ' Expanded form: 503,514
            ' Unexpanded form: 369,233
            If Me.Height <> 233 Or Me.Width <> 369 Then
                Me.Height = 233
                Me.Width = 369
            End If

            ' Create a log.
            ProcessLogger.CreateLogFile()

            ' Ensure radNewRecord is checked as the default option.
            If radNewRecord.Checked <> False Then radNewRecord.Checked = True

            ' Clear text boxes and ensure chkDefect is false.
            txtRecordID.Text = ""
            txtRecordID.ReadOnly = True
            txtRecordID.BackColor = Color.LightGray
            txtComments.Text = ""
            chkDefect.Checked = False
            cmbCorrespondent.Enabled = True
            cmbType.Enabled = False

            ' Hide controls in expanded form.
            For Each ctrl As Control In grpEndRecordForm.Controls
                ctrl.Visible = False
            Next
            grpEndRecordForm.Visible = False

            ' Add correspondent categories to cmbCorrespondent.
            ' Items will be determined by department and/or sender.
            With cmbCorrespondent.Items
                .Add("Consumer")
                .Add("Attorney")
                .Add("Other (Specify in Comments)")
            End With

            ' Add corrospondence categories to cmbType.
            ' Items will be determined by department and/or sender.
            With cmbType.Items
                .Add("Bankruptcy")
                .Add("Payment")
                .Add("Cease & Desist")
                .Add("Other (Specify in Comments)")
            End With

            dateReceived_TextChanged(Nothing, Nothing)
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub frmMain_Shown(sender As Object, e As EventArgs) Handles Me.Shown

        ' Start listening thread.
        Try
            Dim listeningThread As Thread
            listeningThread = New Thread(AddressOf Communications.Listen)
            listeningThread.IsBackground = True
            listeningThread.Start()
        Catch ex As Exception
            MsgBox(ex.Message)
            ProcessLogger.Write("<" & DateAndTime.Now & "> - " & Err.Number & " " & Err.Description)
        End Try

        If Environment.UserName = "KMK" Then
            frmClientServicesCheckIn.Show()
            Me.Hide()
            Exit Sub
        End If

        ' Notify controller instance is active.
        Try
            If Me.IsHandleCreated Then
                Communications.SendMessage("HELLO?")
                tmrWait.Start()
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
            ProcessLogger.Write("<" & DateAndTime.Now & "> - " & Err.Number & " " & Err.Description)
        End Try
    End Sub

    Private Sub btnPunch_Click(sender As Object, e As EventArgs) Handles btnPunch.Click

        If radNewRecord.Checked = True Then
            ' Send delimited string to controller.
            Dim strRecordID As String = InputBox("Enter the last ID generated which represents the total number of envelopes being distributed:", "Enter Total ID's")
            Dim strUserName As String = Environment.UserName
            Dim msg As String

            If strRecordID = "" Then
                Exit Sub
            ElseIf strRecordID = " " Then
                Do
                    strRecordID = InputBox("Enter the last ID generated which represents the total number of envelopes being distributed:", "Enter Total ID's", " ")
                Loop Until strRecordID <> " "
            ElseIf Not IsNumeric(strRecordID) Then
                MsgBox("You must senter a numeric value!", vbOKOnly + vbCritical, "Invalid Entry")
                strRecordID = InputBox("Enter the last ID generated which represents the total number of envelopes being distributed:", "Enter Total ID's", " ")
            End If

            msg = "<NEW=" & strRecordID & "," & DateAndTime.Now & "," & strUserName & ">"

            ' UDP Msg
            Communications.SendMessage(msg)

        ElseIf radCloseRecord.Checked Then
            If txtCorrespondenceNumber.Text = "" Or txtCorrespondenceNumber.Text.Length = 0 Then
                MsgBox("You must specify a correspondence number!", vbOKOnly + vbCritical, "Incomplete Check Out")
                Exit Sub
            End If
            ' Send delimited string to controller.
            Dim strRecordID As String = txtRecordID.Text
            Dim strUserName As String = Environment.UserName
            Dim msg As String

            msg = "<END=" & strRecordID & "," & strUserName & ">"

            ' UDP Msg
            Communications.SendMessage(msg)

            expandForm()

        End If
    End Sub

    Private Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click

        ' Send form information as delimited string.
        Dim strUserDomain As String = Environment.UserName
        Dim strSender As String
        Dim strType As String
        Dim strDefect As String = Nothing
        Dim strComments As String = Replace(txtComments.Text, ",", ";")
        Dim strRecordID As String = txtRecordID.Text
        Dim msg As String

        If cmbCorrespondent.SelectedIndex <> -1 Then
            strSender = cmbCorrespondent.SelectedItem.ToString
        Else
            MsgBox("You must select a correspondent type!", vbOKOnly + vbCritical, "Message to server incomplete!")
            Exit Sub
        End If

        If cmbType.SelectedIndex <> -1 Then
            strType = cmbType.SelectedItem.ToString
        Else
            MsgBox("You must select a correspondence type!", vbOKOnly + vbCritical, "Message to server incomplete!")
            Exit Sub
        End If

        If chkDefect.Checked Then
            strDefect = "Yes"
        Else
            strDefect = "No"
        End If

        msg = "<INFO=" & strRecordID & "," & strUserDomain & "," & strSender & "," & strType & "," & strDefect & "," & strComments & ">"

        ' Send UDP msg
        Communications.SendMessage(msg)

        shrinkForm()

        cmbCorrespondent.SelectedIndex = -1
        cmbType.SelectedIndex = -1
        chkDefect.Checked = False
        txtComments.Text = ""
    End Sub

    Private Sub tmrWait_Tick(sender As Object, e As EventArgs) Handles tmrWait.Tick
        tmrWait.Stop()

        If IsConnected Then
            lblConnectIndicator.Text = "Connected: " & DateAndTime.Now
            lblConnectIndicator.ForeColor = Color.Green
            lblConnectIndicator.Refresh()
            ProcessLogger.Write("<" & DateAndTime.Now & "> - Connection to server confirmed.")
        Else
            lblConnectIndicator.Text = "Not Connected"
            lblConnectIndicator.ForeColor = Color.Red
            lblConnectIndicator.Refresh()
            Communications.SendMessage("HELLO?")
            tmrWait.Start()
            ProcessLogger.Write("<" & DateAndTime.Now & "> - No connection to server. Retrying...")
        End If

    End Sub

    Private Sub dateReceived_TextChanged(sender As Object, e As EventArgs) Handles dateReceived.TextChanged
        strSelectedDate = Format(CDate(dateReceived.Text), "MMddyy")
        txtRecordID.Text = strSelectedDate & strCorrNum
    End Sub

    Private Sub txtCorrespondenceNumber_TextChanged(sender As Object, e As EventArgs) Handles txtCorrespondenceNumber.TextChanged

        If txtCorrespondenceNumber.Text.Length < 6 Then
            Dim x As Integer = 6 - txtCorrespondenceNumber.Text.Length
            Dim fullNum As String = ""
            For Y = 1 To x
                fullNum &= "0"
            Next
            fullNum &= txtCorrespondenceNumber.Text
            strCorrNum = fullNum
        End If
        txtRecordID.Text = strSelectedDate & strCorrNum

    End Sub

    Private Sub cmbCorrespondent_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbCorrespondent.SelectedIndexChanged
        If cmbCorrespondent.SelectedIndex <> -1 And cmbCorrespondent.Text.Length > 0 Then
            cmbType.Enabled = True
        Else
            cmbType.Enabled = False
        End If
    End Sub

    Private Sub tmrRecheckConnection_Tick(sender As Object, e As EventArgs) Handles tmrRecheckConnection.Tick
        tmrRecheckConnection.Stop()
        Communications.SendMessage("HELLO?")
        tmrWait.Start()
    End Sub

    Function expandForm()
        ' Expand form animation.
        If Me.Height <> 514 Then
            Do Until Me.Height = 514 And Me.Width = 503
                If Me.Width < 503 Then
                    Me.Width += 1
                End If
                If Me.Height < 514 Then
                    Me.Height += 1
                End If
            Loop
            grpEndRecordForm.Visible = True
            For Each ctrl As Control In grpEndRecordForm.Controls
                ctrl.Visible = True
            Next
        End If
        Return True
    End Function

    Function shrinkForm()
        ' Unexpand form animation.
        If Me.Width <> 369 Or Me.Height <> 233 Then
            Do Until Me.Width = 369 And Me.Height = 233
                If Me.Width > 369 Then
                    Me.Width -= 1
                End If
                If Me.Height > 233 Then
                    Me.Height -= 1
                End If
            Loop
            For Each ctrl As Control In grpEndRecordForm.Controls
                ctrl.Visible = False
            Next
            grpEndRecordForm.Visible = False
        End If
        Return True
    End Function
End Class

Public Class Communications
    Const JPRS1257IP As String = "192.168.0.48"
    Const ReceivingPort As Integer = 49188
    Const SendingPort As Integer = 49189
    Public Shared serverIP As IPAddress = IPAddress.Parse(JPRS1257IP)
    Public Shared client As UdpClient
    Public Shared ReceivingServer As New UdpClient(ReceivingPort)
    Public Shared SendingServer As New UdpClient(SendingPort)
    Public Shared endPoint As IPEndPoint

    Public Shared Sub Listen()
        endPoint = New IPEndPoint(IPAddress.Any, ReceivingPort)
        Dim data() As Byte

        While True
            Try
                data = ReceivingServer.Receive(endPoint)
                ProcessLogger.Write("<" & DateAndTime.Now & "> | RECEIVED | " & Encoding.ASCII.GetString(data))
                ReceiveServerMessage(Encoding.ASCII.GetString(data))
            Catch ex As Exception
                MsgBox(ex.Message, vbOKOnly + vbCritical, "Unhandled Communications Error")
                ProcessLogger.Write("<" & DateAndTime.Now & "> - " & Err.Number & " " & Err.Description)
            End Try

        End While

    End Sub

    Public Shared Sub SendMessage(message As String)
        Try
            Dim serverIP As IPAddress = IPAddress.Parse(JPRS1257IP)
            Dim endpoint As IPEndPoint = New IPEndPoint(serverIP, SendingPort)
            SendingServer.Connect(endpoint)
            Dim MYIP As String = Dns.GetHostByName(Dns.GetHostName).AddressList(0).ToString
            message = "<MSG><MYIP=" & MYIP & ">" & message & "</MSG>"
            ProcessLogger.Write("<" & DateAndTime.Now & "> |   SENT   | " & message)

            Dim data() As Byte = Encoding.ASCII.GetBytes(message)
            SendingServer.Send(data, data.Length)
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Public Shared Sub ReceiveServerMessage(ByVal message As String)
        Dim server_IP, server_MSG As String

        ' Get IP address.
        Dim intIPStartPos As Integer = InStr(message, "<MYIP=")
        Dim intIPEndPos As Integer = Nothing

        If intIPStartPos <> 0 Then
            Dim s As String = Mid(message, intIPStartPos, message.Length)
            Dim intStringCounter As Integer = 0
            intIPEndPos = InStr(s, ">") + 5

            ' Extract IP address.
            ' Mid start = (intIPStartPos + 6) because Len("<MYIP=") = 6
            ' Mid end = (intIPEndPos - 12) because Len("<MSG><MYIP=") not factored into previous calculation and the value must include the end position.
            server_IP = Mid(message, (intIPStartPos + 6), (intIPEndPos - 12))

            'If server_IP doesn't match the defined constant, alert the user a fidelity issue.
            'If server_IP <> JPRS1257IP Then
            'MsgBox("A message was received that was Not from the controller.", vbOKOnly + vbCritical, "Communication Infidelity")
            'End If

            ' Check if another sub node exists or if a simple message is enclosed.
            s = Mid(message, intIPEndPos + 1, intIPEndPos + 1)
            Dim charArr() As Char = s.ToCharArray
            If s(0) = "<"c Then
                ' Possible messages containing sub nodes from the controller are either a time broadcast or a receipt.
                ' In both cases, return the entire sub node for processing outside of the function.
                server_MSG = Mid(message, intIPEndPos + 1, message.Length)
                'server_MSG = Replace(server_MSG, "</MSG>", "")
            Else ' No sub node.
                ' The only expected message from the controller will be 'HELLO.'
                If message Like "*HELLO.*" Then

                    Try
                        frmMain.IsConnected = True
                        If frmMain.tmrRecheckConnection.Enabled Then
                            frmMain.tmrRecheckConnection.Stop()
                        End If

                        frmMain.tmrRecheckConnection.Start()
                    Catch ex As Exception
                        MsgBox(ex.Message)
                        ProcessLogger.Write("<" & DateAndTime.Now & "> - " & Err.Number & " " & Err.Description)
                    End Try
                Else
                    'MsgBox("An unhandled message was received from the controller.", vbOKOnly + vbCritical, "Unhandled Server Message")
                    server_MSG = message
                End If
            End If
        End If
    End Sub
End Class

Public Class ProcessLogger

    Public Shared strLogPath As String = "C:\Users\" & Environment.UserName & "\Documents\JPRS Correspondence Tracker\JPRSCorroLog_" & Format(Date.Today, "MMddyyyy") & ".txt"

    Public Shared Sub Write(text As String)
        Exit Sub
        Try
            Using ProcessLogger As StreamWriter = File.AppendText(strLogPath)
                ProcessLogger.Write(text & vbNewLine)
            End Using
        Catch ex As Exception
            MsgBox("<" & DateAndTime.Now & "> - " & Err.Number & " " & Err.Description, vbCritical + vbOKOnly, "Write to Log Error")
            ProcessLogger.Write("<" & DateAndTime.Now & "> - " & Err.Number & " " & Err.Description)
        End Try
    End Sub

    Public Shared Sub CreateLogFile()
        ' Creates text file for logging application processes.

        Try
            If Not My.Computer.FileSystem.DirectoryExists("C:\Users\" & Environment.UserName & "\Documents\JPRS Correspondence Tracker") Then
                My.Computer.FileSystem.CreateDirectory("C:\Users\" & Environment.UserName & "\Documents\JPRS Correspondence Tracker")
            End If

            If Not My.Computer.FileSystem.FileExists(strLogPath) Then
                Dim log As FileStream = File.Create(strLogPath)
                log.Close()
                Call InsertHeader(strLogPath)
            End If
        Catch
            MsgBox("An error was encountered. " & Err.Number & " " & Err.Description, vbCritical + vbOKOnly, "Create Log Error")
        End Try
    End Sub

    Public Shared Sub InsertHeader(LogFilePath As String)
        Using ProcessLogger As StreamWriter = File.AppendText(LogFilePath)
            ProcessLogger.Write("                 _ _____    _____                                    " & vbCrLf)
            ProcessLogger.Write("                | |  __ \  |  __ \                                   " & vbCrLf)
            ProcessLogger.Write("                | | |__) | | |__) |___  ___ _____   _____ _ __ _   _ " & vbCrLf)
            ProcessLogger.Write("            _   | |  ___/  |  _  // _ \/ __/ _ \ \ / / _ \ '__| | | |" & vbCrLf)
            ProcessLogger.Write("           | |__| | |      | | \ \  __/ (_| (_) \ V /  __/ |  | |_| |" & vbCrLf)
            ProcessLogger.Write("            \____/|_|      |_|  \_\___|\___\___/ \_/ \___|_|   \__, |" & vbCrLf)
            ProcessLogger.Write("                                                                __/ |" & vbCrLf)
            ProcessLogger.Write("                                                               |___/ " & vbCrLf)
            ProcessLogger.Write("================================================================================" & vbCrLf)
            ProcessLogger.Write($"      {DateAndTime.Now} - JPRS Correspondence Tracker - {Environment.UserName} - {Dns.GetHostByName(Dns.GetHostName).AddressList(0).ToString}" & vbCrLf)
            ProcessLogger.Write("================================================================================" & vbCrLf)
        End Using
    End Sub

End Class
