Option Explicit On
Option Strict On



Imports EPWinSock.v4.Utilities
Imports EPWinSock.v4.BASE
Imports EPWinSock.v4.DataXchange.StringPackages

Public Class Form1

#Region "Constructors"

    Sub New()


        ''VC.PrintBlockASCII()
        ''VC.PrintBlock(" ")
        'Debug.Print(VC.Decrypt("HOUSE", "k~#x"))

        ''Dim CipherText As String, PlainText As String = StrDup(5000, "D")

        ''Debug.Print("------------------Encrypting 5000 Characters with 5 key Size ---------------------------")
        ''Dim StartTime As Date = Now
        ''CipherText = VC.Encrypt("HOUSE", PlainText)
        ''Debug.Print("------------------Encryption Took " & basDateTime.GetTimeDifferenceInMilliseconds(StartTime, Now))


        ''Debug.Print("------------------Decrypting 5000 Characters with 5 key Size ---------------------------")
        ''StartTime = Now
        ''PlainText = VC.Decrypt("HOUSE", CipherText)
        ''Debug.Print("------------------Encryption Took " & basDateTime.GetTimeDifferenceInMilliseconds(StartTime, Now))



        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.


    End Sub



#End Region


#Region "Classes"

    Private Class clsUser
        Private _ID As Int32
        Public ReadOnly Property ID As Int32
            Get
                Return _ID
            End Get
        End Property

        Public Property Nick_Name As String

        Public ConnectedTo As clsUser = Nothing


        Public ReadOnly Property hasConnection As Boolean
            Get
                Return (Me.ConnectedTo IsNot Nothing)
            End Get
        End Property


        Sub New(ByVal usrID As Int32)
            Me._ID = usrID

        End Sub


        Public Function this() As clsUser
            Return Me

        End Function

    End Class



#End Region

#Region "Const"


    Private Const SET_NICK_NAME = "-C SET Nick"
    Private Const BROADCAST_AVAILABLE_USER = "-C BroadCast Users"
    Private Const SEPARATE_EACH_USER = vbCrLf
    Private Const SEPARATE_EACH_USER_INFO = vbTab

    Private Const CHAT_MESSAGE = "-C CHAT"


    Private Const CONNECT_ME_TO_USER As String = "-C CONNECT ME TO"
    Private Const DISCONNECT_ME_FROM_USER As String = "-C Disconnect ME"

    Private Const SVR_MSG_CONNECTED_TO_USER = "-I CONNECTED TO USER"
    Private Const SVR_MSG_DISCONNECTED_FROM_USER = "-E DISCONNECTED FROM USER"

#End Region

#Region "Properties"

    REM Maximum Capacity of 50 .. Just for presentation
    Private Property AvailableClients As Dictionary(Of Int32, clsUser) =
        New Dictionary(Of Int32, clsUser)(50)




#End Region



