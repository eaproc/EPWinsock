Option Explicit On
Option Strict On

Imports System.ComponentModel
Imports System.Net
Imports System.Net.Sockets

Imports EPWinSock.v4.Utilities
Imports EPWinSock.v4.DataXchange.Packets.Layer2Packet
Imports EPWinSock.v4.DataXchange.StringPackages
Imports CODERiT.Logger.v._3._5.Exceptions


Namespace NET
    <System.ComponentModel.DefaultEvent("SocketStateChanged")> _
    Public Class ClientSocket
        Inherits BASE


#Region "Constructors"

        Sub New()
            Me.New(Nothing)
        End Sub

        Sub New(ByVal mContainer As IContainer)

            MyBase.New(mContainer)
            'BASE.____IsDebugMode = True

        End Sub


#End Region



#Region "Events"



        ''' <summary>
        ''' Raises when a New ID has been received from Server
        ''' </summary>
        ''' <remarks></remarks>
        Public Event SocketIDChanged()

        ''' <summary>
        ''' Raises the SocketIDChanged under the Parent Control Thread
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub Try_SocketIDChanged()
            If Me.HandleCrossThreading Then
                REM Just making sure the parent control is ok before we do anything
                If Me.ParentControl Is Nothing Then Return
                If Me.ParentControl.IsDisposed Then Return
                If Not Me.ParentControl.Created Then Return

                Me.ParentControl.Invoke(Sub() Me.Invoke_SocketIDChanged())
            Else
                Me.Invoke_SocketIDChanged()
            End If



        End Sub

        ''' <summary>
        ''' The real invoke inner_Call
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub Invoke_SocketIDChanged()
            RaiseEvent SocketIDChanged()
        End Sub


#End Region



#Region "Properties"


#Region "Private"

        ''' <summary>
        ''' I will enclose the sckSocket If am using it for client
        ''' </summary>
        ''' <remarks></remarks>
        Private sckSocketContainer As ClientSocketWrapper = Nothing

#End Region


        ''' <summary>
        ''' If this is a client .. Then you need the server IP to Connect to Server
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        < _
      System.ComponentModel.Category("Client Connection"), _
      System.ComponentModel.DisplayName("ServerIP"), _
      System.ComponentModel.Description("Enter the Server IP Address to Connect to If this is a Client Socket"), _
       System.ComponentModel.DefaultValue(LOCAL_IP)
          > _
        Public Property ServerIP As String = LOCAL_IP

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

                    If Me.sckSocket Is Nothing OrElse Me.sckSocketContainer.ClientInfo Is Nothing Then Return 0

                    Return Me.sckSocketContainer.ClientInfo.ID


                Catch ex As Exception

                End Try

                Return 0
            End Get
        End Property

        ''' <summary>
        ''' Indicates if this socket is connected to server and has received its unique id from server
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        < _
    System.ComponentModel.Category("RunTime"), _
    System.ComponentModel.DisplayName("isFullyAttached"), _
    System.ComponentModel.Description("Indicates if this socket is connected to server and has received its unique id from server"), _
    System.ComponentModel.Browsable(False)
      > _
        Public ReadOnly Property isFullyAttached As Boolean
            Get
                Return Me.SocketID <> 0
            End Get
        End Property

        ''' <summary>
        ''' Fetches the Full Details of the device this socket is attached to
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        < _
    System.ComponentModel.Category("RunTime"), _
    System.ComponentModel.DisplayName("SocketDeviceInfo"), _
    System.ComponentModel.Description("Fetches the Full Details of the device this socket is attached to"), _
    System.ComponentModel.Browsable(False)
      > _
        Public ReadOnly Property SocketDeviceInfo As PCInfo
            Get
                If Me.isFullyAttached Then Return Me.sckSocketContainer.ClientInfo
                Return Nothing
            End Get
        End Property


#End Region




#Region "Methods"


