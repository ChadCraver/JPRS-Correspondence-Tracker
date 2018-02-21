Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.IO
Imports System.Threading
Imports Microsoft.Office.Interop
Imports System.Data
Imports System.Data.OleDb

Module Module1

    Sub Main()
        Console.Title = "JPRS Correspondence Controller"
        Console.WindowWidth = 200
        ProcessLogger.CreateLogFile()

        'Dim ListeningThread As Thread
        'ListeningThread = New Thread(AddressOf Communications.StartListening)
        'ListeningThread.IsBackground = True
        'ListeningThread.Start()

        Communications.StartListening()
    End Sub

End Module

Public Class Controller

    Public Shared intLastIDCreated As Integer = 0
    Public Shared dteDateOfLastIDCreation As Date = Date.Today

    Public Shared Function GenerateReceipt(ByVal recordID As String, ByVal request As String, ByVal dateAndTime As String) As String
        ' Return string like <MSG><MYIP={IP ADDRESS}><RECEIPT={RecordID,Request,SystemTime}></MSG>
        If recordID = "" Then recordID = "N/A"
        Dim receipt As String
        receipt = $"<RECEIPT={recordID},{request},{dateAndTime}>"
        Return receipt
    End Function

    Public Shared Sub NewRecord(ByVal delimitedInfo As String, ByVal clientIP As String)
        ' Expected message like '<NEW=Total TotalItems, Time, User Name>'

        delimitedInfo = Replace(delimitedInfo, "<NEW=", "")
        delimitedInfo = Replace(delimitedInfo, ">", "")
        Dim info() As String = Split(delimitedInfo, ",")
        Dim intTotalIDs As Integer = CInt(info(0))
        Dim strTime As String = info(1)
        Dim strUser As String = info(2)

        Try
            Dim cn As OleDbConnection
            cn = New OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\CJC.JPRS\Documents\JP Correspondence DB\Correspondence_Tracker.accdb;")
            cn.Open()

            ' Check the last ID entered into the database.
            Dim lastID As String = GetLastID()

            ' If today's date is in the last ID then start from the last correspondence ID.
            If InStr(Left(lastID, 6), Format(Date.Today, "MMddyy")) <> 0 Then
                intLastIDCreated = CInt(Right(lastID, 6)) + 1
            Else
                ' If today's date is not in the last ID, check if the date the last ID created is different from today.
                If dteDateOfLastIDCreation <> Date.Today Then
                    ' If the dates are different, reset the counter.
                    intLastIDCreated = 0
                End If
                ' Continue iteration. ID cannot equal "000000".
                intLastIDCreated += 1
            End If

            For X = intLastIDCreated To (intLastIDCreated + (intTotalIDs - 1))
                Dim strID As String = GenerateID(X)
                ' Verify the ID being submitted doesn't create a duplicate.
                If VerifyIDExists(strID) = False Then