#Region "Methods"

    Private Sub appendString(ByVal Tag As String, ByVal msg As String)

        Me.txtMessageDisplayFull.Text &= String.Format("{0}: {1}{2}{1}{1}", Tag, vbCrLf, msg)

        If chkAutoScroll.Checked Then

            Me.txtMessageDisplayFull.SelectionStart = Me.txtMessageDisplayFull.Text.Length
            Me.txtMessageDisplayFull.ScrollToCaret()


        End If

    End Sub

    Private Sub setClientNickName(ByVal ID As Int32, ByVal NickName As String)
        If Me.AvailableClients.ContainsKey(ID) Then


            With Me.AvailableClients.Item(ID)
                .Nick_Name = NickName
                Me.lsvAvailableClients.Items(.ID.ToString).SubItems(0).Text =
                                            String.Format("{0} - {1}", .ID, .Nick_Name)


            End With

        End If

    End Sub

    Private Sub updateClientConnected(ByVal mineClass As clsUser,
                                      ByVal personIConnectedTo As clsUser,
                                      Optional ByVal isRecursiveCall As Boolean = False)

        ''Try

        ''    With mineClass

        ''        .ConnectedTo = personIConnectedTo

        ''        Me.lsvAvailableClients.Items(.ID.ToString).BackColor = Color.White
        ''        If personIConnectedTo IsNot Nothing Then


        ''            Me.lsvAvailableClients.Items(.ID.ToString).SubItems(1).Text =
        ''                        String.Format("{0} - {1}", .ConnectedTo.ID, .ConnectedTo.Nick_Name)


        ''            Me.lsvAvailableClients.Items(.ID.ToString).BackColor = Color.PaleGreen

        ''            REM IT is connection
        ''            EpWinSock1.SendCommand(SVR_MSG_CONNECTED_TO_USER, personIConnectedTo.ID.ToString, .ID)


        ''            REM I should do this on both sides
        ''            If Not isRecursiveCall Then updateClientConnected(personIConnectedTo, mineClass, True)

        ''        End If



        ''    End With



        ''Catch ex As Exception

        ''End Try

    End Sub


    Private Sub broadcastUsers()

        With New Threading.Thread(
                        New Threading.ThreadStart(AddressOf Me.broadcastUsers_Threaded)
                        )
            .SetApartmentState(Threading.ApartmentState.MTA)
            .Start()

        End With


    End Sub


    Private Sub broadcastUsers_Threaded()

        Dim _____users As String = String.Empty
        For Each usr As KeyValuePair(Of Int32, clsUser) In Me.AvailableClients

            _____users &= String.Format("{0}{1}{2}", usr.Key, SEPARATE_EACH_USER_INFO, usr.Value.Nick_Name)

            If Me.AvailableClients.Count > (Me.AvailableClients.Keys.ToList.IndexOf(usr.Key) + 1) Then
                _____users &= SEPARATE_EACH_USER

            End If

        Next

        ''Try

        ''    REM Add this function to TCP [==> broadCast TCP]
        ''    For Each usr As KeyValuePair(Of Int32, clsUser) In Me.AvailableClients

        ''        'Threading.Thread.Sleep(50)
        ''        Me.EpWinSock1.SendCommand(BROADCAST_AVAILABLE_USER, _____users, usr.Key)

        ''    Next



        ''Catch ex As Exception

        ''    Debug.Print(ex.StackTrace)


        ''End Try

    End Sub




