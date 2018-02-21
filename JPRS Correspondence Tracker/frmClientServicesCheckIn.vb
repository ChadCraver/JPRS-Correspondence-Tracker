Imports System.ComponentModel
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading
Imports System.IO

Public Class frmClientServicesCheckIn

    Private Sub frmClientServicesCheckIn_Shown(sender As Object, e As EventArgs) Handles Me.Shown

        ' Notify controller instance is active.
        Try
            If Me.IsHandleCreated Then
                Communications.SendMessage("HELLO?")
                If Not tmrWait.Enabled Then
                    tmrWait.Start()
                End If
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub tmrWait_Tick(sender As Object, e As EventArgs) Handles tmrWait.Tick
        tmrWait.Stop()

        If frmMain.IsConnected Then
            lblConnectIndicator.Text = "Connected: " & DateAndTime.Now
            lblConnectIndicator.ForeColor = Color.Green
            lblConnectIndicator.Refresh()
        Else
            frmClientServicesCheckIn_Shown(Nothing, Nothing)
        End If

    End Sub

    Private Sub btnCheckIn_Click(sender As Object, e As EventArgs) Handles btnCheckIn.Click
        Dim strTimeItemsPickedUp As String = Nothing
        Dim strTotalItems As String = Nothing

        If txtTimeReceived.Text Like "*#:## AM" Then
            strTimeItemsPickedUp = txtTimeReceived.Text
        Else
            MsgBox("Time received is not in the expected format!", vbOKOnly + vbCritical, "Input Error")
        End If





        If IsNumeric(txtTotalItems.Text) Then
            strTotalItems = txtTotalItems.Text
        Else
            MsgBox("Total items is not numeric!", vbOKOnly + vbCritical, "Input Error")
        End If

        Dim msg As String = "<NEW=" & strTotalItems & "," & strTimeItemsPickedUp & "," & Environment.UserName & ">"

        Communications.SendMessage(msg)

        txtTimeReceived.Text = ""
        txtTotalItems.Text = ""
    End Sub

    Private Sub frmClientServicesCheckIn_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Application.Exit()
    End Sub
End Class