CreateRecord:
                    Dim sqlCmd As String
                    Dim strDateTime As String = Format(Date.Today, "MM/dd/yy") & " " & strTime
                    sqlCmd = $"INSERT INTO [Master] VALUES('{strID}','{strDateTime}','{strUser}','','','','','','','','','')"
                    ProcessLogger.Write($"> Issuing SQL command: {sqlCmd}")
                    Dim oleCmd As New OleDbCommand(sqlCmd, cn)
                    oleCmd.ExecuteNonQuery()
                    Communications.SendMessage(GenerateReceipt(strID, "NEW", DateAndTime.Now), clientIP)
                    intLastIDCreated = X
                    dteDateOfLastIDCreation = Date.Today
                Else
                    ' If a duplicate would be created, iterate the value until the value doesn't already exist.
                    Dim adjustedID As Int64
                    Do Until VerifyIDExists(strID) = False
                        adjustedID = CDbl(strID)
                        adjustedID += 1
                        If adjustedID.ToString.Length = 11 Then
                            strID = "0" & adjustedID.ToString
                        End If
                    Loop
                    If adjustedID.ToString.Length = 11 Then
                        strID = "0" & adjustedID.ToString
                    ElseIf adjustedID.ToString.Length = 12 Then
                        strID = adjustedID.ToString
                    Else
                        SendDistressCall($"Overflow generating new IDs while handling this request: ({delimitedInfo})")
                    End If
                    GoTo CreateRecord
                End If
            Next
            cn.Close()

        Catch ex As Exception
            ProcessLogger.Write(">" & ex.Message)
            Communications.SendMessage(GenerateReceipt("", "ERROR", DateAndTime.Now), clientIP)
            SendDistressCall($"Exception: [{ex.HResult}] {ex.Message}")
        End Try


    End Sub

    Public Shared Sub EndRecord(ByVal delimitedInfo As String, clientIP As String)
        ' Expected message like " <END={strRecordID}, {strUserDomain}, {strCorrespondent}, {strParentType}, {strSubType}, {strDefect}, {txtComments.Text}>"
        delimitedInfo = Replace(delimitedInfo, "<END=", "")
        delimitedInfo = Replace(delimitedInfo, ">", "")
        Dim info() As String = Split(delimitedInfo, ",")
        Dim strID As String = info(0)
        Dim strUser As String = info(1)
        Dim strEndTime As String = Format(DateAndTime.Now, "MM/dd/yy hh: mm:ss tt").ToString
        Dim strCorrespondent As String = info(2)
        Dim strParentType As String = info(3)
        Dim strSubType As String = info(4)
        Dim strDefect As String = info(5)
        Dim strComments As String = info(6)

        Try
            If Not VerifyIDExists(strID) Then
                Communications.SendMessage(GenerateReceipt(strID, "ERROR", DateAndTime.Now), clientIP)
                ProcessLogger.Write("> Invalid ID specified. Unable to update records.")
                Exit Sub
            End If

            Dim cn As OleDbConnection
            cn = New OleDbConnection("Provider=Microsoft.Ace.Oledb.12.0;Data Source=C:\Users\CJC.JPRS\Documents\JP Correspondence DB\Correspondence_Tracker.accdb;")
            cn.Open()
            Dim sqlCmd As String = "UPDATE [Master] SET [End_Time]=?, [End_User_ID]=?, [Correspondent_Type]=?, [Correspondence_Parent_Type]=?, [Correspondence_Sub_Type]=?, [Defect]=?, [Comments]=? WHERE [Correspondence_ID]=?"
            ProcessLogger.Write($"> Issuing SQL command: UPDATE [Master] SET [End_Time]='{strEndTime}', [End_User_ID]='{strUser}', [Correspondent_Type]={strCorrespondent}, [Correspondence_Parent_Type]={strParentType}, [Correspondence_Sub_Type]={strSubType}, [Defect]={strDefect}, [Comments]={strComments} WHERE [Correspondence_ID]='{strID}'")
            Dim oleCmd As New OleDbCommand(sqlCmd, cn)
            oleCmd.Parameters.AddWithValue("@End_Time", strEndTime)
            oleCmd.Parameters.AddWithValue("@End_User_ID", strUser)
            oleCmd.Parameters.AddWithValue("@Correspondent_Type", strCorrespondent)
            oleCmd.Parameters.AddWithValue("@Correspondence_Parent_Type", strParentType)
            oleCmd.Parameters.AddWithValue("@Correspondence_Sub_Type", strSubType)
            oleCmd.Parameters.AddWithValue("@Defect", strDefect)
            oleCmd.Parameters.AddWithValue("@Comments", strComments)
            oleCmd.Parameters.AddWithValue("@Correspondence_ID", strID)
            oleCmd.ExecuteNonQuery()
            cn.Close()
            Communications.SendMessage(GenerateReceipt(strID, "END", DateAndTime.Now), clientIP)
        Catch ex As Exception
            ProcessLogger.Write(">" & ex.Message)
            Communications.SendMessage(GenerateReceipt(strID, "ERROR", DateAndTime.Now), clientIP)
            SendDistressCall($"Exception: [{ex.HResult}] {ex.Message}")
        End Try
    End Sub

    Public Shared Sub FollowUp(ByVal delimitedInfo As String, clientIP As String)
        ' Expected message like '<FOLLOW_UP=ID, User, Date, Time, Defect>'
        delimitedInfo = Replace(delimitedInfo, "<FOLLOW_UP=", "")
        delimitedInfo = Replace(delimitedInfo, ">", "")
        Dim info() As String = Split(delimitedInfo, ",")
        Dim strID As String = info(0)
        Dim strUser As String = info(1)
        Dim strDateTime As String = info(2) & " " & info(3)
        Dim strDefect As String = info(4)
        Dim strComments As String = info(5)

        Try
            If Not VerifyIDExists(strID) Then
                Communications.SendMessage(GenerateReceipt(strID, "ERROR", DateAndTime.Now), clientIP)
                ProcessLogger.Write("> Invalid ID specified. Unable to update records.")
                Exit Sub
            End If

            Dim cn As OleDbConnection
            cn = New OleDbConnection("Provider=Microsoft.Ace.Oledb.12.0;Data Source=C:\Users\CJC.JPRS\Documents\JP Correspondence DB\Correspondence_Tracker.accdb;")
            cn.Open()
            Dim sqlCmd As String
            sqlCmd = "UPDATE [Master] SET [Second_End_Time]=?, [Second_End_User_ID]=?, [Defect]=?, [Comments]=? WHERE [Correspondence_ID]=?"
            ProcessLogger.Write($"> Issuing SQL command: UPDATE [Master] SET [Second_End_Time]={strDateTime}, [Second_End_User_ID]={strUser}, [Defect]={strDefect}, [Comments]={strComments} WHERE [Correspondence_ID]={strID}")
            Dim oleCmd As New OleDbCommand(sqlCmd, cn)
            oleCmd.Parameters.AddWithValue("@Second_End_Time", strDateTime)
            oleCmd.Parameters.AddWithValue("@Second_End_User_ID", strUser)
            oleCmd.Parameters.AddWithValue("@Defect", strDefect)
            oleCmd.Parameters.AddWithValue("@Comments", strComments)
            oleCmd.Parameters.AddWithValue("@Correspondence_ID", strID)
            oleCmd.ExecuteNonQuery()
            cn.Close()
            Communications.SendMessage(GenerateReceipt(strID, "FOLLOW_UP", DateAndTime.Now), clientIP)
        Catch ex As Exception
            ProcessLogger.Write(">" & ex.Message)
            Communications.SendMessage(GenerateReceipt(strID, "ERROR", DateAndTime.Now), clientIP)
            SendDistressCall($"Exception: [{ex.HResult}] {ex.Message}")
        End Try
    End Sub

    Public Shared Sub EditID(ByVal oldID As String, newID As String, clientIP As String)
        Try
            Dim cn As OleDbConnection
            cn = New OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\CJC.JPRS\Documents\JP Correspondence DB\Correspondence_Tracker.accdb;")
            cn.Open()

            Dim sqlCmd As String = "UPDATE [Master] SET [Correspondence_ID]=? WHERE [Correspondence_ID]=?"
            ProcessLogger.Write($"> Issuing SQL command: UPDATE [Master] SET [Correspondence_ID]='{newID}' WHERE [Correspondence_ID]='{oldID}'")
            Dim oleCmd As New OleDbCommand(sqlCmd, cn)
            oleCmd.Parameters.AddWithValue("@p1", newID)
            oleCmd.Parameters.AddWithValue("@p2", oldID)
            oleCmd.ExecuteNonQuery()

            cn.Close()
            Communications.SendMessage(GenerateReceipt(newID, "EDITID", DateAndTime.Now), clientIP)
        Catch ex As Exception
            ProcessLogger.Write(">" & ex.Message)
            Communications.SendMessage(GenerateReceipt(newID, "ERROR", DateAndTime.Now), clientIP)
            SendDistressCall($"Exception: [{ex.HResult}] {ex.Message}")
        End Try
    End Sub

    Public Shared Sub CreateChildren(ByVal clientMSG As String, ByVal clientIP As String)

        ' Parse message. <NEW_CHILDREN=Number of Children, Parent ID>
        clientMSG = Replace(clientMSG, "<NEW_CHILDREN=", "")
        clientMSG = Replace(clientMSG, ">", "")
        Dim msgInfo() As String = Split(clientMSG, ",")

        ' Change the parent ID to be the first child.
        Call EditID(msgInfo(1), $"{msgInfo(1)}:1", clientIP)

        ' Get the values of the parent to copy to the children
        Dim cn As OleDbConnection
        cn = New OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\CJC.JPRS\Documents\JP Correspondence DB\Correspondence_Tracker.accdb;")

        Dim command As New OleDbCommand($"SELECT * FROM Master WHERE [Correspondence_ID]='{msgInfo(1)}:1'", cn)

        cn.Open()
        Dim reader As OleDbDataReader = command.ExecuteReader()

        Dim NumberOfColums As Integer
        Dim meta As Object() = New Object(11) {}
        Dim info(0 To 11) As String
        Dim read As Boolean

        If reader.Read() = True Then
            Do
                NumberOfColums = reader.GetValues(meta)

                For i As Integer = 0 To NumberOfColums - 1
                    For X = LBound(info) To UBound(info)
                        If info(X) = "" Then
                            info(X) = meta(i)
                            Exit For
                        End If
                    Next
                Next

                read = reader.Read()
            Loop While read = True
        End If

        reader.Close()

        Dim sqlCmd As String = ""
        Dim oleCmd As OleDbCommand = Nothing

        For X = 2 To CInt(msgInfo(0))
            sqlCmd = $"INSERT INTO [Master] VALUES('{msgInfo(1)}:{X}','{info(1)}','{info(2)}','','','','','{info(5)}','{info(6)}','{info(7)}','','')"
            oleCmd = New OleDbCommand(sqlCmd, cn)
            ProcessLogger.Write($"> Issuing SQL command: {sqlCmd}")
            oleCmd.ExecuteNonQuery()
            Communications.SendMessage(GenerateReceipt($"{msgInfo(1)}:{X}", "NEW", DateAndTime.Now), clientIP)
        Next

        cn.Close()
    End Sub

    Public Shared Sub AdminQuery(ByVal clientMSG As String, ByVal clientIP As String)

        ' Parse message. <QUERY=[OPEN/CLOSED],[ALL/TODAY/MMDDYY]>
        clientMSG = Replace(clientMSG, "<QUERY=", "")
        clientMSG = Replace(clientMSG, ">", "")

        Dim msgInfo() As String = Split(clientMSG, ",")
        Dim strQuery As String = ""

        Select Case True
            Case msgInfo(0) = "OPEN"
                strQuery = runQuery("[End_Time]='' AND [End_User_ID]='' AND [Second_End_Time]='' AND [Second_End_User_ID]=''", msgInfo(1))

                If strQuery.Length > 9000 Then
                    Dim lines() As String = Split(strQuery, vbCrLf)
                    For X = LBound(lines) To UBound(lines)
                        Communications.SendMessage(lines(X), clientIP)
                    Next
                Else
                    Communications.SendMessage(strQuery, clientIP)
                End If
            Case msgInfo(0) = "CLOSED"
                strQuery = runQuery("([End_Time]<>'' AND [End_User_ID]<>'') OR ([Second_End_Time]<>'' AND [Second_End_User_ID]<>'')", msgInfo(1))

                If strQuery.Length > 9000 Then
                    Dim lines() As String = Split(strQuery, vbCrLf)
                    For X = LBound(lines) To UBound(lines)
                        Communications.SendMessage(lines(X), clientIP)
                    Next
                Else
                    Communications.SendMessage(strQuery, clientIP)
                End If
            Case msgInfo(0) Like "############", msgInfo(0) Like "############:*"
                strQuery = runQuery($"[Correspondence_ID]='{msgInfo(0)}'", msgInfo(1))

                If strQuery.Length > 9000 Then
                    Dim lines() As String = Split(strQuery, vbCrLf)
                    For X = LBound(lines) To UBound(lines)
                        Communications.SendMessage(lines(X), clientIP)
                    Next
                Else
                    Communications.SendMessage(strQuery, clientIP)
                End If
            Case Else

        End Select
    End Sub

    Private Shared Function runQuery(ByVal whereParameters As String, Optional ByVal dateArg As String = "ALL")
        Dim cn As OleDbConnection
        cn = New OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\CJC.JPRS\Documents\JP Correspondence DB\Correspondence_Tracker.accdb;")

        Dim command As New OleDbCommand($"SELECT * FROM Master WHERE {whereParameters}", cn)

        cn.Open()
        Dim reader As OleDbDataReader = command.ExecuteReader()

        Dim NumberOfColums As Integer
        Dim meta As Object() = New Object(11) {}
        Dim strQuery As String = ""
        Dim read As Boolean
        Dim recordCount As Integer = 0

        If reader.Read() = True Then

            Do
                NumberOfColums = reader.GetValues(meta)

                If dateArg = "ALL" Then
                    strQuery &= vbCrLf & meta(0)
                    recordCount += 1
                ElseIf dateArg = "TODAY" Then
                    If InStr(meta(1), Format(Date.Today, "MM/dd/yy")) <> 0 Then
                        strQuery &= vbCrLf & meta(0)
                        recordCount += 1
                    End If
                ElseIf dateArg Like "######" Then
                    dateArg = $"{dateArg.Substring(0, 2)}/{dateArg.Substring(2, 2)}/{dateArg.Substring(4, 2)}"

                    If InStr(meta(1), dateArg) <> 0 Then
                        strQuery &= vbCrLf & meta(0)
                        recordCount += 1
                    End If
                ElseIf dateArg = "" Then

                    For i = 0 To NumberOfColums - 1
                        Select Case i
                            Case 0
                                strQuery &= $"ID: {meta(i)}" & vbCrLf
                                recordCount += 1
                            Case 1
                                strQuery &= $"Start: {meta(i)}" & vbCrLf
                            Case 2
                                strQuery &= $"User: {meta(i)}" & vbCrLf
                            Case 3
                                strQuery &= $"End: {meta(i)}" & vbCrLf
                            Case 4
                                strQuery &= $"User: {meta(i)}" & vbCrLf
                            Case 5
                                strQuery &= $"End: {meta(i)}" & vbCrLf
                            Case 6
                                strQuery &= $"User: {meta(i)}" & vbCrLf
                            Case 7
                                strQuery &= $"Correspondent: {meta(i)}" & vbCrLf
                            Case 8
                                strQuery &= $"Parent Type: {meta(i)}" & vbCrLf
                            Case 9
                                strQuery &= $"Sub Type: {meta(i)}" & vbCrLf
                            Case 10
                                strQuery &= $"Defective: {meta(i)}" & vbCrLf
                            Case 11
                                strQuery &= $"Comments: {meta(i)}" & vbCrLf
                        End Select

                    Next
                End If

                read = reader.Read()
            Loop While read = True
        End If

        strQuery &= vbCrLf & $"Total Records: {recordCount}" & vbCrLf
        strQuery &= "END QUERY"

        Return strQuery
    End Function

    Private Shared Function VerifyIDExists(ByVal ID As String)
        Dim cn As OleDbConnection
        cn = New OleDbConnection("Provider=Microsoft.Ace.Oledb.12.0;Data Source=C:\Users\CJC.JPRS\Documents\JP Correspondence DB\Correspondence_Tracker.accdb;")
        Dim ds As New DataSet
        Dim da As New OleDb.OleDbDataAdapter
        Dim count As Integer = 0
        Dim sqlCmd As String = "SELECT [Correspondence_ID] FROM [Master] WHERE [Correspondence_ID]='" & ID & "'"
        Dim oleCmd As New OleDbCommand

        cn.Open()
        oleCmd.Connection = cn

        da.SelectCommand = New OleDbCommand(sqlCmd, cn)
        da.Fill(ds, "Table1")
        count = ds.Tables("Table1").Rows.Count

        If count = 0 Then
            cn.Close()
            Return False
        End If

        If count = 1 Then
            cn.Close()
            Return True
        End If

        If count > 1 Then
            cn.Close()
            ProcessLogger.Write("> Database primary key [" & ID & "] compromised. The same key exists for multiple records.")
            Return False
        End If

        cn.Close()
        Return False
    End Function

    Private Shared Function GenerateID(ByVal partialID As String)
        Dim strDatePrefix As String = Format(Date.Today, "MMddyy")
        Dim strID As String = ""

        ' Check if the ID provided was 6 digits.
        If partialID.Length < 6 Then
            ' If not, add the remaining digits as zeros.
            Dim intAddZeros As Integer = 6 - partialID.Length
            For X = 1 To intAddZeros
                strID &= 0
            Next
            strID &= partialID
            ' Return the ID with the date prefix
            Return strDatePrefix & strID
        Else
            ' If yes, return the provided ID with the date prefix.
            strID = partialID
            Return strDatePrefix & strID
        End If
    End Function

    Private Shared Function GetLastID()
        Dim cn As OleDbConnection
        cn = New OleDbConnection("Provider=Microsoft.Ace.Oledb.12.0;Data Source=C:\Users\CJC.JPRS\Documents\JP Correspondence DB\Correspondence_Tracker.accdb;")
        Dim dr As OleDb.OleDbDataReader
        Dim da As New OleDb.OleDbDataAdapter
        Dim IDFromTable As String
        Dim sqlCmd As String = "SELECT [Correspondence_ID] FROM [Master] ORDER BY [Correspondence_ID] DESC"
        Dim oleCmd As New OleDbCommand

        cn.Open()
        oleCmd = cn.CreateCommand
        oleCmd.CommandText = sqlCmd

        dr = oleCmd.ExecuteReader

        If dr.Read Then
            IDFromTable = dr(0)
            Return IDFromTable
        End If

        Return False

    End Function

    Public Shared Function getLetterCount()
        Dim cn As OleDbConnection
        cn = New OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\CJC.JPRS\Documents\JP Correspondence DB\Correspondence_Tracker.accdb;")
        cn.Open()
        Dim cmd As OleDbCommand = New OleDbCommand($"SELECT COUNT(*) FROM Master WHERE Correspondence_ID LIKE '{Format(Date.Today, "MMddyy")}%'", cn)
        Dim count As String = cmd.ExecuteScalar
        cn.Close()
        Return count
    End Function

    Public Shared Sub SendDistressCall(Optional ByVal exception As String = "[UNKNOWN]")

        Try
            Dim oApp As Object = CreateObject("Outlook.Application")
            Dim objMail As Outlook.MailItem = oApp.CreateItem(0)
            Dim strTime As String = DateAndTime.TimeOfDay
            With objMail
                .To = "collectionsoperations@jprecovery.com"
                .Subject = "URGENT - Distress Message from JPRS Correspondence Controller"
                .Importance = Outlook.OlImportance.olImportanceHigh
                .Body = $"You're receiving this message because a critical error was encountered by the JPRS Correspondence Controller. It is possible the program is unable to handle requests at this time.{vbCrLf}{vbCrLf}The exception encountered was: {exception}{vbCrLf}{vbCrLf}Please review the logs and restart the application by logging into JPRS\CJC on 192.168.0.48. You may need to request logs from all participants to retrieve lost data.{vbCrLf}{vbCrLf}This is an automated message."
                .Send()
            End With
        Catch ex As Exception

        End Try
    End Sub
