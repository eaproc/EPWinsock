Imports System.Net.Sockets
Imports System.ComponentModel

Imports EPWinSock.v4.DataXchange.StringPackages
Imports EPWinSock.v4.NET
Imports EPWinSock.v4.DataXchange.Packets.Layer2Packet
Imports EPWinSock.v4.DataXchange.Packets
Imports EPWinSock.v4.Utilities

Imports EPRO.Library.v4


''' <summary>
''' Bi-Functional Class. Acting as Server or as Client
''' </summary>
''' <remarks>Server Listens on All IPs by default</remarks>
<ToolboxBitmap("F:\OneDrive\.NET PROGRAMMING\Projs\Class Libraries Projs\Sockets\V4.0\R10\sckProject\EPWinSock.v4\Resources\Wsck_16_16.bmp")> _
Public MustInherit Class BASE
    Implements IComponent


    REM Perhaps, Server Listening on Local IP will solve the multi-adapter issue
    REM In as much you can get to the machine you should always get a response ..  regardless of the route you are taking


#Region "Constructors"

    Sub New()

        LocalLogger =
        New CODERiT.Logger.v._3._5.Log1(Me.GetType(), CODERiT.Logger.v._3._5.Log1.Modes.CONSOLE_AND_FILE, True)

        REM I just discovered that this is not required to connect to a pc.
        REM It is only required if you want to ping the pc
        REM Check if ICMPv4 is enabled
        If Shell.OperatingSystem.getOSType = Shell.OperatingSystem.MicrosoftOS.WINDOWS_VISTA OrElse
           Shell.OperatingSystem.getOSType = Shell.OperatingSystem.MicrosoftOS.WINDOWS_7 OrElse
           Shell.OperatingSystem.getOSType = Shell.OperatingSystem.MicrosoftOS.WINDOWS_8 OrElse
           Shell.OperatingSystem.getOSType = Shell.OperatingSystem.MicrosoftOS.WINDOWS_8_1 OrElse
            Shell.OperatingSystem.getOSType = Shell.OperatingSystem.MicrosoftOS.WINDOWS_XP Then
            If Not Shell.Firewall.isICMPv4Allowed Then
                If IsDebugMode Then Debug.Print("AddICMPv4Exception: " & Shell.Firewall.AddICMPv4Exception().ToString())
            End If
        End If
        REM --------------------------------------------


    End Sub

    ''' <summary>
    ''' Mainly used by designer
    ''' </summary>
    ''' <param name="mContainer">Use to dispose components</param>
    ''' <remarks></remarks>
    Sub New(ByVal mContainer As IContainer)

        Me.New()


        Me.ParentControl_IContainer = mContainer


    End Sub


#End Region


#Region "Enums"

    ''' <summary>
    ''' Socket Connection States
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum SocketConnectionState
        Connected
        Disconnected
        Connecting
        Listening
    End Enum


    ''' <summary>
    ''' Socket Connection Protocol
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum SocketConnectionProtocol
        TCP
        REM For now TCP Only
    End Enum

    ''' <summary>
    ''' The amount of bytes that should be transfer per packet
    ''' </summary>
    ''' <remarks>This depends on the transfer layer. Very Large Packet is not recommended</remarks>
    Public Enum SocketBufferSize
        ''' <summary>
        ''' 1KB
        ''' </summary>
        ''' <remarks></remarks>
        SMALL = 1024

        ''' <summary>
        ''' 2KB
        ''' </summary>
        ''' <remarks></remarks>
        MEDIUM = 2048

        ''' <summary>
        ''' 4KB
        ''' </summary>
        ''' <remarks></remarks>
        LARGE = 4096

        ''' <summary>
        ''' 8KB
        ''' </summary>
        ''' <remarks></remarks>
        EXTRA_LARGE = 8192

        ''' <summary>
        ''' 16KB
        ''' </summary>
        ''' <remarks></remarks>
        EXTREMELY_LARGE = 16384

        ''' <summary>
        ''' 32KB - 32768 dropped to 32767
        ''' </summary>
        ''' <remarks></remarks>
        ARE_YOU_KIDDING = 32767

    End Enum

    REM Can get up to 64KB Theoretically


#End Region


#Region "Events"

    REM But if you are sure it is on same thread you can call it directly
    REM Am attaching each event to a Sub to Handle Cross Threading



    ''' <summary>
    ''' Raises when Socket receives a simple Command
    ''' </summary>
    ''' <param name="SckCommand"></param>
    ''' <remarks></remarks>
    Public Event CommandReceived(ByVal SckCommand As UserCommandPackage,
                                 ByVal senderSocketID As Integer)

    ''' <summary>
    ''' Raises the Command Received under the Parent Control Thread
    ''' </summary>
    ''' <param name="SckCommand"></param>
    ''' <remarks></remarks>
    Protected Sub Try_CommandReceived(ByVal SckCommand As UserCommandPackage,
                                    ByVal senderSocketID As Integer)

        If Me.HandleCrossThreading Then
            REM Just making sure the parent control is ok before we do anything
            If Me.ParentControl Is Nothing Then Return
            If Me.ParentControl.IsDisposed Then Return
            If Not Me.ParentControl.Created Then Return

            Me.ParentControl.Invoke(Sub() Me.Invoke_CommandReceived(SckCommand, senderSocketID))
        Else
            Me.Invoke_CommandReceived(SckCommand, senderSocketID)
        End If

    End Sub

    ''' <summary>
    ''' The real invoke inner_Call
    ''' </summary>
    ''' <param name="SckCommand"></param>
    ''' <remarks></remarks>
    Private Sub Invoke_CommandReceived(ByVal SckCommand As UserCommandPackage,
                                       ByVal senderSocketID As Integer)
        RaiseEvent CommandReceived(SckCommand, senderSocketID)
    End Sub






    ''' <summary>
    ''' Raises when Socket receives a Data sent through sendData method
    ''' </summary>
    ''' <param name="SckData"></param>
    ''' <remarks></remarks>
    Public Event DataReceived(ByVal SckData As Byte(),
                                 ByVal senderSocketID As Integer)

    ''' <summary>
    ''' Raises the Data Received under the Parent Control Thread
    ''' </summary>
    ''' <param name="SckData"></param>
    ''' <remarks></remarks>
    Protected Sub Try_DataReceived(ByVal SckData As Byte(),
                                    ByVal senderSocketID As Integer)

        If Me.HandleCrossThreading Then
            REM Just making sure the parent control is ok before we do anything
            If Me.ParentControl Is Nothing Then Return
            If Me.ParentControl.IsDisposed Then Return
            If Not Me.ParentControl.Created Then Return

            Me.ParentControl.Invoke(Sub() Me.Invoke_DataReceived(SckData, senderSocketID))
        Else
            Me.Invoke_DataReceived(SckData, senderSocketID)
        End If


    End Sub

    ''' <summary>
    ''' The real invoke inner_Call
    ''' </summary>
    ''' <param name="SckData"></param>
    ''' <remarks></remarks>
    Private Sub Invoke_DataReceived(ByVal SckData As Byte(),
                                       ByVal senderSocketID As Integer)
        RaiseEvent DataReceived(SckData, senderSocketID)
    End Sub




    ''' <summary>
    ''' Raises when Socket Received File sent through sendFile method
    ''' </summary>
    ''' <param name="FileReceived"></param>
    ''' <remarks></remarks>
    Public Event FileReceived(ByVal FileReceived As Utilities.FileAccumulator,
                                 ByVal senderSocketID As Integer)

    ''' <summary>
    ''' Raises the File Received under the Parent Control Thread
    ''' </summary>
    ''' <param name="FileReceived"></param>
    ''' <remarks></remarks>
    Protected Sub Try_FileReceived(ByVal FileReceived As Utilities.FileAccumulator,
                                 ByVal senderSocketID As Integer)

        If Me.HandleCrossThreading Then
            REM Just making sure the parent control is ok before we do anything
            If Me.ParentControl Is Nothing Then Return
            If Me.ParentControl.IsDisposed Then Return
            If Not Me.ParentControl.Created Then Return

            Me.ParentControl.Invoke(Sub() Me.Invoke_FileReceived(FileReceived, senderSocketID))
        Else
            Me.Invoke_FileReceived(FileReceived, senderSocketID)
        End If


    End Sub


    Protected Sub Try_FileReceived__Async(ByVal FileReceived As Utilities.FileAccumulator,
                                 ByVal senderSocketID As Integer)
        REM Should be async to avoid being stucked since
        REM Idle time depends so much on receiving
        With New Threading.Thread(Sub() Me.Try_FileReceived(FileReceived, senderSocketID))
            .IsBackground = True
            .Start()
        End With
        REM Return back this thread
    End Sub


    ''' <summary>
    ''' The real invoke inner_Call
    ''' </summary>
    ''' <param name="FileReceived"></param>
    ''' <remarks></remarks>
    Private Sub Invoke_FileReceived(ByVal FileReceived As Utilities.FileAccumulator,
                                 ByVal senderSocketID As Integer)
        RaiseEvent FileReceived(FileReceived, senderSocketID)
        FileReceived.Dispose()
        FileReceived = Nothing
    End Sub







    ''' <summary>
    ''' Raises when Socket Completes File sent through sendFile method
    ''' </summary>
    ''' <remarks></remarks>
    Public Event FileSent(ByVal senderSocketID As Integer)

    ''' <summary>
    ''' Raises the File Received under the Parent Control Thread
    ''' </summary>
    ''' <remarks></remarks>
    Protected Sub Try_FileSent(ByVal senderSocketID As Integer)

        If Me.HandleCrossThreading Then
            REM Just making sure the parent control is ok before we do anything
            If Me.ParentControl Is Nothing Then Return
            If Me.ParentControl.IsDisposed Then Return
            If Not Me.ParentControl.Created Then Return

            Me.ParentControl.Invoke(Sub() Me.Invoke_FileSent(senderSocketID))

        Else

            Me.Invoke_FileSent(senderSocketID)

        End If


    End Sub

    ''' <summary>
    ''' The real invoke inner_Call
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub Invoke_FileSent(ByVal senderSocketID As Integer)
        RaiseEvent FileSent(senderSocketID)
    End Sub





    ''' <summary>
    ''' Raises when Sending is Cancelled
    ''' </summary>
    ''' <remarks></remarks>
    Public Event SendingCancelled(ByVal senderSocketID As Integer)

    ''' <summary>
    ''' Raises the Sending Cancelled under the Parent Control Thread
    ''' </summary>
    ''' <remarks></remarks>
    Protected Sub Try_SendingCancelled(ByVal senderSocketID As Integer)
        If Me.HandleCrossThreading Then
            REM Just making sure the parent control is ok before we do anything
            If Me.ParentControl Is Nothing Then Return
            If Me.ParentControl.IsDisposed Then Return
            If Not Me.ParentControl.Created Then Return

            Me.ParentControl.Invoke(Sub() Me.Invoke_SendingCancelled(senderSocketID))
        Else
            Me.Invoke_SendingCancelled(senderSocketID)
        End If


    End Sub

    ''' <summary>
    ''' The real invoke inner_Call
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub Invoke_SendingCancelled(ByVal senderSocketID As Integer)
        RaiseEvent SendingCancelled(senderSocketID)
    End Sub





    Protected Sub Try_ReceivingCancelled__Async(ByVal senderSocketID As Integer)
        REM Should be async to avoid being stucked since
        REM Idle time depends so much on receiving
        With New Threading.Thread(Sub() Me.Try_ReceivingCancelled(senderSocketID))
            .IsBackground = True
            .Start()
        End With
        REM Return back this thread
    End Sub


    ''' <summary>
    ''' Raises when Receiving is Cancelled
    ''' </summary>
    ''' <remarks></remarks>
    Public Event ReceivingCancelled(ByVal senderSocketID As Integer)

    ''' <summary>
    ''' Raises the Receiving Cancelled under the Parent Control Thread
    ''' </summary>
    ''' <remarks></remarks>
    Protected Sub Try_ReceivingCancelled(ByVal senderSocketID As Integer)

        If Me.HandleCrossThreading Then
            REM Just making sure the parent control is ok before we do anything
            If Me.ParentControl Is Nothing Then Return
            If Me.ParentControl.IsDisposed Then Return
            If Not Me.ParentControl.Created Then Return

            Me.ParentControl.Invoke(Sub() Me.Invoke_ReceivingCancelled(senderSocketID))
        Else
            Me.Invoke_ReceivingCancelled(senderSocketID)
        End If


    End Sub

    ''' <summary>
    ''' The real invoke inner_Call
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub Invoke_ReceivingCancelled(ByVal senderSocketID As Integer)

        RaiseEvent ReceivingCancelled(senderSocketID)


    End Sub










    ''' <summary>
    ''' Raises when Socket receiving a file transmission
    ''' </summary>
    ''' <remarks></remarks>
    Public Event ReceiveFileTransferProgress(ByVal FileName As String, ByVal TotalBytesReceived As Int32, ByVal FileBytesSize As Int32,
                                                ByVal SenderSocketID As Int32)

    ''' <summary>
    ''' Raises the File transfer progress under the Parent Control Thread
    ''' </summary>
    ''' <remarks></remarks>
    Protected Sub Try_ReceiveFileTransferProgress(ByVal FileName As String, ByVal TotalBytesReceived As Int32, ByVal FileBytesSize As Int32,
                                                ByVal SenderSocketID As Int32)
        If Me.HandleCrossThreading Then
            REM Just making sure the parent control is ok before we do anything
            If Me.ParentControl Is Nothing Then Return
            If Me.ParentControl.IsDisposed Then Return
            If Not Me.ParentControl.Created Then Return

            Me.ParentControl.Invoke(Sub() Me.Invoke_ReceiveFileTransferProgress(FileName, TotalBytesReceived, FileBytesSize, SenderSocketID))

        Else
            Me.Invoke_ReceiveFileTransferProgress(FileName, TotalBytesReceived, FileBytesSize, SenderSocketID)
        End If



    End Sub

    ''' <summary>
    ''' The real invoke inner_Call
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub Invoke_ReceiveFileTransferProgress(ByVal FileName As String, ByVal TotalBytesReceived As Int32, ByVal FileBytesSize As Int32,
                                                ByVal SenderSocketID As Int32)
        RaiseEvent ReceiveFileTransferProgress(FileName, TotalBytesReceived, FileBytesSize, SenderSocketID)
    End Sub











    ''' <summary>
    ''' Raises when Socket receiving a file transmission
    ''' </summary>
    ''' <remarks></remarks>
    Public Event SendFileTransferProgress(ByVal FileName As String, ByVal TotalBytesSent As Int32, ByVal FileBytesSize As Int32,
                                                ByVal SenderSocketID As Int32)

    ''' <summary>
    ''' Raises the File transfer progress under the Parent Control Thread
    ''' </summary>
    ''' <remarks></remarks>
    Protected Sub Try_SendFileTransferProgress(ByVal FileName As String, ByVal TotalBytesSent As Int32, ByVal FileBytesSize As Int32,
                                                ByVal SenderSocketID As Int32)
        If Me.HandleCrossThreading Then
            REM Just making sure the parent control is ok before we do anything
            If Me.ParentControl Is Nothing Then Return
            If Me.ParentControl.IsDisposed Then Return
            If Not Me.ParentControl.Created Then Return

            Me.ParentControl.Invoke(Sub() Me.Invoke_SendFileTransferProgress(FileName, TotalBytesSent, FileBytesSize, SenderSocketID))
        Else
            Me.Invoke_SendFileTransferProgress(FileName, TotalBytesSent, FileBytesSize, SenderSocketID)
        End If

    End Sub

    ''' <summary>
    ''' The real invoke inner_Call
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub Invoke_SendFileTransferProgress(ByVal FileName As String, ByVal TotalBytesSent As Int32, ByVal FileBytesSize As Int32,
                                                ByVal SenderSocketID As Int32)
        RaiseEvent SendFileTransferProgress(FileName, TotalBytesSent, FileBytesSize, SenderSocketID)
    End Sub





    ''' <summary>
    ''' Raises when Socket receives a simple message
    ''' </summary>
    ''' <param name="SckMessage"></param>
    ''' <remarks></remarks>
    Public Event MessageReceived(ByVal SckMessage As String, ByVal senderSocketID As Integer)

    ''' <summary>
    ''' Raises the Message Received under the Parent Control Thread
    ''' </summary>
    ''' <param name="SckMessage"></param>
    ''' <remarks></remarks>
    Protected Sub Try_MessageReceived(ByVal SckMessage As String, ByVal senderSocketID As Integer)
        If Me.HandleCrossThreading Then
            REM Just making sure the parent control is ok before we do anything
            If Me.ParentControl Is Nothing Then Return
            If Me.ParentControl.IsDisposed Then Return
            If Not Me.ParentControl.Created Then Return

            Me.ParentControl.Invoke(Sub() Me.Invoke_MessageReceived(SckMessage, senderSocketID))
        Else
            Me.Invoke_MessageReceived(SckMessage, senderSocketID)
        End If


    End Sub

    ''' <summary>
    ''' The real invoke inner_Call
    ''' </summary>
    ''' <param name="SckMessage"></param>
    ''' <remarks></remarks>
    Private Sub Invoke_MessageReceived(ByVal SckMessage As String, ByVal senderSocketID As Integer)
        RaiseEvent MessageReceived(SckMessage, senderSocketID)
    End Sub



    ''' <summary>
    ''' Raises when there is an error
    ''' </summary>
    ''' <param name="SckMessage"></param>
    ''' <remarks></remarks>
    Public Event SocketErrorMessage(ByVal SckMessage As String)
    Public Delegate Sub dlgSocketErrorMessage(ByVal SckMessage As String)

    ''' <summary>
    ''' Raises the Socket Error Message under the Parent Control Thread
    ''' </summary>
    ''' <param name="SckMessage"></param>
    ''' <remarks></remarks>
    Protected Sub Try_SocketErrorMessage(ByVal SckMessage As String)
        If Me.HandleCrossThreading Then
            REM Just making sure the parent control is ok before we do anything
            If Me.ParentControl Is Nothing Then Return
            If Me.ParentControl.IsDisposed Then Return
            If Not Me.ParentControl.Created Then Return

            Me.ParentControl.Invoke(Sub() Me.Invoke_SocketErrorMessage(SckMessage))
        Else
            Me.Invoke_SocketErrorMessage(SckMessage)
        End If


    End Sub

    ''' <summary>
    ''' The real invoke inner_Call
    ''' </summary>
    ''' <param name="SckMessage"></param>
    ''' <remarks></remarks>
    Private Sub Invoke_SocketErrorMessage(ByVal SckMessage As String)
        RaiseEvent SocketLogMessage(SckMessage)
    End Sub




    ''' <summary>
    ''' Raised when the state of this socket changes
    ''' </summary>
    ''' <param name="CurrentState"></param>
    ''' <param name="DebugMessage"></param>
    ''' <remarks></remarks>
    Public Event SocketStateChanged(ByVal CurrentState As SocketConnectionState,
                                    ByVal DebugMessage As String)


    ''' <summary>
    ''' Raises the SocketStateChanged under the Parent Control Thread
    ''' </summary>
    ''' <remarks></remarks>
    Protected Sub Try_SocketStateChanged(ByVal CurrentState As SocketConnectionState,
                                    ByVal DebugMessage As String)

        If Me.HandleCrossThreading Then
            REM Just making sure the parent control is ok before we do anything
            If Me.ParentControl Is Nothing Then Return
            If Me.ParentControl.IsDisposed Then Return
            If Not Me.ParentControl.Created Then Return

            Me.ParentControl.Invoke(Sub() Me.Invoke_SocketStateChanged(CurrentState, DebugMessage))
        Else
            Me.Invoke_SocketStateChanged(CurrentState, DebugMessage)
        End If

    End Sub

    ''' <summary>
    ''' The real invoke inner_Call
    ''' </summary>
    ''' <remarks>Only call by invoking</remarks>
    Private Sub Invoke_SocketStateChanged(ByVal CurrentState As SocketConnectionState,
                                    ByVal DebugMessage As String)
        RaiseEvent SocketStateChanged(CurrentState, DebugMessage)
    End Sub



    ''' <summary>
    ''' Raises event log messages
    ''' </summary>
    ''' <param name="sckLog"></param>
    ''' <remarks></remarks>
    Public Event SocketLogMessage(ByVal sckLog As String)

    ''' <summary>
    ''' Raises the SocketLogMessage under the Parent Control Thread
    ''' </summary>
    ''' <remarks></remarks>
    Protected Sub Try_SocketLogMessage(ByVal sckLog As String)
        If Me.HandleCrossThreading Then
            REM Just making sure the parent control is ok before we do anything
            If Me.ParentControl Is Nothing Then Return
            If Me.ParentControl.IsDisposed Then Return
            If Not Me.ParentControl.Created Then Return

            Me.ParentControl.Invoke(Sub() Me.Invoke_SocketLogMessage(sckLog))

        Else
            Me.Invoke_SocketLogMessage(sckLog)
        End If

    End Sub

    ''' <summary>
    ''' The real invoke inner_Call. Only Call by invoking
    ''' </summary>
    ''' <remarks>Only call by invoking</remarks>
    Private Sub Invoke_SocketLogMessage(ByVal sckLog As String)
        RaiseEvent SocketLogMessage(sckLog)
    End Sub



#End Region


#Region "Delegates"

    Friend Delegate Sub dlgProcessReceiveDataHandler(ByVal clientReadingClass As ClientSocketWrapper,
                                                               ByVal dataReceived As FRAME)
    Friend Delegate Sub dlgProcessReceiveFileHandler(ByVal clientReadingClass As ClientSocketWrapper,
                                                               ByVal dataReceived As Utilities.FileAccumulator)

    Friend Delegate Sub dlgFileTransferProgress(ByVal FileName As String, ByVal TotalBytesSent As Int32, ByVal FileBytesSize As Int32,
                                                ByVal SenderSocketID As Int32)

    Friend Delegate Sub dlgFileSent(ByVal senderSocketID As Integer)


    Friend Delegate Sub dlgDoClientWentOffline(ByVal ClientReadingClass As ClientSocketWrapper)

#End Region


#Region "Constants"


    ''' <summary>
    ''' Local IP if you Need to Use it
    ''' </summary>
    ''' <remarks></remarks>
    Public Const LOCAL_IP As String = "127.0.0.1"


    Protected Friend Const CMD_CLIENT_PC_INFO As String = "CLIENT INFO"
    Protected Friend Const CMD_HERE_IS_YOUR_ID As String = "HereIsYourID"
    Protected Friend Const CMD_FILE_TRANSFER_BEGIN As String = "Begin File Transfer"
    Protected Friend Const CMD_AM_ALIVE As String = "AM ALIVE"
    Protected Friend Const CMD_AM_ALIVE__ECHO As String = "AM ALIVE ECHO"

#End Region



#Region "Properties"

    Friend Shared LocalLogger As CODERiT.Logger.v._3._5.Log1



#Region "Protected"

    Public Shared ____IsDebugMode As Boolean = False
    Protected Friend Shared ReadOnly Property IsDebugMode As Boolean
        Get
            Return ____IsDebugMode
        End Get
    End Property


    ''' <summary>
    ''' The acting socket as client or server for this class
    ''' </summary>
    ''' <remarks></remarks>
    Protected sckSocket As Socket

    ''' <summary>
    ''' The IContainer passed in by the designer
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Property ParentControl_IContainer As IContainer = Nothing

    Protected _ParentControl As Form = Nothing


#End Region



    ''' <summary>
    ''' The amount of bytes that should be transfer per packet 
    ''' [NOTE: Total Memory usage for clients will be paket Size * Number of Connected Clients]
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    < _
    System.ComponentModel.Category("Data"), _
    System.ComponentModel.DisplayName("PaketSize"), _
    System.ComponentModel.Description("The amount of bytes that should be transfer per packet [NOTE: Total Memory usage for clients will be paket Size * Number of Connected Clients]"), _
    System.ComponentModel.DefaultValue(SocketBufferSize.SMALL)
        > _
    Public Property PacketSize As SocketBufferSize = SocketBufferSize.SMALL



    ''' <summary>
    ''' GET or Set the  Main Form or Control holding this Component
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    < _
   System.ComponentModel.Category("CrossThreading"), _
   System.ComponentModel.DisplayName("ParentControl"), _
   System.ComponentModel.Description("GET or Set the  Main Form or Control holding this Component"), _
   System.ComponentModel.DefaultValue(GetType(Form))
       > _
    Public Property ParentControl As Form
        Get

            Return Me._ParentControl
        End Get
        Set(ByVal value As Form)

            If Me.ConnectionState <> SocketConnectionState.Disconnected Then
                RaiseEvent SocketErrorMessage("You cant change this property while the Socket is NOT Disconnected!")
            Else
                Me._ParentControl = value
            End If
        End Set
    End Property



    Private ___HandleCrossThreading As Boolean = True
    ''' <summary>
    ''' GET or Set the if events should be called on the Parent Control. If TRUE, Parent Control MUST be set
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    < _
   System.ComponentModel.Category("CrossThreading"), _
   System.ComponentModel.DisplayName("HandleCrossThreading"), _
   System.ComponentModel.Description("GET or Set the if events should be called on the Parent Control. If TRUE, Parent Control MUST be set"), _
   System.ComponentModel.DefaultValue(True)
       > _
    Public Property HandleCrossThreading As Boolean
        Get

            Return Me.___HandleCrossThreading
        End Get
        Set(ByVal value As Boolean)
            If Me.ConnectionState <> SocketConnectionState.Disconnected Then
                RaiseEvent SocketErrorMessage("You cant change this property while the Socket is NOT Disconnected!")
            Else
                Me.___HandleCrossThreading = value
            End If
        End Set
    End Property



    ''' <summary>
    ''' Pass out the connection state
    ''' </summary>
    ''' <remarks></remarks>
    Protected _ConnectionState As SocketConnectionState = SocketConnectionState.Disconnected

    ''' <summary>
    ''' Socket Connection State
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    < _
System.ComponentModel.Category("RunTime"), _
System.ComponentModel.DisplayName("ConnectionState"), _
System.ComponentModel.Description("Socket Connection State"), _
System.ComponentModel.Browsable(False)
   > _
    Public ReadOnly Property ConnectionState As SocketConnectionState
        Get
            Return Me._ConnectionState
        End Get
    End Property



    ''' <summary>
    ''' Ranges between 1 to 32767
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    < _
    System.ComponentModel.Category("Connection"), _
    System.ComponentModel.DisplayName("Port"), _
    System.ComponentModel.Description("Enter a Port Number to bind the socket to. Ranges between 1 to 32767"), _
    System.ComponentModel.DefaultValue(2000)
        > _
    Public Property Port As Int16 = 2000


    ''' <summary>
    ''' The Idle time in millisecs for client sockets to disconnect automatically. Ranges between 20000 to 65536 millisecs
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    < _
    System.ComponentModel.Category("Connection"), _
    System.ComponentModel.DisplayName("InActivityTimeout"), _
    System.ComponentModel.Description("The Idle time in millisecs for client sockets to disconnect automatically. Ranges between 20000 to 65536 millisecs"), _
     System.ComponentModel.DefaultValue(ClientSocketWrapper.SocketActivityMonitor.DEFAULT__INACTIVE_TIMEOUT)
        > _
    Public Property InActivityTimeout As UInt16 = ClientSocketWrapper.SocketActivityMonitor.DEFAULT__INACTIVE_TIMEOUT



    ''' <summary>
    ''' For Now I will be exposing the Protocols supported by this control
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    < _
     System.ComponentModel.Category("Connection"), _
     System.ComponentModel.DisplayName("ConnectionProtocol"), _
     System.ComponentModel.Description("Select the Connection Protocol"),
     System.ComponentModel.DefaultValue(SocketConnectionProtocol.TCP)
         > _
    Public Property ConnectionProtocol As SocketConnectionProtocol = SocketConnectionProtocol.TCP



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
    Public MustOverride ReadOnly Property SocketID As Integer


