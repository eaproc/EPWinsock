Option Explicit On
Option Strict On

Imports System.ComponentModel
Imports System.Net
Imports System.Net.Sockets


Imports EPWinSock.v4.Utilities
Imports EPWinSock.v4.DataXchange.Packets.Layer2Packet
Imports EPWinSock.v4.DataXchange.StringPackages


Namespace NET
    <System.ComponentModel.DefaultEvent("SocketStateChanged")> _
    Public Class ServerSocket
        Inherits BASE


#Region "Constructors"

        Sub New()
            Me.New(Nothing)
        End Sub

        Sub New(ByVal mContainer As IContainer)

            MyBase.New(mContainer)
            Me.ClientsOnSever = New SortedList(Of Integer, ClientSocketWrapper)(Me.Maximum_Connection_Allowed)

            ''BASE.____IsDebugMode = True



        End Sub


#End Region


#Region "Enums"

        Public Enum NICListeningPolicy
            ListenOnAllNICs
            ListenOnSpecifiedNIC
            ListenOnLocalPCOnly
        End Enum


#End Region



#Region "Events"


        ''' <summary>
        ''' Raises when a new client disconnects
        ''' </summary>
        ''' <param name="SocketID "></param>
        ''' <remarks></remarks>
        Public Event AClientDisconnected(ByVal SocketID As Integer)

        ''' <summary>
        ''' Raises the AClientDisConnected under the Parent Control Thread
        ''' </summary>
        ''' <param name="SocketID "></param>
        ''' <remarks></remarks>
        Private Sub Try_AClientDisConnected(ByVal SocketID As Integer)

            If Me.HandleCrossThreading Then
                REM Just making sure the parent control is ok before we do anything
                If Me.ParentControl Is Nothing Then Return
                If Me.ParentControl.IsDisposed Then Return
                If Not Me.ParentControl.Created Then Return

                Me.ParentControl.Invoke(Sub() Me.Invoke_AClientDisConnected(SocketID))
            Else
                Me.Invoke_AClientDisConnected(SocketID)
            End If
        End Sub

        ''' <summary>
        ''' The real invoke inner_Call
        ''' </summary>
        ''' <param name="SocketID"></param>
        ''' <remarks></remarks>
        Private Sub Invoke_AClientDisConnected(ByVal SocketID As Integer)
            RaiseEvent AClientDisconnected(SocketID)
        End Sub



        ''' <summary>
        ''' Raises when a new client connects
        ''' </summary>
        ''' <param name="SocketID "></param>
        ''' <remarks></remarks>
        Public Event AClientConnected(ByVal SocketID As Integer)

        ''' <summary>
        ''' Raises the ClientConnected under the Parent Control Thread
        ''' </summary>
        ''' <param name="SocketID "></param>
        ''' <remarks></remarks>
        Private Sub Try_AClientConnected(ByVal SocketID As Integer)
            If Me.HandleCrossThreading Then
                REM Just making sure the parent control is ok before we do anything
                If Me.ParentControl Is Nothing Then Return
                If Me.ParentControl.IsDisposed Then Return
                If Not Me.ParentControl.Created Then Return

                Me.ParentControl.Invoke(Sub() Me.Invoke_AClientConnected(SocketID))
            Else
                Me.Invoke_AClientConnected(SocketID)
            End If

        End Sub

        ''' <summary>
        ''' The real invoke inner_Call
        ''' </summary>
        ''' <param name="SocketID"></param>
        ''' <remarks></remarks>
        Private Sub Invoke_AClientConnected(ByVal SocketID As Integer)
            RaiseEvent AClientConnected(SocketID)
        End Sub







#End Region


#Region "Properties"


#Region "Private"

        ''' <summary>
        ''' For now I will keep this private
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Property Maximum_Connection_Allowed As Integer = 200


        ''' <summary>
        ''' Keep the list of Connected Clients for Fast indexing
        ''' </summary>
        ''' <remarks></remarks>
        Private ClientsOnSever As SortedList(Of Integer, ClientSocketWrapper) = Nothing