End Class

Public Class ProcessLogger

    Public Shared strLogPath As String = "C:\Users\CJC.JPRS\Documents\JPRS Correspondence Controller\JPRSCorroLog_" & Format(Date.Today, "MMddyyyy") & ".txt"

    Public Shared Sub Write(text As String)
        Try
            Using ProcessLogger As StreamWriter = File.AppendText(strLogPath)
                ProcessLogger.Write(text & vbNewLine)
                Console.WriteLine(text)
            End Using
        Catch ex As Exception
            Console.WriteLine("<" & DateAndTime.Today & "> - " & Err.Number & " " & Err.Description)
            Controller.SendDistressCall($"Exception: [{ex.HResult}] {ex.Message}")
        End Try
    End Sub

    Public Shared Sub CreateLogFile()
        ' Creates text file for logging application processes.

        Try
            If My.Computer.FileSystem.FileExists(strLogPath) Then
                Console.WriteLine("<" & TimeOfDay & "> - Existing log file located." & vbNewLine)
            Else
                Dim log As FileStream = File.Create(strLogPath)
                log.Close()
                Call InsertHeader(strLogPath)
            End If
        Catch ex As Exception
            Using ProcessLogger As StreamWriter = File.AppendText(strLogPath)
                ProcessLogger.Write("An error was encountered. " & Err.Number & " " & Err.Description & vbCrLf)
            End Using
            Controller.SendDistressCall($"Exception: [{ex.HResult}] {ex.Message}")
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
            ProcessLogger.Write("        " & DateAndTime.Now & " - JPRS Correspondence Controller" & vbCrLf)
            ProcessLogger.Write("================================================================================" & vbCrLf)
        End Using
        Call Main()
    End Sub