#End Region


    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub

    Private Sub cmdConnect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdConnect.Click

        Me.EpWinSock1.Start()

    End Sub

    Private Sub EpWinSock1_AClientConnected(ByVal SocketID As Integer) Handles EpWinSock1.AClientConnected

        lsvAvailableClients.Items.Add(SocketID.ToString, SocketID.ToString, 0)

        REM add blank
        lsvAvailableClients.Items(
                                   lsvAvailableClients.Items.Count - 1
                                   ).SubItems.Add("")


        Me.AvailableClients.Add(SocketID, New clsUser(SocketID))

        Me.broadcastUsers()



        Debug.Print("Client Connected: " & EpWinSock1.getClient(SocketID).ToString())

        ''Me.lstClientLists.Items.Clear()
        ''Me.lstClientLists.Items.AddRange(EpWinSock1.ConnectedClients.Cast(Of Object)().ToArray)
    End Sub

    Private Sub EpWinSock1_AClientDisconnected(ByVal SocketID As Integer) Handles EpWinSock1.AClientDisconnected

        If lsvAvailableClients.Items.ContainsKey(SocketID.ToString) Then
            lsvAvailableClients.Items.RemoveByKey(SocketID.ToString)
        End If

        If Me.AvailableClients.ContainsKey(SocketID) Then

            With Me.AvailableClients(SocketID)
                If .hasConnection Then
                    If .ConnectedTo.hasConnection Then EpWinSock1.SendMessage("You are now disconnected from user " & .ConnectedTo.ConnectedTo.ID, .ConnectedTo.ID)
                    Me.updateClientConnected(.ConnectedTo, Nothing) REM remove connection

                End If

            End With


            Me.AvailableClients.Remove(SocketID)



        End If

        Me.broadcastUsers()

        ''Me.lstClientLists.Items.Clear()
        ''Me.lstClientLists.Items.AddRange(EpWinSock1.ConnectedClients.Cast(Of Object)().ToArray)
    End Sub

    Private Sub EpWinSock1_CommandReceived(SckCommand As EPWinSock.v4.DataXchange.StringPackages.UserCommandPackage, senderSocketID As Integer) Handles EpWinSock1.CommandReceived
        Debug.Print("Command Received: " & SckCommand.CommandName)
    End Sub

    Private Sub EpWinSock1_DataReceived(SckData() As Byte, senderSocketID As Integer) Handles EpWinSock1.DataReceived
        Debug.Print("Data Received Length: " & SckData.Length)
    End Sub

    Private Sub EpWinSock1_FileReceived(ByVal FileReceived As EPWinSock.v4.Utilities.FileAccumulator, ByVal senderSocketID As Integer) Handles EpWinSock1.FileReceived
        prgReceivingIndicator.Value = 0

        Debug.Print("File Received: " & FileReceived.OriginalFileName)

    End Sub

    Private Sub EpWinSock1_FileSent(senderSocketID As Integer) Handles EpWinSock1.FileSent
        REM EpWinSock1.SendMessage("Hello there", senderSocketID)
        Debug.Print(EPRO.Library.v3._5.Objects.EDateTime.GetTimeDifferenceInMilliseconds(Me.SentTimeStart, Now) & " ms")
        prgSendingIndicator.Value = 0
    End Sub



    ' ''Private Sub EpWinSock1_CommandReceived(ByVal SckCommand As String, ByVal Params As String, ByVal senderSocketID As Integer) Handles EpWinSock1.CommandReceived

    ' ''    Try


    ' ''        Select Case SckCommand

    ' ''            Case Is = SET_NICK_NAME

    ' ''                If Me.AvailableClients.ContainsKey(senderSocketID) Then
    ' ''                    With Me.AvailableClients(senderSocketID)
    ' ''                        Me.setClientNickName(senderSocketID, Params)

    ' ''                        REM If someone connects to this guy that is being updated
    ' ''                        If .ConnectedTo IsNot Nothing Then Me.updateClientConnected(.ConnectedTo, Me.AvailableClients(senderSocketID))

    ' ''                    End With
    ' ''                End If

    ' ''                Me.broadcastUsers()

    ' ''            Case Is = CONNECT_ME_TO_USER

    ' ''                Dim ConnectToID As Int32 = CInt(Val(Params))

    ' ''                If ConnectToID = senderSocketID Then

    ' ''                    EpWinSock1.SendMessage("You can not connect to yourself :)", senderSocketID)
    ' ''                    Exit Select
    ' ''                End If

    ' ''                If Not Me.AvailableClients.ContainsKey(ConnectToID) Then

    ' ''                    EpWinSock1.SendMessage("Invalid Client ID. Try search for the client's ID!", senderSocketID)
    ' ''                    Exit Select
    ' ''                End If


    ' ''                With Me.AvailableClients(senderSocketID)

    ' ''                    REM Check if the user is busy
    ' ''                    If Me.AvailableClients(ConnectToID).hasConnection Then

    ' ''                        EpWinSock1.SendMessage("User is busy!!",
    ' ''                                                   senderSocketID)

    ' ''                        Exit Select
    ' ''                    End If




    ' ''                    If .hasConnection Then

    ' ''                        EpWinSock1.SendMessage(
    ' ''                            String.Format(
    ' ''                                                  "You are already connected to {0}-{1}. Disconnect first!",
    ' ''                                                    .ConnectedTo.ID, .ConnectedTo.Nick_Name
    ' ''                                                    ),
    ' ''                                                   senderSocketID
    ' ''                                                   )
    ' ''                        Exit Select
    ' ''                    End If


    ' ''                    Me.updateClientConnected(.this, Me.AvailableClients(ConnectToID))

    ' ''                    ' EpWinSock1.SendCommand(SVR_MSG_CONNECTED_TO_USER, ConnectToID, senderSocketID)

    ' ''                    ''EpWinSock1.SendMessage(
    ' ''                    ''        String.Format(
    ' ''                    ''                              "You are NOW connected to {0}-{1}!",
    ' ''                    ''                                .ConnectedTo.ID, .ConnectedTo.Nick_Name
    ' ''                    ''                                ),
    ' ''                    ''                               senderSocketID
    ' ''                    ''                               )

    ' ''                End With


    ' ''            Case Is = DISCONNECT_ME_FROM_USER


    ' ''                With Me.AvailableClients(senderSocketID)
    ' ''                    If Not .hasConnection Then

    ' ''                        EpWinSock1.SendMessage(
    ' ''                                                  "You are NOT connected to anyone!",
    ' ''                                                   senderSocketID
    ' ''                                                   )
    ' ''                        Exit Select
    ' ''                    End If


    ' ''                    EpWinSock1.SendCommand(SVR_MSG_DISCONNECTED_FROM_USER, String.Empty, senderSocketID)
    ' ''                    EpWinSock1.SendCommand(SVR_MSG_DISCONNECTED_FROM_USER, String.Empty, .ConnectedTo.ID)


    ' ''                    ''EpWinSock1.SendMessage(
    ' ''                    ''        String.Format(
    ' ''                    ''                              "You are NOW Disconnected from {0}-{1}!",
    ' ''                    ''                                .ConnectedTo.ID, .ConnectedTo.Nick_Name
    ' ''                    ''                                ),
    ' ''                    ''                               senderSocketID
    ' ''                    ''                               )


    ' ''                    ''EpWinSock1.SendMessage(
    ' ''                    ''        String.Format(
    ' ''                    ''                              "You are NOW Disconnected from {0}-{1}!",
    ' ''                    ''                                .ConnectedTo.ConnectedTo.ID, .ConnectedTo.ConnectedTo.Nick_Name
    ' ''                    ''                                ),
    ' ''                    ''                               .ConnectedTo.ID
    ' ''                    ''                               )


    ' ''                    Me.updateClientConnected(.ConnectedTo, Nothing)
    ' ''                    Me.updateClientConnected(.this, Nothing)



    ' ''                End With





    ' ''        End Select


    ' ''    Catch ex As Exception

    ' ''    End Try

    ' ''End Sub

    Dim SentTimeStart As Date

    Private Sub EpWinSock1_MessageReceived(ByVal SckMessage As String, ByVal senderSocketID As Integer) Handles EpWinSock1.MessageReceived

        Try
            REM always display it on server
            Me.appendString(String.Format("Client[{0}] ", senderSocketID), SckMessage)

            Me.SentTimeStart = Now
            REM EpWinSock1.SendFile("C:\Windows\Temp\sckProject.zip", senderSocketID)

            EpWinSock1.SendFile("C:\Windows\Temp\Koala.jpg", senderSocketID)
            REM EpWinSock1.SendFile("C:\Windows\Temp\fish__small.jpg", senderSocketID)
            REM EpWinSock1.SendFile("C:\Windows\Temp\Test.txt", senderSocketID)
            REM EpWinSock1.SendCommand(New UserCommandPackage("FunnyCommand", UserCommandPackage.DEFAULT____PARAM___DELIMITER, "Cool"), senderSocketID)
            ''Threading.Thread.Sleep(5000)
            ''EpWinSock1.SendMessage("Okay, why?", senderSocketID)


            ''REM Any Message received from a connected client should be relayed
            ''If Me.AvailableClients.ContainsKey(senderSocketID) Then

            ''    With Me.AvailableClients(senderSocketID)

            ''        If .hasConnection Then

            ''            REM I can send the client info .. but it is not neccessary since each client has the list of
            ''            REM available clients
            ''            EpWinSock1.SendCommand(CHAT_MESSAGE, SckMessage, .ConnectedTo.ID)

            ''            REM always display it on server
            ''            Me.appendString(String.Format("Client[{0}] ", senderSocketID), SckMessage)
            ''        End If


            ''    End With

            ''End If


        Catch ex As Exception

        End Try


    End Sub

    Private Sub EpWinSock1_ReceiveFileTransferProgress(ByVal FileName As String, ByVal TotalBytesReceived As Integer, ByVal FileBytesSize As Integer, ByVal SenderSocketID As Integer) Handles EpWinSock1.ReceiveFileTransferProgress
        prgReceivingIndicator.Maximum = FileBytesSize
        prgReceivingIndicator.Value = TotalBytesReceived
    End Sub

    Private Sub EpWinSock1_SendFileTransferProgress(ByVal FileName As String, ByVal TotalBytesSent As Integer, ByVal FileBytesSize As Integer, ByVal SenderSocketID As Integer) Handles EpWinSock1.SendFileTransferProgress
        ''prgSendingIndicator.Maximum = FileBytesSize
        ''prgSendingIndicator.Value = TotalBytesSent


        ''Application.DoEvents()

    End Sub

    Private Sub EpWinSock1_SendingCancelled(senderSocketID As Integer) Handles EpWinSock1.SendingCancelled
        prgSendingIndicator.Value = 0
    End Sub



    Private Sub EpWinSock1_SocketErrorMessage(ByVal SckMessage As String) Handles EpWinSock1.SocketErrorMessage

        Me.appendString("Error Message:", SckMessage)

        ''Me.lstServerResponse.Items.Add(
        ''   String.Format("Error Message: {0}", SckMessage)
        ''   )

        ''Me.lstServerResponse.SelectedIndex = Me.lstServerResponse.Items.Count - 1

    End Sub

    Private Sub EpWinSock1_SocketLogMessage(ByVal sckLog As String) Handles EpWinSock1.SocketLogMessage

        Me.appendString("Log:", sckLog)

        ''Me.lstServerResponse.Items.Add(
        ''   String.Format("Log: {0}", sckLog)
        ''   )
        ''Me.lstServerResponse.SelectedIndex = Me.lstServerResponse.Items.Count - 1

    End Sub

    Private Sub EpWinSock1_SocketStateChanged(ByVal CurrentState As SocketConnectionState, ByVal DebugMessage As String) Handles EpWinSock1.SocketStateChanged

        Me.appendString("Status Changed:", DebugMessage)

        ''Me.lstServerResponse.Items.Add(
        ''    String.Format("Status Changed: {0}", DebugMessage)
        ''    )
        ''Me.lstServerResponse.SelectedIndex = Me.lstServerResponse.Items.Count - 1

    End Sub

    Private Sub cmdDisconnect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdDisconnect.Click
        Me.EpWinSock1.Stop()
    End Sub



    ''Private Sub btnGoAbsoluteOFFLine_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
    ''    EpWinSock1.StopServerAbsolutely()
    ''End Sub

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

    Private Sub lsvAvailableClients_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lsvAvailableClients.SelectedIndexChanged

        If lsvAvailableClients.SelectedItems.Count > 0 Then

            Debug.Print(EpWinSock1.getClient(
                        CInt(Val(
                                lsvAvailableClients.SelectedItems(0).Text
                                ))
                        ).ToString()
                    )



        End If

    End Sub

    Private Sub btnCancelSend_Click(sender As Object, e As EventArgs) Handles btnCancelSend.Click
        If EpWinSock1.ConnectedClients.Count > 0 Then EpWinSock1.CancelSending(EpWinSock1.ConnectedClients(0))
    End Sub
End Class