#End Region


        REM Makes it in Category when it is in category view
        ''' <summary>
        ''' If this is a server .. then fetch the list of connected clients SocketIDs
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        < _
      System.ComponentModel.Category("RunTime"), _
      System.ComponentModel.DisplayName("ConnectedClients"), _
      System.ComponentModel.Description("Fetches the list of connected clients SocketIDs"), _
      System.ComponentModel.Browsable(False)
          > _
        Public ReadOnly Property ConnectedClients As List(Of Integer)
            Get
                Return Me.ClientsOnSever.Keys.ToList
            End Get
        End Property


        ''' <summary>
        ''' Fetches the Unique ID of this Socket
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        < _
    System.ComponentModel.Category("RunTime"), _
    System.ComponentModel.DisplayName("SocketID"), _
    System.ComponentModel.Description("Fetches the Unique ID of this Socket"), _
    System.ComponentModel.Browsable(False)
      > _
        Public Overrides ReadOnly Property SocketID As Integer
            Get
                Try

                    If Me.sckSocket Is Nothing Then Return 0
                    Return Me.sckSocket.Handle.ToInt32

                Catch ex As Exception

                End Try

                Return 0
            End Get
        End Property



        Private _______NICPolicy As NICListeningPolicy
        ''' <summary>
        ''' Controls which NIC is used by this socket
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        < _
    System.ComponentModel.Category("Connection"), _
    System.ComponentModel.DisplayName("NICPolicy"), _
    System.ComponentModel.Description("Controls which NIC is used by this socket"), _
    System.ComponentModel.Browsable(True), _
    System.ComponentModel.DefaultValue(NICListeningPolicy.ListenOnAllNICs)
      > _
        Public Property NICPolicy As NICListeningPolicy
            Get
                Return Me._______NICPolicy
            End Get
            Set(value As NICListeningPolicy)
                _______NICPolicy = value
            End Set
        End Property



        Private _______NICIPAddress As String
        ''' <summary>
        ''' Uses this NIC if NIC Policy is set to specified
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        < _
    System.ComponentModel.Category("Connection"), _
    System.ComponentModel.DisplayName("NICIPAddress"), _
    System.ComponentModel.Description("Uses this NIC if NIC Policy is set to specified"), _
    System.ComponentModel.Browsable(True), _
    System.ComponentModel.DefaultValue("")
      > _
        Public Property NICIPAddress As String
            Get
                Return Me._______NICIPAddress
            End Get
            Set(value As String)
                _______NICIPAddress = value
            End Set
        End Property



#End Region


#Region "Methods"