End Class

Public Class Communications
    Const JPRS1257IP As String = "192.168.0.48"
    Const ReceivingPort As Integer = 49189
    Const SendingPort As Integer = 49188
    Public Shared endpoint As IPEndPoint
    Public Shared ReceivingServer As New UdpClient(ReceivingPort)
    Public Shared SendingServer As New UdpClient(SendingPort)
    Public Shared client_IP, client_MSG As String
    Public Shared clientIPList(0 To 99) As String
    Public Shared queuedUpdates(0 To 99) As String

    Public Shared Sub StartListening()
        endpoint = New IPEndPoint(IPAddress.Any, ReceivingPort)
        Dim data() As Byte

        While True
            'Try
            data = ReceivingServer.Receive(endpoint)
                ProcessLogger.Write("<" & DateAndTime.Now & "> | RECEIVED | " & Encoding.ASCII.GetString(data))
                ProcessClientMessage(data)
            'Catch ex As Exception
            'MsgBox(ex.Message, vbOKOnly + vbCritical, "Unhandled Communications Error")
            'End Try

        End While
    End Sub

    Public Shared Sub SendMessage(ByVal message As String, ByVal clientIP As String)

        Dim clientEndPoint As IPEndPoint
        clientEndPoint = New IPEndPoint(IPAddress.Parse(clientIP), SendingPort)

        'SendingServer.Connect(clientEndPoint)

        Dim MYIP As String = Dns.GetHostByName(Dns.GetHostName).AddressList(0).ToString
        message = "<MSG><MYIP=" & MYIP & ">" & message & "</MSG>"
        ProcessLogger.Write("<" & DateAndTime.Now & "> |   SENT   | " & message)

        Dim data() As Byte = Encoding.ASCII.GetBytes(message)
        SendingServer.Send(data, data.Length, clientEndPoint)

    End Sub

    Public Shared Sub ProcessClientMessage(ByVal data As Byte())

        Dim message As String = Encoding.ASCII.GetString(data)

        ' Get IP address.
        Dim intIPStartPos As Integer = InStr(message, "<MYIP=")
        Dim intIPEndPos As Integer = Nothing
        Dim blnIPSaved As Boolean = False

        If intIPStartPos <> 0 Then
            Dim s As String = Mid(message, intIPStartPos, message.Length)
            Dim intStringCounter As Integer = 0
            intIPEndPos = InStr(s, ">") + 5

            ' Extract IP address.
            ' Mid start = (intIPStartPos + 6) because Len("<MYIP=") = 6
            ' Mid end = (intIPEndPos - 12) because Len("<MSG><MYIP=") not factored into previous calculation and the value must include the end position.
            client_IP = Mid(message, (intIPStartPos + 6), (intIPEndPos - 12))

            ' Add client_IP to clientIPList().
            ' Check if the IP is already in the array.
            For X = LBound(clientIPList) To UBound(clientIPList)
                If clientIPList(X) = client_IP Then
                    blnIPSaved = True
                    Exit For
                End If
            Next
            ' If not then add to the array.
            If Not blnIPSaved Then
                For X = LBound(clientIPList) To UBound(clientIPList)
                    If clientIPList(X) = "" Then
                        clientIPList(X) = client_IP
                        Exit For
                    End If
                Next
            End If

            ' Check if another sub node exists or if a simple message is enclosed.
            s = Mid(message, intIPEndPos + 1, intIPEndPos + 1)
            Dim charArr() As Char = s.ToCharArray
            If s(0) = "<"c Then
                ' Possible messages containing sub nodes from the client are either a NEW, END, or INFO request.
                ' In all cases, return the entire sub node for processing outside of the function.
                client_MSG = Mid(message, intIPEndPos + 1, message.Length)
                client_MSG = Replace(client_MSG, "</MSG>", "")
                Select Case True
                    Case InStr(client_MSG, "<NEW=") <> 0
                        ' Add new record to database and update with available info.
                        Controller.NewRecord(client_MSG, client_IP)
                    Case InStr(client_MSG, "<END=") <> 0
                        ' Complete end date/time for record in db.
                        Controller.EndRecord(client_MSG, client_IP)
                    Case InStr(client_MSG, "<FOLLOW_UP=") <> 0
                        ' Complete follow up info in db.
                        Controller.FollowUp(client_MSG, client_IP)
                    Case InStr(client_MSG, "<NEW_CHILDREN=") <> 0
                        ' Create a child thread in db.
                        Controller.CreateChildren(client_MSG, client_IP)
                    Case InStr(client_MSG, "<QUERY=") <> 0
                        ' Return a count and record info for the requested query.
                        Controller.AdminQuery(client_MSG, client_IP)
                    Case InStr(client_MSG, "<NEW_USER=") <> 0
                        ' Queue updates for a new user in client app settings.
                        For X = LBound(queuedUpdates) To UBound(queuedUpdates)
                            If queuedUpdates(X) = "" Then
                                queuedUpdates(X) = client_MSG
                                Exit For
                            End If
                        Next
                    Case InStr(client_MSG, "<EDIT_USER=") <> 0
                        ' Queue updates for an existing user in client app settings.
                        For X = LBound(queuedUpdates) To UBound(queuedUpdates)
                            If queuedUpdates(X) = "" Then
                                queuedUpdates(X) = client_MSG
                                Exit For
                            End If
                        Next
                End Select
            Else ' No sub node.
                ' The only expected message from the controller will be 'HELLO?'
                If InStr(message, "HELLO?") <> 0 Then
                    Communications.SendMessage("HELLO.", client_IP)
                ElseIf InStr(message, "HELLO.") <> 0 Then

                ElseIf InStr(message, "LETTER COUNT") <> 0 Then
                    Communications.SendMessage($"<LETTER_COUNT={Controller.getLetterCount}>", client_IP)
                ElseIf InStr(message, "BROADCAST UPDATES") <> 0 Then
                    For X = LBound(queuedUpdates) To UBound(queuedUpdates)
                        If queuedUpdates(X) <> "" Then
                            Broadcast(queuedUpdates(X))
                            queuedUpdates(X) = ""
                        End If
                    Next
                ElseIf InStr(message, "REFRESH CLIENTS") <> 0 Then
                    QueryActiveClients()
                    Dim brdcstTimer As New System.Timers.Timer
                    brdcstTimer.AutoReset = False
                    brdcstTimer.Interval = 10000 ' 10 sec
                    AddHandler brdcstTimer.Elapsed, AddressOf brdcstTimer_Tick
                    brdcstTimer.Start()


                Else
                    MsgBox("An unhandled message was received from the client.", vbOKOnly + vbCritical, "Unhandled Client Message")
                End If
            End If
        End If
    End Sub

    Public Shared Sub Broadcast(ByVal message As String)
        For X = LBound(clientIPList) To UBound(clientIPList)
            If clientIPList(X) <> "" Then
                Communications.SendMessage(message, clientIPList(X))
            End If
        Next
    End Sub

    Public Shared Sub QueryActiveClients()
        Dim broadcastEndPoint As New IPEndPoint(IPAddress.Broadcast, SendingPort)
        SendingServer.EnableBroadcast = True

        'SendingServer.Connect(broadcastEndPoint)

        Dim MYIP As String = Dns.GetHostByName(Dns.GetHostName).AddressList(0).ToString
        Dim message As String = $"<MSG><MYIP={MYIP}>HELLO?</MSG>"
        ProcessLogger.Write($"<{DateAndTime.Now}> |   BROADCAST   | {message}")

        Dim data() As Byte = Encoding.ASCII.GetBytes(message)
        SendingServer.Send(data, data.Length, broadcastEndPoint)

        For X = LBound(clientIPList) To UBound(clientIPList)
            clientIPList(X) = ""
        Next

    End Sub

    Private Shared Function getIPListCount()
        Dim count As Integer = 0

        For X = LBound(clientIPList) To UBound(clientIPList)
            If clientIPList(X) <> "" Then count += 1
        Next

        Return count
    End Function

    Private Shared Sub brdcstTimer_Tick(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs)
        Communications.SendMessage($"{Communications.getIPListCount} clients responded to the broadcast.", client_IP)
    End Sub

End Class
