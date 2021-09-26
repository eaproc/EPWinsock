Imports EPWinSock.v4.BASE

Imports System.IO

Public Class Form1

#Region "Constructors"

    Sub New()




        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.lblClientID.Text = String.Empty
        Me.pnlBackground.BackColor = getNextRandomColor()
        Me.txtServerIP.Text = System.Net.Dns.GetHostByName(My.Computer.Name).AddressList(0).ToString()

    End Sub


 
#End Region

    ' Private AType As Type = GetType(Form1)

    'Private LocalLogger As CODERiT.Logger.v._3._5.Log1 =
    '  New CODERiT.Logger.v._3._5.Log1(GetType(Form1)).Load(CODERiT.Logger.v._3._5.Log1.Modes.FILE, True)




#Region "ColorRandomizer"

    Private Colors() As Color = New Color() {
                                            Color.FromArgb(255, 224, 192),
                                            Color.FromArgb(255, 255, 192),
                                            Color.FromArgb(192, 255, 192),
                                            Color.FromArgb(192, 255, 255),
                                            Color.FromArgb(192, 192, 255),
                                            Color.FromArgb(255, 192, 255),
                                            Color.FromArgb(255, 192, 192)
                                            }

    Private Function getNextRandomColor() As Color

        REM if you dont call randomize, it uses same template always

        Dim intBetween0AndCount As Int32


        'For i As Int16 = 0 To 20
        Randomize()
        intBetween0AndCount = Math.Round(
                                                    (Rnd() * Me.Colors.Count)
                                                    ) Mod Me.Colors.Count  REM Without including the count itself



        ''Debug.Print(intBetween0AndCount)

        ''Next



        Return Me.Colors(intBetween0AndCount)
    End Function




#End Region


#Region "Methods"



    Private Sub appendString(ByVal Tag As String, ByVal msg As String)

        Me.txtMessageDisplayFull.Text &= String.Format("[ {0} ] {1}{2}{1}{1}", Tag.ToUpper(), vbCrLf, msg)

        If chkAutoScroll.Checked Then

            Me.txtMessageDisplayFull.SelectionStart = Me.txtMessageDisplayFull.Text.Length
            Me.txtMessageDisplayFull.ScrollToCaret()


        End If

    End Sub


    Private Sub ClearMe()
        Me.lsvAvailableClients.Items.Clear()

    End Sub

    Private Function isMessageACommand(ByVal msg As String) As COMMAND_LINE_COMMANDS
        If msg Is Nothing OrElse msg.Length = 0 Then Return COMMAND_LINE_COMMANDS.NONE


        If Strings.Left(msg, Len(CONNECT_ME_TO_USER)).ToLower().Equals(CONNECT_ME_TO_USER.ToLower()) Then
            Return COMMAND_LINE_COMMANDS.CONNECT_ME


        ElseIf Strings.Left(msg, Len(DISCONNECT_ME_FROM_USER)).ToLower().Equals(DISCONNECT_ME_FROM_USER.ToLower()) Then

            Return COMMAND_LINE_COMMANDS.DISCONNECT_ME




        Else

            Return COMMAND_LINE_COMMANDS.NONE

        End If

    End Function

    Private Function fetchParamsCommands(ByVal cmd As String, ByVal str As String)

        Return Strings.Mid(str, Len(cmd) + 1)

    End Function

#End Region


#Region "Const"

    Private Const SET_NICK_NAME = "-C SET Nick"
    Private Const BROADCAST_AVAILABLE_USER = "-C BroadCast Users"
    Private Const SEPARATE_EACH_USER = vbCrLf
    Private Const SEPARATE_EACH_USER_INFO = vbTab
    '  		==> using command [-C CONNECT ME TO 1123] |Automatically
    '==> -C Disconnect ME
    Private Const CONNECT_ME_TO_USER As String = "-C CONNECT ME TO"
    Private Const DISCONNECT_ME_FROM_USER As String = "-C Disconnect ME"


    Private Const SVR_MSG_CONNECTED_TO_USER = "-I CONNECTED TO USER"
    Private Const SVR_MSG_DISCONNECTED_FROM_USER = "-E DISCONNECTED FROM USER"

    Private Const CHAT_MESSAGE = "-C CHAT"


    Private Enum COMMAND_LINE_COMMANDS
        NONE
        CONNECT_ME
        DISCONNECT_ME
    End Enum