#Region "Private"

        ''' <summary>
        ''' Make Server Available For Connections
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Server Listens on All IPs Available</remarks>
        Private Function ServerGoOnline() As Boolean
            'We are running on port 1300
            '

            '   *Possible Errors*
            '   Socket 1300 Already in USE
            '   Socket is Already in Listening State
            '   Invalid IP Address for the Server


            '   
            '   Different Form of Connection
            '   
            '   IPAddress.Any will listen on any IP Addresses assigned to the PC. For example, if I am connected to the network via wireless 
            '   and wired, there would be two IP Addresses assigned. This means that I would listen for requests on both IP Addresses. '
            '   If I would take the IP of say the wired then I could only receive requests from that NIC. 
            '
            '   The loopback address is only on the current computer. It never hits the network. 
            '   There is a dedicated loop address according to the IPv4 and 6 standards.

            Try
                '            Dim IEndpt As IPEndPoint = New IPEndPoint(IPAddress.Parse(LocalIP), Me.Port)
                'Dim IEndpt As IPEndPoint = New IPEndPoint(IPAddress.Parse("192.168.56.101"), Me.Port)
                Dim IEndpt As IPEndPoint = New IPEndPoint(IPAddress.Any, Me.Port)

                If Me.NICPolicy = NICListeningPolicy.ListenOnSpecifiedNIC Then _
                    IEndpt = New IPEndPoint(IPAddress.Parse(Me.NICIPAddress), Me.Port)

                If Me.NICPolicy = NICListeningPolicy.ListenOnLocalPCOnly Then _
                   IEndpt = New IPEndPoint(IPAddress.Parse(LOCAL_IP), Me.Port)



                Me.sckSocket = New Socket(IEndpt.AddressFamily, SocketType.Stream, Me.getSelectedProtocol())
                Me.sckSocket.Bind(IEndpt)
                '
                'Allow Maximum 100 Pending Connections
                '
                Me.sckSocket.Listen(100)

                REM Continue or Begin to Accept Incoming Connections Asychronously
                Return ServerKeepAccepting()

            Catch ex As Exception

                Me.Try_SocketErrorMessage("Undocumented Error: " & ex.StackTrace)

            End Try

            Return False
        End Function


        ''' <summary>
        ''' Set Server to Keep Accepting Pending Connection Requests
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function ServerKeepAccepting() As Boolean
            '   *Possible Errors*
            '   Begin Accept Already Opened
            '
            '
            Try

                Me.sckSocket.BeginAccept(New AsyncCallback(AddressOf ConnectionRequestReceivedFromAClient), Me.sckSocket)

                Try_SocketLogMessage("Server is waiting for client connection request")

                Return True
            Catch ex As Exception

                Me.Try_SocketErrorMessage("Undocumented Error: " & ex.StackTrace)

                Return False
            End Try
        End Function



        Dim LOCK_ConnectionRequestReceivedFromAClient As New Object REM Allo a thread per time in this method
        ''' <summary>
        ''' Connection Request Received From A Client
        ''' </summary>
        ''' <param name="ar">The Server Object [Socket]</param>
        ''' <remarks>This is on a different thread away from the host</remarks>
        Private Sub ConnectionRequestReceivedFromAClient(ByVal ar As IAsyncResult)
            REM If server socket go offline .. It will try to close the beginAccept
            REM method by entering here

            If Me.sckSocket Is Nothing OrElse Me.ConnectionState = SocketConnectionState.Disconnected Then Return

            SyncLock Me.LOCK_ConnectionRequestReceivedFromAClient


                Try
                    ' Get the listener that handles the client request. 
                    Dim listener As Socket = CType(ar.AsyncState, Socket)

                    ' End the operation and display the received data on the 
                    'console. 
                    '

                    '
                    'ClientSocket is Temporary and is Not Indexed . . .
                    ' When system name is gotten, it will be indexed
                    Dim ClientSocket As Socket = listener.EndAccept(ar)

                    REM Temporary here Socket is no more accepting connections from other client
                    REM We will be using socket handle here to uniquely identify each client
                    REM Send the SocketHandle to the client .. Incase it needs it
                    REM add the client to collection



                    '
                    Me.Try_SocketLogMessage("A client connected!")

                    '
                    ' Start Receiving From the Client, Send ID to Client
                    '
                    Dim ClientReadingClass As ClientSocketWrapper =
                        New ClientSocketWrapper(ClientSocket, Me.PacketSize, Me.InActivityTimeout,
                                                                AddressOf MyBase.ProcessDataReceived,
                                                                AddressOf MyBase.ProcessFileReceived,
                                                                AddressOf Me.DoClientWentOffline)

                    Me.ClientKeepReceivingMessage(ClientReadingClass)


                    REM Send the connecting client's ID
                    '' '' ''SocketHelper.SendCommand(ClientReadingClass.ClientSocket,
                    '' '' ''               ClientReadingClass.SocketKey.ToString(),
                    '' '' ''               CMD_HERE_IS_YOUR_ID)

                    ClientReadingClass.SendMessage(New SocketCommandPackage(CMD_HERE_IS_YOUR_ID, SocketCommandPackage.DEFAULT____PARAM___DELIMITER,
                                                                             ClientReadingClass.SocketKey.ToString())
                                                                         )


                    Me.Try_SocketLogMessage("Sent ID to Client: " & ClientReadingClass.SocketKey)


                    REM Add to Client List
                    Me.ClientsOnSever.Add(ClientReadingClass.SocketKey, ClientReadingClass)

                    Me.Try_SocketLogMessage("Total Connections: " & Me.ClientsOnSever.Count)



                    '
                    ' If I want it to continue accepting .. I have to turn on the begin accept again
                    Me.ServerKeepAccepting()



                Catch ex As ObjectDisposedException

                    REM This part will not be called again

                    '
                    'Currently, I cant detect a way to know the server closed than exception
                    REM Server socket is shutting down

                    Me.Try_SocketErrorMessage("Server has gone offline")



                Catch ex As Exception


                End Try


            End SyncLock



        End Sub 'Client Connection Request Recieved


        ''' <summary>
        ''' Stream is received From Client.
        ''' </summary>
        ''' <param name="ar"></param>
        ''' <remarks>This is on a different thread away from the host</remarks>
        Private Sub MessageReceivedFromClient(ByVal ar As IAsyncResult)

            'Information Received here is on another thread

            REM Make it able to captivate message greater than 1024 kb by receiving asyncronously until no byte is available
            Dim ClientReadingClass As ClientSocketWrapper = CType(ar.AsyncState, ClientSocketWrapper)
            Dim ClientSocket As Socket = ClientReadingClass.ClientSocket


            '
            '
            'If Client has shutdown It disconnects before calling endreceive function itself
            'Most times we are not that fast to catch it so it goes down as exception

            Try

                If ClientReadingClass IsNot Nothing AndAlso
                  Not ClientReadingClass.isDisposed AndAlso ClientSocket IsNot Nothing Then


                    'Say We Have Received the bytes and Stored it in Buffer [ClientReadingClass]
                    Dim ByteSizeRecieved As Integer = ClientSocket.EndReceive(ar)

                    'If any byte was received
                    If ByteSizeRecieved > 0 Then


                        ''Call ProcessServerCommand(
                        ''            ClientReadingClass,
                        ''            SocketHelper.ReceiveCommand(
                        ''                ByteRecieved,
                        ''                ClientReadingClass
                        ''                )
                        '')


                        If Not ClientReadingClass.NotifyReceived(CUShort(ByteSizeRecieved), AddressOf Me.Try_ReceivingCancelled__Async) Then
                            REM Protocol brigded
                            Return
                        End If

                        'Keep Receiving from client
                        ClientKeepReceivingMessage(ClientReadingClass)


                    End If

                End If

            Catch ex As SocketException
                REM I assumed if error occurred here it means a client is disconnecting
                'ex returns nothing
                'If ex.SocketErrorCode = SocketError.ConnectionReset Then
                'Client Application is Off or Client System Shut Down
                'Refer to shutdown process

                'End If
                'Clean Up SocketClass and Remove Item on List
                'First Adjust the counter
                ' ''Me.aClientIsLeaving(ClientReadingClass.SocketKey)

                REM Client listening for messages sockets closed

                ' Debug.Print("Message Received From Client: " & ex.StackTrace)
                Me.DoClientWentOffline(ClientReadingClass)



            Catch ex As Exception

                Me.Try_SocketErrorMessage("Undocumented Error: " & ex.StackTrace)



            End Try
        End Sub 'Read_Callback



        ''' <summary>
        ''' Set Client Socket To keep receiving messages
        ''' </summary>
        ''' <param name="ClientClass"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function ClientKeepReceivingMessage(
                                                ByRef ClientClass As ClientSocketWrapper
                                                ) As Boolean

            Try
                ClientClass.ClientSocket.BeginReceive(ClientClass.SocketBucketReceiver, 0, ClientClass.Maximum_Buffer_Size, SocketFlags.None,
                                  New AsyncCallback(AddressOf MessageReceivedFromClient),
                                  ClientClass)

                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function



        Private Function ServerGoOffline() As Boolean
            '   Since am using lsvMainClients to store the connected clients directly
            '   Saving their socket information in each item

            '   So if Server is to go offline, First 
            '   Block incoming and outgoing transaction on server
            '   Stop listening

            'I am thinking of not disposing clients here even though server is shutdown
            'Because they have nothing in common. Server Shutdown only means no more accepting new connection
            '
            'Clients can be shutdown from options on client or when it goes off
            '
            '' ''   Now Remove and Dispose Each item on the list
            '' ''       On Dispose of each item, let them dispose their socket indexes

            REM Incase an error occurs .. restore previous state
            Dim PrevState As SocketConnectionState = Me.ConnectionState

            Try

                If Me.sckSocket IsNot Nothing Then
                    With Me.sckSocket
                        '' Only use shutdown for a connected Socket. Not a listening socket
                        '.Shutdown(SocketShutdown.Both)
                        Me._ConnectionState = SocketConnectionState.Disconnected REM Because of async method
                        .Close()



                    End With

                End If

                Return True

            Catch ex As Exception
                Me._ConnectionState = PrevState
            End Try
            Return False
        End Function

        '' ''' <summary>
        '' ''' Handles the communication data between server and client and decides if it needs to relay it to user [as what to do]
        '' ''' </summary>
        '' ''' <param name="clientReadingClass"></param>
        '' ''' <param name="MessageReceived"></param>
        '' ''' <remarks></remarks>
        ''Private Sub ProcessServerCommand(ByRef clientReadingClass As ClientSocketWrapper,
        ''                           ByVal MessageReceived As String()
        ''                           )
        ''    'Be Consistent With Your Command Name Because It is Case Sensitive
        ''    REM {{EPCommand}}{{ProgrammerCommand}}{{MessageContent}}

        ''    REM I have to check how i process the message because it could be a file and not a string
        ''    REM For now we are dealing with strings
        ''    '' '' ''If MessageReceived IsNot Nothing AndAlso MessageReceived.Count > 0 Then

        ''    '' '' ''    Select Case MessageReceived.Count

        ''    '' '' ''        Case Is = 1
        ''    '' '' ''            REM Raw Message not from internal
        ''    '' '' ''            Me.Try_SocketLogMessage(
        ''    '' '' ''       String.Format(
        ''    '' '' ''           "Client {0}: {1}", clientReadingClass.SocketKey, MessageReceived(0)
        ''    '' '' ''           )
        ''    '' '' ''       )
        ''    '' '' ''            'SocketHelper.SendRawMessage(clientReadingClass.ClientSocket, "HTTP/1.0 404 Not Found!")
        ''    '' '' ''            REM Ignore for now
        ''    '' '' ''            REM This probably a different access
        ''    '' '' ''            'SocketHelper.SendRawMessage(clientReadingClass.ClientSocket, "Language NOT Understood!")
        ''    '' '' ''            Return
        ''    '' '' ''        Case Is = 2
        ''    '' '' ''            REM Normal Message Internally is just of 2 sections
        ''    '' '' ''            Me.Try_SocketLogMessage(
        ''    '' '' ''       String.Format(
        ''    '' '' ''           "Client {0}: {1}: {2}", clientReadingClass.SocketKey, MessageReceived(0), MessageReceived(1)
        ''    '' '' ''           )
        ''    '' '' ''       )
        ''    '' '' ''        Case Else

        ''    '' '' ''            Me.Try_SocketLogMessage(
        ''    '' '' ''       String.Format(
        ''    '' '' ''           "Client {0}: Message Arguments too much [args: {1}] ", clientReadingClass.SocketKey, MessageReceived.Count
        ''    '' '' ''           )
        ''    '' '' ''       )
        ''    '' '' ''            REM Impossible or crazy data
        ''    '' '' ''            Return
        ''    '' '' ''    End Select



        ''    '' '' ''    REM Check where it belongs
        ''    '' '' ''    Select Case MessageReceived(0)

        ''    '' '' ''        Case Is = SIMPLE_MESSAGE_COMMAND

        ''    '' '' ''            Me.Try_MessageReceived(MessageReceived(1), clientReadingClass.SocketKey)


        ''    '' '' ''        Case Is = SIMPLE_COMMAND
        ''    '' '' ''            Dim innerMSG() As String = Split(MessageReceived(1), SocketHelper.DelimiterUsedForOutterCommands)

        ''    '' '' ''            Me.Try_CommandReceived(innerMSG(0), innerMSG(1), clientReadingClass.SocketKey)


        ''    '' '' ''        Case Is = CMD_CLIENT_INFO

        ''    '' '' ''            clientReadingClass.ClientInfo = New PCInfo(MessageReceived(1))
        ''    '' '' ''            REM Postponed this alert till the full info of the client is received
        ''    '' '' ''            Me.Try_AClientConnected(clientReadingClass.SocketKey)



        ''    '' '' ''    End Select





        ''    '' '' ''Else
        ''    '' '' ''    Me.Try_SocketLogMessage(
        ''    '' '' ''        String.Format(
        ''    '' '' ''            "Client {0}: Empty Message", clientReadingClass.SocketKey
        ''    '' '' ''            )
        ''    '' '' ''        )
        ''    '' '' ''End If

        ''End Sub

        Protected Overrides Sub ProcessSocketCommandReceived(clientReadingClass As ClientSocketWrapper, sckCMD As SocketCommandPackage)

            If BASE.IsDebugMode Then BASE.LocalLogger.Write("Got here with " & sckCMD.Parameters(0))

            Select Case sckCMD.CommandName

                Case Is = CMD_CLIENT_PC_INFO
                    clientReadingClass.ClientInfo = New PCInfo(CType(sckCMD.Parameters(0), CommandPackage))
                    REM Postponed this alert till the full info of the client is received
                    Me.Try_AClientConnected(clientReadingClass.SocketKey)

                Case Is = CMD_FILE_TRANSFER_BEGIN
                    clientReadingClass.NotifyFileTransferBegins(sckCMD,
                                                                 AddressOf MyBase.Try_SocketErrorMessage,
                                                                AddressOf MyBase.Try_ReceiveFileTransferProgress)



            End Select

        End Sub




        ''' <summary>
        ''' Perform Client Went offline
        ''' </summary>
        ''' <param name="ClientReadingClass"></param>
        ''' <remarks></remarks>
        Private Sub DoClientWentOffline(ByVal ClientReadingClass As ClientSocketWrapper)
            Me.Try_SocketLogMessage(
                  String.Format(
                      "Client {0}: is shutting down",
                      ClientReadingClass.SocketKey
                      )
                  )


            REM Dispose Client and remove from bank list
            Me.ClientsOnSever.Remove(ClientReadingClass.SocketKey)

            Me.Try_AClientDisConnected(ClientReadingClass.SocketKey)


            ClientReadingClass.Dispose()
            ClientReadingClass = Nothing

        End Sub


#End Region

#Region "Public"


        ''' <summary>
        ''' Disconnect the Specified Client
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub DisconnectClient(ByVal ClientSocketID As Integer)

            Try

                If Not Me.ConnectedClients.Contains(SocketID) Then

                    Try_SocketErrorMessage("Client Not Found!")

                    Return

                End If



                Me.DoClientWentOffline(Me.ClientsOnSever(ClientSocketID))

            Catch ex As Exception

                Me.Try_SocketErrorMessage("DisconnectClient: " & ex.StackTrace)

            End Try
        End Sub


        ''' <summary>
        ''' Stops Server and Disconnects all Clients
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub StopServerAbsolutely()

            Try

                Me.Stop()


                For intC As Byte = 1 To CByte(Me.ClientsOnSever.Count)

                    Me.DoClientWentOffline(Me.ClientsOnSever.ElementAt(0).Value)
                Next


            Catch ex As Exception

                Me.Try_SocketErrorMessage("StopServerAbsolutely: " & ex.StackTrace)

            End Try
        End Sub




        ''' <summary>
        ''' Send a Simple Message of Maximum 1014 characters over LAN
        ''' </summary>
        ''' <param name="Message">Message to send</param>
        ''' <param name="SocketID">Socket ID to Send to .. It is only neccessary if this component is running in server mode</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function SendMessage(ByVal Message As String,
                                    Optional ByVal SocketID As Integer = 0) As Boolean


            If Not Me.ConnectedClients.Contains(SocketID) Then

                Try_SocketErrorMessage("Client Not Found!")

                Return False

            End If


            If Not Me.ClientsOnSever(SocketID).SendMessage(Message) Then
                Try_SocketErrorMessage("Invalid Message or Socket is busy")
                Return False

            End If

            Return True

        End Function

        ''' <summary>
        ''' Send a Simple Command
        ''' </summary>
        ''' <param name="SocketID">Socket ID to Send to .. It is only neccessary if this component is running in server mode</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SendCommand(ByVal pktCommand As UserCommandPackage,
                                    ByVal SocketID As Integer) As Boolean


            If Not Me.ConnectedClients.Contains(SocketID) Then

                Try_SocketErrorMessage("Client Not Found!")

                Return False

            End If
            REM So for a simple message am appending 10bytes which is Message with delimiter which is 3bytes currently
            REM So User's message should not be more than Me.PacketSize - 10 characters

            If Not Me.ClientsOnSever(SocketID).SendMessage(pktCommand) Then
                Try_SocketErrorMessage("Invalid Message or Socket is busy")
                Return False

            End If

            Return True
            'Dim Message As String = CommandTitle & SocketHelper.DelimiterUsedForOutterCommands & Params
            'Return Me.SendMessage(SIMPLE_COMMAND, Message, Me.ClientsOnSever(SocketID).ClientSocket)

        End Function



        ''' <summary>
        ''' Send a Raw Data
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SendData(ByVal pktData As Byte(),
                                    ByVal SocketID As Integer) As Boolean


            If Not Me.ConnectedClients.Contains(SocketID) Then

                Try_SocketErrorMessage("Client Not Found!")

                Return False

            End If

            If Not Me.ClientsOnSever(SocketID).SendData(pktData) Then

                Try_SocketErrorMessage("Invalid Data or Socket is busy")
                Return False

            End If

            Return True

        End Function


        ''' <summary>
        ''' Send a file to client
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SendFile(ByVal FileFullPath As String,
                                  ByVal SocketID As Integer) As Boolean


            If Not Me.ConnectedClients.Contains(SocketID) Then

                Try_SocketErrorMessage("Client Not Found!")

                Return False

            End If

            If Not MyBase.CanSendData(Me.ClientsOnSever(SocketID)) Then Return False

            If Not Me.ClientsOnSever(SocketID).SendFile(FileFullPath, AddressOf MyBase.Try_SocketErrorMessage,
                                                 AddressOf Me.Try_SendFileTransferProgress,
                                                 AddressOf Me.Try_FileSent,
                                                 AddressOf Me.Try_SendingCancelled) Then

                Try_SocketErrorMessage("Invalid Data or Socket is busy")
                Return False

            End If

            Return True

        End Function


        ''' <summary>
        ''' Fetch stored client details
        ''' </summary>
        ''' <param name="ClientID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function getClient(ByVal ClientID As Int32) As PCInfo

            If Me.ConnectedClients.Contains(ClientID) Then Return Me.ClientsOnSever(ClientID).ClientInfo
            Return Nothing

        End Function



        ''' <summary>
        ''' Cancel current sending operation
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub CancelSending(ByVal ClientID As Int32)
            If Me.ConnectedClients.Contains(ClientID) Then Me.ClientsOnSever(ClientID).CancelSending()
        End Sub



        ''' <summary>
        ''' Starts the socket Synchronously
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub [Start]()

            If Me.HandleCrossThreading AndAlso Not Me.CanInvokeEvents Then Return


            If Me.ConnectionState = SocketConnectionState.Disconnected Then
                Try_SocketLogMessage("Server is going online ...")
                Try_SocketStateChanged(SocketConnectionState.Connecting, "Trying to listen on port " & Me.Port)

                If Not Me.ServerGoOnline() Then
                    Try_SocketLogMessage("Server Failed to go online ...")
                    Me._ConnectionState = SocketConnectionState.Disconnected
                    Try_SocketStateChanged(Me.ConnectionState, "Server could not listen on port " & Me.Port)

                Else

                    Me._ConnectionState = SocketConnectionState.Connected
                    Try_SocketStateChanged(Me.ConnectionState, "Server is listening on port " & Me.Port)
                    Try_SocketLogMessage("Server is online")

                End If

            Else

                REM And it is going to remain in it's current form
                Try_SocketErrorMessage("Server is NOT disconnected!")


            End If


        End Sub

        ''' <summary>
        ''' Stops the Socket
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub [Stop]()


            REM It not necessary under stop because you can't change it if it is connected


            If Not Me.ConnectionState = SocketConnectionState.Disconnected Then
                If Me.ServerGoOffline Then

                    Try_SocketErrorMessage("Server is now offline")
                    Me._ConnectionState = SocketConnectionState.Disconnected
                    Try_SocketStateChanged(Me.ConnectionState, "Server Socket Went offline")
                Else

                    Try_SocketErrorMessage("Server could not shutdown")

                End If
            Else

                Try_SocketErrorMessage("Server is already Offline")


            End If



        End Sub



#End Region

#End Region



    End Class

End Namespace