#End Region



#Region "Methods"


#Region "Prrotected"

    ''' <summary>
    ''' Confirm and notify if socket can send info
    ''' </summary>
    ''' <param name="sck"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Function CanSendData(ByVal sck As ClientSocketWrapper) As Boolean
        If sck Is Nothing OrElse Not sck.CanSendData() Then Try_SocketErrorMessage("Socket is in a pretty bad shape. You can't send any data on it. It may be disconnected.") : Return False
        Return True
    End Function

    Protected Function CanInvokeEvents() As Boolean
        If Me.ParentControl Is Nothing Then
            MsgBox("Please SET the ParentControl Property on the Socket")
            Return False
        End If
        Return True
    End Function

    '' ''' <summary>
    '' ''' Use for direct access ..  This is what is going to appear
    '' ''' </summary>
    '' ''' <param name="Message">Message to send</param>
    '' ''' <param name="sck ">Socket ID to Send to .. It is only neccessary if this component is running in server mode</param>
    '' ''' <returns></returns>
    '' ''' <remarks></remarks>
    ''Protected Function SendMessage(ByVal CommandTitle As String,
    ''                            ByVal Message As String,
    ''                            ByVal sck As Socket
    ''                            ) As Boolean

    ''    REM So for a simple message am appending 10bytes which is Message with delimiter which is 3bytes currently
    ''    REM So User's message should not be more than Me.PacketSize - 10 characters
    ''    Dim AppendingTitleLength As Int32 = CommandTitle.Length + DelimiterUsedForInnerCommands.Length

    ''    If Message.Length > Me.PacketSize - AppendingTitleLength Then
    ''        Try_SocketErrorMessage(
    ''            String.Format("Message length is {0}. Maximum: {1} Characters is Allowed.", Message.Length, Me.PacketSize - AppendingTitleLength)
    ''            )

    ''        Return False
    ''    End If



    ''    If Not Me.ConnectionState = SocketConnectionState.Connected Then

    ''        Try_SocketErrorMessage("Socket is NOT Connected!")

    ''        Return False
    ''    End If


    ''    REM A client sends the message to itself
    ''    REM A Server searches for the socket responsible for the client and sends it


    ''    Return SocketHelper.SendCommand(sck, Message, CommandTitle)


    ''End Function

    ''' <summary>
    ''' Converts my exposed protocols to Standard Protocol Enumerations
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Function getSelectedProtocol() As ProtocolType
        Select Case Me.ConnectionProtocol
            Case Is = SocketConnectionProtocol.TCP
                Return ProtocolType.Tcp
        End Select

        Return ProtocolType.Tcp

    End Function


    Friend Sub ProcessDataReceived(ByVal clientReadingClass As ClientSocketWrapper, ByVal dataReceived As FRAME)

        If dataReceived.DataType = DataXchange.Packets.Layer1Packet.PayLoadDataTypes.STRING Then
            Dim pDataRecv As Object = DataXchange.StringPackages.SimplePackage.parseStringPackage(dataReceived.RawData)


            Select Case pDataRecv.GetType().Name
                Case Is = GetType(SimplePackage).Name
                    Dim sPkg As SimplePackage = CType(pDataRecv, SimplePackage)
                    BASE.LocalLogger.Write(sPkg.PackageIdentifierName & " - " & sPkg.getContent())
                    BASE.LocalLogger.Write("Invalid Package Received")

                Case Is = GetType(UserSimplePackage).Name
                    Dim sPkg As UserSimplePackage = CType(pDataRecv, UserSimplePackage)
                    Me.Try_MessageReceived(sPkg.getContent(), clientReadingClass.SocketKey)

                Case Is = GetType(SocketSimplePackage).Name
                    Dim sPkg As SocketSimplePackage = CType(pDataRecv, SocketSimplePackage)
                    If BASE.IsDebugMode Then BASE.LocalLogger.Write(sPkg.PackageIdentifierName & " - " & sPkg.getContent())
                    Select Case sPkg.getContent()
                        Case Is = CMD_AM_ALIVE
                            REM Echo back
                            clientReadingClass.SendMessage(New SocketSimplePackage(CMD_AM_ALIVE__ECHO)) REM ECHO
                        Case Is = CMD_AM_ALIVE__ECHO
                            REM Do nothing

                    End Select

                Case Is = GetType(CommandPackage).Name
                    Dim sPkg As CommandPackage = CType(pDataRecv, CommandPackage)
                    BASE.LocalLogger.Write(sPkg.PackageIdentifierName & " - " & sPkg.getContent())
                    BASE.LocalLogger.Write("Invalid Package Received")

                Case Is = GetType(UserCommandPackage).Name
                    Dim sPkg As UserCommandPackage = CType(pDataRecv, UserCommandPackage)
                    Me.Try_CommandReceived(sPkg, clientReadingClass.SocketKey)


                Case Is = GetType(SocketCommandPackage).Name
                    Dim sPkg As SocketCommandPackage = CType(pDataRecv, SocketCommandPackage)

                    ProcessSocketCommandReceived(clientReadingClass, sPkg)

            End Select


        ElseIf dataReceived.DataType = DataXchange.Packets.Layer1Packet.PayLoadDataTypes.RAW_DATA Then

            If IsDebugMode Then Debug.Print("Received Raw DATA")
            Me.Try_DataReceived(dataReceived.RawData, clientReadingClass.SocketKey)


        ElseIf dataReceived.DataType = DataXchange.Packets.Layer1Packet.PayLoadDataTypes.FILE Then
            REM File Should Not come here :)
            LocalLogger.Write("Received File on Process Data Received. Very Weird")

        End If
    End Sub


    Protected Friend Sub ProcessFileReceived(clientReadingClass As ClientSocketWrapper, sckCMD As FileAccumulator)
        Me.Try_FileReceived__Async(sckCMD, clientReadingClass.SocketKey)
    End Sub


    Protected MustOverride Sub ProcessSocketCommandReceived(ByVal clientReadingClass As ClientSocketWrapper,
                                                            ByVal sckCMD As SocketCommandPackage)


#End Region





    ''' <summary>
    ''' Starts the socket
    ''' </summary>
    ''' <remarks></remarks>
    Public MustOverride Sub [Start]()

    ''' <summary>
    ''' Stops the Socket
    ''' </summary>
    ''' <remarks></remarks>
    Public MustOverride Sub [Stop]()





#End Region




#Region "IDisposable Support"

    REM ISite Not Functional Yet -------------------------------------------
    REM Am making Private for now so it doesnt interfare with the Usage
    REM Because if user tries to make it none on design mode .. It will cause error
    Private __Site As ISite
    Private Property Site As System.ComponentModel.ISite Implements System.ComponentModel.IComponent.Site
        Get
            Return __Site
        End Get
        Set(ByVal value As System.ComponentModel.ISite)
            __Site = value
        End Set
    End Property

    REM ----------------------------------------------------------------------------




    Public Event Disposed As System.EventHandler Implements IComponent.Disposed

    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                If Me.sckSocket IsNot Nothing Then
                    If Me.sckSocket.Connected Then Me.sckSocket.Close()
                    Me.sckSocket = Nothing
                End If
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub


#End Region




End Class