#Region "Public"

        ''' <summary>
        ''' Send a Simple Message over LAN
        ''' </summary>
        ''' <param name="Message">Message to send</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function SendMessage(ByVal Message As String) As Boolean

            If Not MyBase.CanSendData(Me.sckSocketContainer) Then Return False

            If Not Me.sckSocketContainer.SendMessage(Message) Then
                Try_SocketErrorMessage("Invalid Message or Socket is busy")
                Return False

            End If

            Return True

        End Function

        ''' <summary>
        ''' Send a Simple Command
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SendCommand(ByVal pktCommand As UserCommandPackage) As Boolean

            If Not MyBase.CanSendData(Me.sckSocketContainer) Then Return False

            If Not Me.sckSocketContainer.SendMessage(pktCommand) Then

                Try_SocketErrorMessage("Invalid Message or Socket is busy")
                Return False

            End If

            Return True

        End Function


        ''' <summary>
        ''' Send a Raw Data
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SendData(ByVal pktData As Byte()) As Boolean

            If Not MyBase.CanSendData(Me.sckSocketContainer) Then Return False

            If Not Me.sckSocketContainer.SendData(pktData) Then

                Try_SocketErrorMessage("Invalid Data or Socket is busy")
                Return False

            End If

            Return True

        End Function


        ''' <summary>
        ''' Send a file to server
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SendFile(ByVal FileFullPath As String) As Boolean

            If Not MyBase.CanSendData(Me.sckSocketContainer) Then Return False

            If Not Me.sckSocketContainer.SendFile(FileFullPath, AddressOf MyBase.Try_SocketErrorMessage,
                                                 AddressOf Me.Try_SendFileTransferProgress,
                                                 AddressOf Me.Try_FileSent,
                                                 AddressOf Me.Try_SendingCancelled) Then

                Try_SocketErrorMessage("Invalid Data or Socket is busy")
                Return False

            End If

            Return True

        End Function

        ''' <summary>
        ''' Use default send file dialog to send file
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SendFile() As Boolean
            Using ofd As OpenFileDialog = New OpenFileDialog()
                With ofd
                    .InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                    .CheckFileExists = True
                    .Multiselect = False
                    .Filter = "All Files (*)|*.*;"
                    If ofd.ShowDialog() = DialogResult.OK Then Return Me.SendFile(ofd.FileName)

                End With

            End Using

            Return False
        End Function


        ''' <summary>
        ''' Cancel current sending operation
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub CancelSending()
            Me.sckSocketContainer.CancelSending()
        End Sub



        ''' <summary>
        ''' Starts the socket Asynchronously
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub [Start]()

            If Me.HandleCrossThreading AndAlso Not Me.CanInvokeEvents Then Return


            If Me.ConnectionState = SocketConnectionState.Disconnected Then
                Try_SocketLogMessage(String.Format("Connecting to server {0} on port {1} ...", Me.ServerIP, Me.Port))
                Me._ConnectionState = SocketConnectionState.Connecting REM To Avoid more than one request
                If Not Me.TryConnectingToServer() Then
                    Try_SocketLogMessage("Client could not connect to server ...")
                    Me._ConnectionState = SocketConnectionState.Disconnected
                    Try_SocketStateChanged(Me.ConnectionState, "Connection to Server failed.")

                Else
                    REM Trying to connect asycn successful but doesnt mean connected
                    Try_SocketLogMessage("Opening Port on Server ...")
                End If
            ElseIf Me.ConnectionState = SocketConnectionState.Connecting Then
                Try_SocketLogMessage("Attempting multi-request")
                Try_SocketErrorMessage("Please be patient ... Trying to connect to Server")

            Else

                REM And it is going to remain in it's current form
                Try_SocketErrorMessage("Client is NOT disconnected!")


            End If

        End Sub

        ''' <summary>
        ''' Stops the Socket
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub [Stop]()

            REM It not necessary under stop because you can't change it if it is connected

            If Not Me.ConnectionState = SocketConnectionState.Disconnected Then
                If Me.DisconnectClient Then

                    Me.Try_SocketErrorMessage("Client is now offline")
                    Me._ConnectionState = SocketConnectionState.Disconnected
                    Me.Try_SocketStateChanged(Me.ConnectionState, "Client Socket went offline")


                Else

                    Me.Try_SocketErrorMessage("Client could not shutdown")

                End If
            Else

                Me.Try_SocketErrorMessage("Client is Already Disconnected!")


            End If



        End Sub



#End Region


