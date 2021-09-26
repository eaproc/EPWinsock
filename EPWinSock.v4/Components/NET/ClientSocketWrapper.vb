Imports System.Net.Sockets
Imports EPWinSock.v4.Utilities

Imports EPWinSock.v4.DataXchange.StringPackages
Imports EPWinSock.v4.DataXchange.Packets.Layer2Packet
Imports EPWinSock.v4.DataXchange.Packets

Imports CODERiT.Logger.v._3._5.Exceptions

Imports EPWinSock.v4.BASE
Imports EPRO.Library.v3._5.Objects

Imports System.Threading

Namespace NET

    ''' <summary>
    ''' Encase the client socket so we can extend it and have more functions without extending the socket itself
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ClientSocketWrapper
        Implements IDisposable

        ' This Algorithm helps fast indexing
        '
#Region "Constructors"
        ''Sub New()

        ''End Sub
        Friend Sub New(ByVal ClientSocket As Socket,
                ByVal PacketSize As BASE.SocketBufferSize,
                ByVal InActivityTimeout As UShort,
                ByVal ReceiverHandler As dlgProcessReceiveDataHandler,
                ByVal FileReceiverHandler As dlgProcessReceiveFileHandler,
                ByVal onTimeOutAction As dlgDoClientWentOffline )

            Me.___ClientSocket = ClientSocket
            Me.ClientSocket.NoDelay = True

            Me._Buffer_Size = CInt(PacketSize)

            Me.evTimeOutAction = onTimeOutAction

            Me.CleanBucketReceiver()
            Me.Mailer = New DataXchange.Packets.Layer2Packet(Me, ReceiverHandler, FileReceiverHandler)
            Me.MonitorInActivity = New SocketActivityMonitor(AddressOf Me.onTimeUpInActive, AddressOf Me.SendMessage, InActivityTimeout)

        End Sub

        ''Sub New(ByVal IEndpt As IPEndPoint)
        ''    Me.workSocket = New Socket(IEndpt.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
        ''End Sub

#End Region

#Region "Destructors"

        ''' <summary>
        ''' Indicates if this class is disposed
        ''' </summary>
        ''' <remarks></remarks>
        Private Disposed As Boolean = False
        ''' <summary>
        ''' Indicates if this class is disposed
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property isDisposed As Boolean
            Get
                Return Me.Disposed
            End Get
        End Property

        Public Sub Dispose() Implements IDisposable.Dispose

            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

        ''' <summary>
        ''' Dispose All referenced and unreferenced manage objects
        ''' </summary>
        ''' <param name="disposing"></param>
        ''' <remarks></remarks>
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If disposing Then
                Me.Disposed = True REM Because of Async events

                If Me.Mailer IsNot Nothing Then Me.Mailer.Dispose()
                Me.Mailer = Nothing

                If Me.___ClientSocket IsNot Nothing Then _
                    If Me.___ClientSocket.Connected Then Me.___ClientSocket.Close()
                Me.___ClientSocket = Nothing

                If Me.MonitorInActivity IsNot Nothing AndAlso Not Me.MonitorInActivity.IsDisposed Then Me.MonitorInActivity.Dispose()
                Me.MonitorInActivity = Nothing

                Me.SocketBucketReceiver = Nothing

                Me.DisposeArranger()

            End If

        End Sub

        Protected Overrides Sub Finalize()
            Dispose(False)
        End Sub

#End Region


#Region "Methods"

        Private evTimeOutAction As dlgDoClientWentOffline
        Private Sub onTimeUpInActive()
            Me.evTimeOutAction(Me)
        End Sub

        Public Function SendMessage(ByVal Msg As String) As Boolean

            Return Me.SendMessage(New UserSimplePackage(Msg))

        End Function

        Friend Shared Function getAmAlivePacket() As SocketSimplePackage
            Return New SocketSimplePackage(CMD_AM_ALIVE)
        End Function

        Public Delegate Function dlgSendMessage(ByVal MSG As SimplePackage) As Boolean
        Friend Function SendMessage(ByVal MSG As SimplePackage) As Boolean

            If MSG IsNot Nothing AndAlso MSG.IsValid() Then

                REM if it is busy and message is am alive
                If MSG.PackageIdentifierName = ClientSocketWrapper.getAmAlivePacket().PackageIdentifierName Then
                    If Me.Mailer.isSendingFile Then Me.MonitorInActivity.UpdateLastAccessTime() : Return True
                    If BASE.IsDebugMode Then BASE.LocalLogger.Write("Confirmed Trying to send AM Alive")

                End If

                If BASE.IsDebugMode Then BASE.LocalLogger.Write("Sending: " & MSG.ToString())
                Dim SentResult As Boolean = Me.Mailer.Send(MSG.getBytes(), DataXchange.Packets.Layer1Packet.PayLoadDataTypes.STRING, True)

                If BASE.IsDebugMode Then BASE.LocalLogger.Write("SentResult: " & SentResult)

                Return SentResult
            End If


            Return False

        End Function

        Friend Function SendData(ByVal MSG As Byte()) As Boolean

            If MSG IsNot Nothing AndAlso MSG.Length > 0 Then

                If BASE.IsDebugMode Then BASE.LocalLogger.Write("Sending Data: " & MSG.Length)

                Return Me.Mailer.Send(MSG, DataXchange.Packets.Layer1Packet.PayLoadDataTypes.RAW_DATA, True)

            End If



            If BASE.IsDebugMode Then BASE.LocalLogger.Write("Am returning false on no data to send")
            Return False

        End Function



        Friend Function SendFile(ByVal FileFullPath As String,
                                 ByVal ErrNotifier As dlgSocketErrorMessage,
                                 ByVal ProgressNotifier As dlgFileTransferProgress,
                                 ByVal FileSentNotifier As BASE.dlgFileSent,
                                 ByVal FileSendingCancelledNotifier As BASE.dlgFileSent) As Boolean


            If FileFullPath IsNot Nothing AndAlso System.IO.File.Exists(FileFullPath) Then

                If BASE.IsDebugMode Then BASE.LocalLogger.Write("Sending File: " & FileFullPath)

                REM Max Allowed File transfer Size 1GB
                Return Me.Mailer.Send(FileFullPath, ErrNotifier, ProgressNotifier, FileSentNotifier, FileSendingCancelledNotifier)


            ElseIf ErrNotifier IsNot Nothing Then
                ErrNotifier("Invalid File Specified: " & FileFullPath)
            End If



            If BASE.IsDebugMode Then BASE.LocalLogger.Write("Am returning false on sendFile")
            Return False

        End Function


        ''' <summary>
        ''' Cancel current sending operation
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub CancelSending()
            If Me.Mailer.isSending() Then Me.Mailer.CancelSending()
        End Sub


        ''' <summary>
        ''' Returns false only if protocol is bridged
        ''' </summary>
        ''' <param name="ByteSizeReceived"></param>
        ''' <param name="FileReceivingCancelledNotifier"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Friend Function NotifyReceived(ByVal ByteSizeReceived As UInt16,
                                  ByVal FileReceivingCancelledNotifier As BASE.dlgFileSent) As Boolean
            Try

                Me.MonitorInActivity.UpdateLastAccessTime() REM Am active

                REM Copy the received bytes form the Socket Receiver
                If BASE.IsDebugMode Then BASE.LocalLogger.Write("Notify Received Bytes: " & ByteSizeReceived)
                Dim BytesReceived As Array = Array.CreateInstance(GetType(Byte), ByteSizeReceived)
                Array.Copy(Me.SocketBucketReceiver, BytesReceived, ByteSizeReceived)
                If BASE.IsDebugMode Then BASE.LocalLogger.Write("Notify BytesReceived after copy: " & BytesReceived.Length)


                REM Handle the Arrangement Here
                If Me.ReceivedPacketArranger IsNot Nothing Then
                    If BASE.IsDebugMode Then BASE.LocalLogger.Write("Me.ReceivedPacketArranger Is Nothing and BytesReceived.Length is " & BytesReceived.Length)
                    Do While BytesReceived.Length > 0


                        If Me.ReceivedPacketArranger.isDevelopingHeader Then
                            If BASE.IsDebugMode Then BASE.LocalLogger.Write("isDevelopingHeader true.")
                            BytesReceived = Me.ReceivedPacketArranger.ExtractHeaderRemaining(BytesReceived.Cast(Of Byte).ToArray())
                            If BASE.IsDebugMode Then BASE.LocalLogger.Write("BytesReceived after header extraction: " & BytesReceived.Length)
                            Me.ReceivedPacketArranger.CreateNextPacketFromHeader()

                        End If


                        If BASE.IsDebugMode Then BASE.LocalLogger.Write("Me.ReceivedPacketArranger.ReadyPackage.isValid(): " & Me.ReceivedPacketArranger.ReadyPackage.isValid())
                        If Not Me.ReceivedPacketArranger.ReadyPackage.isValid() Then Return Me.Disconnect()

                        If BASE.IsDebugMode Then BASE.LocalLogger.Write("ReceivedPacketArranger.hasPayloadCompletelyArrived: " & ReceivedPacketArranger.hasPayloadCompletelyArrived)
                        If Not ReceivedPacketArranger.hasPayloadCompletelyArrived AndAlso BytesReceived.Length > 0 Then
                            BytesReceived = Me.ReceivedPacketArranger.ExtractPayloadRemaining(BytesReceived.Cast(Of Byte).ToArray())
                            If BASE.IsDebugMode Then BASE.LocalLogger.Write("BytesReceived after payload extraction: " & BytesReceived.Length)
                        End If

                        If BASE.IsDebugMode Then BASE.LocalLogger.Write("ReceivedPacketArranger.hasPayloadCompletelyArrived: " & ReceivedPacketArranger.hasPayloadCompletelyArrived)
                        If ReceivedPacketArranger.hasPayloadCompletelyArrived Then _
                                Me.Mailer.Receive(Me.ReceivedPacketArranger.ReadyPackage, FileReceivingCancelledNotifier) : Me.DisposeArranger()

                        REM if there is remainder
                        REM Try to create new header from it
                        If BytesReceived.Length > 0 Then
                            If BASE.IsDebugMode Then BASE.LocalLogger.Write("BytesReceived Left: " & BytesReceived.Length)
                            Me.DisposeArranger()
                            Me.ReceivedPacketArranger = New ArrivalPacketArranger(New Byte() {})
                            If BASE.IsDebugMode Then BASE.LocalLogger.Write("Extracting new Header from the left packet: ")
                            BytesReceived = Me.ReceivedPacketArranger.ExtractHeaderRemaining(BytesReceived.Cast(Of Byte).ToArray())
                            If BASE.IsDebugMode Then BASE.LocalLogger.Write("BytesReceived after header extraction: " & BytesReceived.Length)
                        End If

                    Loop
                Else
                    REM first packet
                    If BytesReceived.Length < Layer1Packet.HEADER_BYTE_SIZE Then Return Me.Disconnect()
                    Dim FirstPacket As New Layer1Packet(BytesReceived.Cast(Of Byte).ToArray())
                    If Not FirstPacket.isValid() Then Return Me.Disconnect()

                    Me.DisposeArranger()
                    Me.ReceivedPacketArranger = New ArrivalPacketArranger(FirstPacket)

                    If BASE.IsDebugMode Then BASE.LocalLogger.Write("Creating new Arranger")
                    If BASE.IsDebugMode Then BASE.LocalLogger.Write("ReceivedPacketArranger.hasPayloadCompletelyArrived : " & ReceivedPacketArranger.hasPayloadCompletelyArrived)
                    If ReceivedPacketArranger.hasPayloadCompletelyArrived Then _
                        Me.Mailer.Receive(Me.ReceivedPacketArranger.ReadyPackage, FileReceivingCancelledNotifier) : Me.DisposeArranger()



                End If



                ''If Not Me.Mailer.Receive(BytesReceived.Cast(Of Byte).ToArray(), FileReceivingCancelledNotifier) Then
                REM DISCONNECT CONNECTION


                ''End If

                Me.CleanBucketReceiver()
                BytesReceived = Nothing

            Catch ex As Exception
                BASE.LocalLogger.Write(New EException("Error While processing Received Bytes: " & ex.Message, ex))
                Return False
            End Try

            Return True
        End Function


        Friend Sub NotifyFileTransferBegins(ByVal FileDetails As SocketCommandPackage,
                ByVal ErrNotifier As dlgSocketErrorMessage,
                ByVal ProgressNotifier As dlgFileTransferProgress)

            Me.Mailer.BeginReceivingFile(FileDetails,
                ErrNotifier,
                ProgressNotifier)

        End Sub


        ''' <summary>
        ''' Private Dispose and recreate the bucket receiver
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub CleanBucketReceiver()
            Me.SocketBucketReceiver = Nothing REM Clear it
            ReDim SocketBucketReceiver(CInt(Me.Maximum_Buffer_Size))
        End Sub

        Private Function Disconnect() As Boolean
DISCONNECT__CONNECTION:
            REM Protocol bridged
            Me.onTimeUpInActive()
            Return False
        End Function

        Private Sub DisposeArranger()
            If Me.ReceivedPacketArranger IsNot Nothing Then Me.ReceivedPacketArranger.Dispose()
            Me.ReceivedPacketArranger = Nothing
        End Sub

#End Region


#Region "Properties"

        ''' <summary>
        ''' Socket Information We are Transferring. Also I will use this to access client list on ClientsSockets Collection
        ''' </summary>
        ''' <remarks></remarks>
        Private ___ClientSocket As Socket = Nothing
        Public ReadOnly Property ClientSocket As Socket
            Get
                Return Me.___ClientSocket
            End Get
        End Property


        REM ON LAN, Socket Can Only send 1kb of data per transfer =1024Kb

        Private _Buffer_Size As Integer
        ''' <summary>
        ''' Maximum Size of data to send on the Socket Layer Per Time
        ''' </summary>
        ''' <remarks></remarks>
        Public ReadOnly Property Maximum_Buffer_Size As Integer
            Get
                Return Me._Buffer_Size
            End Get
        End Property

        ''' <summary>
        ''' Biggest space to Just use to receive directly from socket
        ''' </summary>
        ''' <remarks></remarks>
        Public SocketBucketReceiver() As Byte

        REM Normally as Server, All Socket save for clients has their keys equivalent to their handles
        REM But has a Client It's key is the ID sent from the Server
        ''' <summary>
        ''' Unique Key to Access this Client
        ''' </summary>
        ''' <remarks></remarks>
        Public ReadOnly Property SocketKey As Integer
            Get

                If Me.___ClientSocket IsNot Nothing Then Return Me.___ClientSocket.Handle.ToInt32
                Return 0
            End Get
        End Property


        '' ''' <summary>
        '' ''' This will be main effectuated on Client. Change only when Server sents a new one
        '' ''' </summary>
        '' ''' <value></value>
        '' ''' <returns></returns>
        '' ''' <remarks></remarks>
        ''Public Property UniqueID_ReceivedFrom_Server As Integer = 0

        ''' <summary>
        ''' Indicate if this class is still ok to send data. Mainly checking the socket only
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property CanSendData() As Boolean
            Get
                Return Not Me.isDisposed AndAlso Me.ClientSocket IsNot Nothing AndAlso Me.ClientSocket.Connected
            End Get
        End Property

        Private Property Mailer As DataXchange.Packets.Layer2Packet = Nothing

        Private Property ReceivedPacketArranger As DataXchange.Packets.ArrivalPacketArranger = Nothing

        Private Property MonitorInActivity As SocketActivityMonitor

        Public Property ClientInfo As PCInfo = Nothing

#End Region


#Region "Inner Classes"

        Public Class SocketActivityMonitor
            Implements IDisposable

            Sub New(ByVal __TimeUpNotifier As dlgInActivityTimeUp,
                    ByVal __MessageSendHandler As dlgSendMessage,
                    ByVal TimeOutinMillisecs As UInt16)

                Me.TimeUpNotifier = __TimeUpNotifier
                Me.TimeupMillisecs = TimeOutinMillisecs
                Me.MessageSendHandler = __MessageSendHandler

                Me.MyLocalTimer = New FixedSystemClock().Start()
                Me.LastAccessedTime = Me.MyLocalTimer.NOW
                Me.thrMonitor = New Threading.Thread(AddressOf Me.Run)
                Me.thrMonitor.IsBackground = True
                Me.thrMonitor.SetApartmentState(ApartmentState.MTA)
                Me.thrMonitor.Start()

            End Sub



#Region "Properties"

            Public Delegate Sub dlgInActivityTimeUp()

            Private TimeUpNotifier As dlgInActivityTimeUp
            Private MessageSendHandler As dlgSendMessage

            Private ______TimeupMillisecs As UShort
            Private Property TimeupMillisecs As UShort
                Get
                    Return Me.______TimeupMillisecs
                End Get
                Set(value As UShort)
                    If value < MINIMUM_TIME_OUT Then value = MINIMUM_TIME_OUT
                    Me.______TimeupMillisecs = value
                End Set
            End Property

            Private MyLocalTimer As FixedSystemClock
            Private LastAccessedTime As DateTime
            Private thrMonitor As System.Threading.Thread

            ''' <summary>
            ''' Minimum Timeout setting  10secs
            ''' </summary>
            ''' <remarks></remarks>
            Public Const MINIMUM_TIME_OUT As UShort = 20000

            Public Const DEFAULT__INACTIVE_TIMEOUT As UShort = 25000


            ''' <summary>
            ''' Time to start sending am alive packet
            ''' </summary>
            ''' <remarks></remarks>
            Private Const QUERY__ALIVE__TIME As UShort = 8000

            ''' <summary>
            ''' REM Time for checker to keep running 2secs
            ''' </summary>
            ''' <remarks></remarks>
            Private Const CHECKER__INTERVAL As UShort = 2000

#End Region


#Region "Method"


            Private Sub Run()

                Dim isTimingUpNormally As Boolean = False

                Try

                    While Not Me.disposedValue

                        If BASE.IsDebugMode Then BASE.LocalLogger.Write("Me.getLastAccessMillsecsDifference(): " & Me.getLastAccessMillsecsDifference())


                        If Me.getLastAccessMillsecsDifference() > Me.TimeupMillisecs Then
                            isTimingUpNormally = True
                            Exit While
                        Else
                            If Me.getLastAccessMillsecsDifference() > SocketActivityMonitor.QUERY__ALIVE__TIME Then
                                If BASE.IsDebugMode Then BASE.LocalLogger.Write("Trying to send AM Alive")
                                If Me.MessageSendHandler IsNot Nothing Then Me.MessageSendHandler(ClientSocketWrapper.getAmAlivePacket())

                            End If
                        End If
                        Thread.Sleep(CHECKER__INTERVAL)
                    End While

                Catch ex As ThreadAbortException
                    REM IGNORE
                    BASE.LocalLogger.Write(New EException(ex))
                Catch ex As Exception
                    BASE.LocalLogger.Write(New EException(ex))
                End Try


                If isTimingUpNormally Then
                    Me.Dispose(False)
                    If BASE.IsDebugMode Then BASE.LocalLogger.Write("Timing Up")
                    If Me.TimeUpNotifier IsNot Nothing Then Me.TimeUpNotifier()
                End If



            End Sub





            Private Function getLastAccessMillsecsDifference() As UShort

                SyncLock Me.LockUpdatingAndReadingAccessTime
                    If BASE.IsDebugMode Then BASE.LocalLogger.Write(String.Format("Me.LastAccessedTime: {0}, Me.MyLocalTimer.NOW: {1}, EDateTime.GetTimeDifferenceInMilliseconds(Me.LastAccessedTime, Me.MyLocalTimer.NOW): {2}",
                                                         Me.LastAccessedTime, Me.MyLocalTimer.NOW,
                                                         EDateTime.GetTimeDifferenceInMilliseconds(Me.LastAccessedTime, Me.MyLocalTimer.NOW))
                                                     )
                    Return CUShort(EDateTime.GetTimeDifferenceInMilliseconds(Me.LastAccessedTime, Me.MyLocalTimer.NOW))

                End SyncLock

            End Function


            Private LockUpdatingAndReadingAccessTime As Object = New Object()
            Public Sub UpdateLastAccessTime()
                SyncLock Me.LockUpdatingAndReadingAccessTime
                    Me.LastAccessedTime = Me.MyLocalTimer.NOW
                End SyncLock
            End Sub


#End Region



#Region "IDisposable Support"
            Private disposedValue As Boolean ' To detect redundant calls

            ' IDisposable
            Protected Overridable Sub Dispose(disposing As Boolean)
                If Not Me.disposedValue Then
                    If disposing Then
                        Try

                        
                        ' TODO: dispose managed state (managed objects).
                            If Me.thrMonitor IsNot Nothing AndAlso Me.thrMonitor.IsAlive Then Me.thrMonitor.Abort()
                            Me.thrMonitor = Nothing

                        Catch ex As Exception
                            BASE.LocalLogger.Write(New EException("Am not expecting exception here: ", ex))
                        End Try
                    End If

                    ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                    ' TODO: set large fields to null.

                    If Me.MyLocalTimer IsNot Nothing Then Me.MyLocalTimer.Dispose()
                    Me.MyLocalTimer = Nothing

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
                ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
                Dispose(True)
                GC.SuppressFinalize(Me)
            End Sub

            Public ReadOnly Property IsDisposed() As Boolean
                Get
                    Return Me.disposedValue
                End Get
            End Property

#End Region

        End Class

#End Region


    End Class


End Namespace