#End Region


#Region "Properties"



    Private Property ConnectedToUserID As Int32 = 0

    Private ReadOnly Property isConnectedToUser As Boolean
        Get
            Return Me.ConnectedToUserID <> 0
        End Get
    End Property


#End Region



    Private Sub cmdSend_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSend.Click

        Dim mtype As COMMAND_LINE_COMMANDS = Me.isMessageACommand(txtMessage.Text)

        If mtype = COMMAND_LINE_COMMANDS.NONE Then

            Me.appendString("ME:", txtMessage.Text)

            REM Encrpt message if neccessary

            EpWinSock1.SendMessage(txtMessage.Text)



        Else
            ''REM COmmand mode
            ''Select Case mtype
            ''    Case Is = COMMAND_LINE_COMMANDS.CONNECT_ME

            ''        REM I can check locally here if you are connected, you should disconnect first
            ''        EpWinSock1.SendCommand(
            ''                    CONNECT_ME_TO_USER,
            ''                               Strings.Mid(txtMessage.Text, Len(CONNECT_ME_TO_USER) + 1)
            ''                               )

            ''    Case Is = COMMAND_LINE_COMMANDS.DISCONNECT_ME

            ''        EpWinSock1.SendCommand(DISCONNECT_ME_FROM_USER, String.Empty)

            ''End Select


        End If





        txtMessage.Text = vbNullString


    End Sub

    Private Sub EpWinSock1_CommandReceived(SckCommand As EPWinSock.v4.DataXchange.StringPackages.UserCommandPackage, senderSocketID As Integer) Handles EpWinSock1.CommandReceived

        Debug.Print("Command Received: " & SckCommand.CommandName)

        REM EpWinSock1.SendData(System.Text.ASCIIEncoding.ASCII.GetBytes("Hello"))

    End Sub

    Private Sub EpWinSock1_FileReceived(FileReceived As EPWinSock.v4.Utilities.FileAccumulator, senderSocketID As Integer) Handles EpWinSock1.FileReceived

        FileReceived.Save()
        REM FileReceived.Save()
        Debug.Print("File Received: " & FileReceived.OriginalFileName)
        prgReceivingIndicator.Value = 0


        'EpWinSock1.SendFile("C:\Windows\Temp\fish__small.jpg")

    End Sub

    Private Sub EpWinSock1_FileSent(ByVal senderSocketID As Integer) Handles EpWinSock1.FileSent
        prgSendingIndicator.Value = 0
    End Sub

    ''Private Sub EpWinSock1_CommandReceived(ByVal SckCommand As String, ByVal Params As String, ByVal senderSocketID As Integer) Handles EpWinSock1.CommandReceived

    ''    Try


    ''        Select Case SckCommand

    ''            Case Is = SVR_MSG_CONNECTED_TO_USER
    ''                Me.ConnectedToUserID = CInt(Val(Params))

    ''                If Me.lsvAvailableClients.Items.ContainsKey(Me.ConnectedToUserID) Then

    ''                    Me.appendString("Server Info:",
    ''                                    "You are NOW connected to " & Me.lsvAvailableClients.Items(Me.ConnectedToUserID.ToString).Text)

    ''                    Me.lblClientID.Text = String.Format("ID: {0} ==> {1}", Me.EpWinSock1.SocketID, Me.ConnectedToUserID)
    ''                End If


    ''            Case Is = SVR_MSG_DISCONNECTED_FROM_USER
    ''                Me.appendString("Server Info:",
    ''                                "You are NOW Disconnected from " & Me.ConnectedToUserID)

    ''                Me.ConnectedToUserID = 0
    ''                Me.lblClientID.Text = "ID: " & Me.EpWinSock1.SocketID

    ''            Case Is = CHAT_MESSAGE

    ''                REM Decrpt message if neccessary

    ''                Dim Sender As String = Me.ConnectedToUserID
    ''                If Me.lsvAvailableClients.Items.ContainsKey(Me.ConnectedToUserID) Then _
    ''                    Sender = Me.lsvAvailableClients.Items(Me.ConnectedToUserID.ToString).Text

    ''                Me.appendString(Sender & ":", Params)
    ''                REM BLink if not in focus
    ''                If Form.ActiveForm Is Nothing OrElse Not (Form.ActiveForm.Equals(Me)) Then
    ''                    basFlashWin.FlashWindow(Me)
    ''                End If


    ''            Case Is = BROADCAST_AVAILABLE_USER

    ''                Dim Users() As String = Split(Params, SEPARATE_EACH_USER)
    ''                Dim UserIDs As List(Of String) = New List(Of String)(Users.Count)

    ''                'COMPLIMENT LIST
    ''                For Each usr As String In Users
    ''                    Dim __user() As String = Split(usr, SEPARATE_EACH_USER_INFO)
    ''                    UserIDs.Add(__user(0))

    ''                    If Me.lsvAvailableClients.Items.ContainsKey(__user(0)) Then
    ''                        REM Update NickName
    ''                        With Me.lsvAvailableClients.Items(__user(0))
    ''                            If __user.Count > 1 Then
    ''                                .SubItems(0).Text = String.Format("{0} - {1}", __user(0), __user(1))
    ''                            Else
    ''                                .SubItems(0).Text = String.Format("{0}", __user(0))
    ''                            End If


    ''                        End With



    ''                    Else
    ''                        REM Add User
    ''                        With Me.lsvAvailableClients.Items
    ''                            If __user.Count > 1 Then
    ''                                .Add(__user(0), String.Format("{0} - {1}", __user(0), __user(1)), 0)
    ''                            Else
    ''                                .Add(__user(0), String.Format("{0}", __user(0)), 0)
    ''                            End If


    ''                        End With


    ''                    End If


    ''                Next


    ''                Dim AllOk As Boolean

    ''                Do

    ''                    AllOk = True
    ''                    With Me.lsvAvailableClients.Items
    ''                        REM Delete old lists
    ''                        For i As Int32 = 0 To .Count - 1


    ''                            If Not UserIDs.Contains(.Item(i).Name) Then

    ''                                .RemoveAt(i)
    ''                                AllOk = False
    ''                                Exit For
    ''                            End If

    ''                        Next


    ''                    End With

    ''                Loop While Not AllOk


    ''        End Select


    ''    Catch ex As Exception

    ''    End Try

    ''End Sub

    Private Sub EpWinSock1_MessageReceived(ByVal SckMessage As String, ByVal senderSocketID As Integer) Handles EpWinSock1.MessageReceived

        Me.appendString("Server Message:", SckMessage)

    End Sub

    Private Sub EpWinSock1_ReceiveFileTransferProgress(ByVal FileName As String, ByVal TotalBytesReceived As Integer, ByVal FileBytesSize As Integer, ByVal SenderSocketID As Integer) Handles EpWinSock1.ReceiveFileTransferProgress
        prgReceivingIndicator.Maximum = FileBytesSize
        prgReceivingIndicator.Value = TotalBytesReceived
        Application.DoEvents()
    End Sub

    Private Sub EpWinSock1_ReceivingCancelled(senderSocketID As Integer) Handles EpWinSock1.ReceivingCancelled
        prgReceivingIndicator.Value = 0
        Debug.Print("EpWinSock1_ReceivingCancelled")
    End Sub

    Private Sub EpWinSock1_SendFileTransferProgress(ByVal FileName As String, ByVal TotalBytesSent As Integer, ByVal FileBytesSize As Integer, ByVal SenderSocketID As Integer) Handles EpWinSock1.SendFileTransferProgress
        prgSendingIndicator.Maximum = FileBytesSize
        prgSendingIndicator.Value = TotalBytesSent
    End Sub

    Private Sub EpWinSock1_SocketErrorMessage(ByVal SckMessage As String) Handles EpWinSock1.SocketErrorMessage

        Me.appendString("Error Message:", SckMessage)

    End Sub

    Private Sub EpWinSock1_SocketIDChanged() Handles EpWinSock1.SocketIDChanged

        ''Me.Text = "Client App - " & EpWinSock1.SocketID
        '' Else
        If Me.EpWinSock1.ConnectionState = SocketConnectionState.Connected Then _
            Me.lblClientID.Text = "ID: " & Me.EpWinSock1.SocketID

        Call txtNickName_TextChanged(Nothing, Nothing)
    End Sub

    Private Sub EpWinSock1_SocketLogMessage(ByVal sckLog As String) Handles EpWinSock1.SocketLogMessage

        Me.appendString("Log:", sckLog)
        ''REM BLink if not in focus
        ''If Form.ActiveForm Is Nothing OrElse Not (Form.ActiveForm.Equals(Me)) Then
        ''    basFlashWin.FlashWindow(Me)
        ''End If

    End Sub

    Private Sub EpWinSock1_SocketStateChanged(ByVal CurrentState As EPWinSock.v4.BASE.SocketConnectionProtocol, ByVal DebugMessage As String) Handles EpWinSock1.SocketStateChanged

        Me.appendString("Server DebugMsg:", DebugMessage)


        If CurrentState = SocketConnectionState.Disconnected Then
            Me.lblClientID.Text = String.Empty

            REM When you are offline, it is assumed you can see anything anymore
            Me.ClearMe()

        End If

    End Sub


    Private Sub cmdDisconnect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGoOffline.Click
        EpWinSock1.Stop()


    End Sub

    Private Sub cmdConnect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGoOnline.Click
        'EpWinSock1.ServerIP = txtServerIP.Text
        EpWinSock1.ServerIP = "192.168.0.107"
        EpWinSock1.Start()

    End Sub

    Private Sub picMaximize_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles picMaximize.Click
        Me.Size = Me.MaximumSize
        Me.picMaximize.Visible = False
        Me.picMinimize.Visible = True
    End Sub

    Private Sub picMinimize_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles picMinimize.Click
        Me.Size = Me.MinimumSize
        Me.picMaximize.Visible = True
        Me.picMinimize.Visible = False
    End Sub

    Private Sub pnlBackground_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles pnlBackground.Click

        Me.pnlBackground.BackColor = getNextRandomColor()

    End Sub

    Private Sub txtNickName_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtNickName.Click

    End Sub

    Private Sub txtNickName_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtNickName.GotFocus
        If Me.txtNickName.Text = "Nick Name" Then Me.txtNickName.Text = String.Empty
    End Sub

    Private Sub txtNickName_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtNickName.LostFocus

        If Me.txtNickName.Text.Trim = String.Empty Then Me.txtNickName.Text = "Nick Name"

    End Sub

    Private Sub txtNickName_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtNickName.TextChanged
        ''If Me.EpWinSock1.ConnectionState = SocketConnectionState.Connected And txtNickName.Text <> "Nick Name" Then
        ''    EpWinSock1.SendCommand(SET_NICK_NAME, txtNickName.Text)

        ''End If

    End Sub



    Private Sub EpWinSock1_SocketStateChanged(CurrentState As SocketConnectionState, DebugMessage As String) Handles EpWinSock1.SocketStateChanged

    End Sub
End Class