#Region "Private"


        ''' <summary>
        ''' Disconnect Client from Server
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function DisconnectClient() As Boolean
            Try

                Me.sckSocketContainer.Dispose()

                REM This part will probably not run because it is already disposed up there
                If Me.sckSocket IsNot Nothing Then
                    If Me.sckSocket.Connected Then Me.sckSocket.Close()
                    Me.sckSocket = Nothing

                End If

                Return True

            Catch ex As Exception

                BASE.LocalLogger.Write(New EException(" DisconnectClient()", ex))
                Return False

            End Try

        End Function

        Private Sub DoClientWentOffline(ByVal ClientReadingClass As ClientSocketWrapper)
            Me.Stop()
        End Sub


        ''' <summary>
        ''' The result of the Async Func Begin Connect
        ''' </summary>
        ''' <param name="ar"></param>
        ''' <remarks></remarks>
        Private Sub DoConnectToServerResult(ByVal ar As IAsyncResult)
            ' Get the listener that handles the client request. 
            Dim connecter As Socket = CType(ar.AsyncState, Socket)

            ' End the operation and display the received data on the 
            'console. 
            Try
                connecter.EndConnect(ar)

                REM If successful no error will occur

                Me.sckSocketContainer = New ClientSocketWrapper(connecter, Me.PacketSize, Me.InActivityTimeout,
                                                                AddressOf MyBase.ProcessDataReceived,
                                                                AddressOf MyBase.ProcessFileReceived,
                                                                AddressOf Me.DoClientWentOffline)


                KeepReceivingMessageFromServer(sckSocketContainer)




                Me.Try_SocketLogMessage("Client Connected")

                Me._ConnectionState = SocketConnectionState.Connected

                REM I will alert client of state changed once I receive my ID


            Catch ex As SocketException
                REM No Connection was made

                REM If I want to dispose the client i will just dispose current socket
                Me.Try_SocketLogMessage(ex.Message)
                Me.Try_SocketStateChanged(SocketConnectionState.Disconnected, "Connection to Server failed.")
                Me._ConnectionState = SocketConnectionState.Disconnected


            Catch ex As Exception
                'Error Occurred Could Not Connect
                'Keep Trying
                Me.Try_SocketErrorMessage("Undocumented Error: " & ex.StackTrace)

            End Try

        End Sub


        ''' <summary>
        ''' Handles Messages Received From Server
        ''' </summary>
        ''' <param name="ar"></param>
        ''' <remarks></remarks>
        Private Sub MessageReceivedFromServer(ByVal ar As IAsyncResult)
            Dim ClientReadingClass As ClientSocketWrapper = CType(ar.AsyncState, ClientSocketWrapper)
            Dim ClientSocket As Socket = ClientReadingClass.ClientSocket


            '
            '
            'If Server has shutdown
            '
            ''        'It blocks before shutting down
            ''        If Not ClientSocket.Connected And ClientSocket.Blocking Then
            ''ServerSocketDownProcess:
            ''            'Client is Shutting Down or has shut down
            ''            'Completing the shutdown process on this place
            ''            'Remove client from lsvMain list
            ''            'Decompose this class with its socket
            ''            ''msg("Server has shut down")
            ''            ''msg("Creating New Client .. ")
            ''            ''msg("Waiting for you to click connect")
            ''            'MySocket = New SocketReadingStateObject()
            ''            ' Me.ServerStatus = MyConnectionStatus.Disconnected
            ''            Debug.Print("Client disconnecting")
            ''            Return
            ''        End If


            '
            'An exception occurrs here .. An existing connection was forcibly closed by the remote operation
            'Which pass the close checking parameters
            '
            Dim ByteSizeRecieved As Integer
            Try
                If ClientReadingClass IsNot Nothing AndAlso
                    Not ClientReadingClass.isDisposed AndAlso ClientSocket IsNot Nothing Then
                    'Say We Have Received the bytes and Stored it in Buffer [ClientReadingClass]
                    ByteSizeRecieved = ClientSocket.EndReceive(ar)
                End If

            Catch ex As ObjectDisposedException
                REM Socket has been disposed 
                REM Meaning no longer available
                REM This calls under client

                REM This happens if the Client Disconnects Manually

            Catch ex As SocketException
                REM This happens only when Server Shuts Down or Something happens to server pc
                If ex.SocketErrorCode = SocketError.ConnectionReset Then
                    'Server Application is Off or Server System Shut Down
                    'Refer to shutdown process
                    Me.Try_SocketLogMessage("Connection Reset")
                    Me.Stop()

                End If
            Catch ex As Exception
                BASE.LocalLogger.Write(New EException(ex))
            End Try

            'If any byte was received
            If ByteSizeRecieved > 0 Then


                If Not ClientReadingClass.NotifyReceived(CUShort(ByteSizeRecieved), AddressOf Me.Try_ReceivingCancelled__Async) Then
                    REM Protocol brigded
                    Return
                End If

                REM ClientReadingClass.NotifyReceived(CUShort(ByteSizeRecieved), AddressOf Me.Try_ReceivingCancelled)
                ' ''Call ProcessClientCommand(
                ' ''            ClientReadingClass,
                ' ''            SocketHelper.ReceiveCommand(
                ' ''                ByteRecieved,
                ' ''                ClientReadingClass
                ' ''                )
                ' ''            )



                'Keep Receiving from server
                Me.KeepReceivingMessageFromServer(ClientReadingClass)

            End If



        End Sub 'Read_Callback

        '' ''' <summary>
        '' ''' Process Command Received From Server
        '' ''' </summary>
        '' ''' <param name="clientReadingClass"></param>
        '' ''' <param name="MessageReceived"></param>
        '' ''' <remarks></remarks>
        ''Private Sub ProcessClientCommand(ByVal clientReadingClass As ClientSocketWrapper,
        ''                          ByVal MessageReceived As String()
        ''                          )

        ''    'Be Consistent With Your Command Name Because It is Case Sensitive
        ''    REM HereIsYourID
        ''    Dim CommandReceived As String = MessageReceived(0)

        ''    Select Case CommandReceived

        ''        Case Is = CMD_HERE_IS_YOUR_ID

        ''            clientReadingClass.ClientInfo = New PCInfo(CInt(Val(MessageReceived(1))))

        ''            Me.SendMessage(CMD_CLIENT_INFO, clientReadingClass.ClientInfo.ToString(), Me.sckSocket)

        ''            Me.Try_SocketStateChanged(SocketConnectionState.Connected, "Connected to Server on Port: " & Me.Port)
        ''            Me.Try_SocketIDChanged()
        ''            REM Check where it belongs

        ''        Case Is = SIMPLE_MESSAGE_COMMAND

        ''            Me.Try_MessageReceived(MessageReceived(1), clientReadingClass.SocketKey)


        ''        Case Is = SIMPLE_COMMAND
        ''            Dim innerMSG() As String = Split(MessageReceived(1), SocketHelper.DelimiterUsedForOutterCommands)

        ''            Me.Try_CommandReceived(innerMSG(0), innerMSG(1), clientReadingClass.SocketKey)




        ''    End Select



        ''End Sub

        ''Private Sub ProcessDataReceived(ByVal clientReadingClass As ClientSocketWrapper, ByVal dataReceived As FRAME)

        ''    If dataReceived.DataType = DataXchange.Packets.Layer1Packet.PayLoadDataTypes.STRING Then
        ''        Dim pDataRecv As Object = DataXchange.StringPackages.SimplePackage.parseStringPackage(dataReceived.RawData)

        ''        Select Case pDataRecv.GetType().Name
        ''            Case Is = GetType(SimplePackage).Name
        ''                Dim sPkg As SimplePackage = CType(pDataRecv, SimplePackage)
        ''                BASE.LocalLogger.Write(sPkg.Name & " - " & sPkg.getContent())
        ''                BASE.LocalLogger.Write("Invalid Package Received")

        ''            Case Is = GetType(UserSimplePackage).Name
        ''                Dim sPkg As UserSimplePackage = CType(pDataRecv, UserSimplePackage)
        ''                Me.Try_MessageReceived(sPkg.getContent(), clientReadingClass.SocketKey)

        ''            Case Is = GetType(SocketSimplePackage).Name
        ''                Dim sPkg As SocketSimplePackage = CType(pDataRecv, SocketSimplePackage)
        ''                Debug.Print(sPkg.Name & " - " & sPkg.getContent())


        ''            Case Is = GetType(CommandPackage).Name
        ''                Dim sPkg As CommandPackage = CType(pDataRecv, CommandPackage)
        ''                BASE.LocalLogger.Write(sPkg.Name & " - " & sPkg.getContent())
        ''                BASE.LocalLogger.Write("Invalid Package Received")

        ''            Case Is = GetType(UserCommandPackage).Name
        ''                Dim sPkg As UserCommandPackage = CType(pDataRecv, UserCommandPackage)
        ''                Me.Try_CommandReceived(sPkg, clientReadingClass.SocketKey)


        ''            Case Is = GetType(SocketCommandPackage).Name
        ''                Dim sPkg As SocketCommandPackage = CType(pDataRecv, SocketCommandPackage)

        ''                Select Case sPkg.CommandName

        ''                    Case Is = CMD_HERE_IS_YOUR_ID

        ''                        clientReadingClass.ClientInfo = New PCInfo(CInt(sPkg.Parameters(0)))

        ''                        REM clientReadingClass.SendMessage(CMD_CLIENT_INFO, clientReadingClass.ClientInfo.ToString(), Me.sckSocket)

        ''                        Me.Try_SocketStateChanged(SocketConnectionState.Connected, "Connected to Server on Port: " & Me.Port)
        ''                        Me.Try_SocketIDChanged()
        ''                        REM Check where it belongs
        ''                End Select


        ''        End Select


        ''    ElseIf dataReceived.DataType = DataXchange.Packets.Layer1Packet.PayLoadDataTypes.RAW_DATA Then

        ''        Debug.Print("Received Raw DATA")

        ''    ElseIf dataReceived.DataType = DataXchange.Packets.Layer1Packet.PayLoadDataTypes.FILE Then

        ''        Debug.Print("Received File")

        ''    End If
        ''End Sub

        Protected Overrides Sub ProcessSocketCommandReceived(ByVal clientReadingClass As ClientSocketWrapper,
                                                             ByVal sckCMD As SocketCommandPackage)

            Select Case sckCMD.CommandName

                Case Is = CMD_HERE_IS_YOUR_ID

                    clientReadingClass.ClientInfo = New PCInfo(CInt(sckCMD.Parameters(0))) REM Store My ID

                    clientReadingClass.SendMessage(New SocketCommandPackage(CMD_CLIENT_PC_INFO, clientReadingClass.ClientInfo))

                    Me.Try_SocketStateChanged(SocketConnectionState.Connected, "Connected to Server on Port: " & Me.Port)
                    Me.Try_SocketIDChanged()
                    REM Check where it belongs

                Case Is = CMD_FILE_TRANSFER_BEGIN
                    clientReadingClass.NotifyFileTransferBegins(sckCMD,
                                                                 AddressOf MyBase.Try_SocketErrorMessage,
                                                                AddressOf MyBase.Try_ReceiveFileTransferProgress
                                                                )

            End Select

        End Sub

        ''' <summary>
        ''' Keep Receiving Messages From Server
        ''' </summary>
        ''' <param name="ClientClass"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function KeepReceivingMessageFromServer(
                                                ByRef ClientClass As ClientSocketWrapper
                                                ) As Boolean

            Try
                ClientClass.ClientSocket.BeginReceive(ClientClass.SocketBucketReceiver, 0, ClientClass.Maximum_Buffer_Size, 0,
                                  New AsyncCallback(AddressOf MessageReceivedFromServer),
                                  ClientClass)

                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function


        ''' <summary>
        ''' Initiate Begin Connect Function
        ''' </summary>
        ''' <remarks></remarks>
        Private Function TryConnectingToServer() As Boolean
            Try
                'Get a fresh copy .. because when a socket is closed it can not be used again

                Dim IEndpt As IPEndPoint = New IPEndPoint(
                                           IPAddress.Parse(Me.ServerIP),
                                           Me.Port
                                           )


                Me.sckSocket = New Socket(IEndpt.AddressFamily, SocketType.Stream, Me.getSelectedProtocol)


                Me.sckSocket.BeginConnect(IEndpt, New AsyncCallback(AddressOf Me.DoConnectToServerResult), Me.sckSocket)

                Return True

            Catch ex As ArgumentNullException
                Me.Try_SocketErrorMessage("Invalid NULL Server IP Address")
                Return False

            Catch ex As FormatException
                Me.Try_SocketErrorMessage("INVALID SERVER IP ADDRESS: " & Me.ServerIP)
                Return False

            Catch ex As Exception

                Me.Try_SocketErrorMessage("Client Try Connect Failed: " & ex.StackTrace)
                Return False

            End Try
        End Function



#End Region

#End Region






    End Class

End Namespace
