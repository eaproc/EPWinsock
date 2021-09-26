Imports System.Net.Sockets
Imports System.Text
Imports System.IO

Imports EPWinSock.v4.NET
Imports EPWinSock.v4.BASE
Imports CODERiT.Logger.v._3._5.Exceptions
Imports EPWinSock.v4.Utilities
Imports EPWinSock.v4.DataXchange.StringPackages
Imports EPRO.Library.v3._5

Namespace DataXchange.Packets

    Friend Class Layer2Packet
        Implements IDisposable

#Region "Structures"


        Public Structure FRAME
            Implements IDisposable


            Sub New(ByVal RawData As Byte(),
                    ByVal FileName As String,
                    ByVal DataType As Layer1Packet.PayLoadDataTypes)

                Me._____DataType = DataType
                Me.___FileName = FileName
                Me.____RawData = RawData

            End Sub

            Sub New(ByVal RawData As Byte(),
                ByVal FileName As String,
                ByVal DataType As Layer1Packet.PayLoadDataTypes,
                ByVal ErrNotifier As dlgSocketErrorMessage,
                ByVal ProgressNotifier As dlgFileTransferProgress,
                ByVal FileSentNotifier As BASE.dlgFileSent,
                ByVal FileSendingCancelledNotifier As BASE.dlgFileSent)

                Me._____DataType = DataType
                Me.___FileName = FileName
                Me.____RawData = RawData
                Me.ErrNotifier = ErrNotifier
                Me.ProgressNotifier = ProgressNotifier
                Me.FileSentNotifier = FileSentNotifier
                Me.FileSendingCancelledNotifier = FileSendingCancelledNotifier

            End Sub
            


            Friend ErrNotifier As dlgSocketErrorMessage


            Friend ProgressNotifier As dlgFileTransferProgress


            Friend FileSentNotifier As BASE.dlgFileSent

            Friend FileSendingCancelledNotifier As BASE.dlgFileSent



            Private _____DataType As Layer1Packet.PayLoadDataTypes
            Public ReadOnly Property DataType As Layer1Packet.PayLoadDataTypes
                Get
                    Return Me._____DataType
                End Get
            End Property


            Private ___FileName As String
            ''' <summary>
            ''' Original File Name [name only] received
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public ReadOnly Property FileName As String
                Get
                    Return Me.___FileName
                End Get
            End Property

            ''' <summary>
            ''' Returns the TempFile Full Name Path
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public ReadOnly Property LocalFileStorageFullPath As String
                Get
                    Throw New NotImplementedException()
                End Get
            End Property

            Private ____RawData As Byte()
            Public ReadOnly Property RawData As Byte()
                Get
                    Return Me.____RawData
                End Get
            End Property



            Public Sub Accummulate(ByVal __data As Byte())
                Try


                    Me.____RawData = Me.RawData.Concat(__data).ToArray()

                Catch ex As Exception
                    REM Probably out of memory
                    BASE.LocalLogger.Write(New EException(ex))
                End Try
            End Sub


            Public Sub Dispose() Implements IDisposable.Dispose
                Me.____RawData = Nothing
            End Sub
        End Structure


#End Region

#Region "Constructors"

        '   Serves as accumulator and disseminator 
        '   It is responsible for chunking data into the the size needed for each packet
        '   It is also responsible for accumulating data till it is ready to be received
        '
        Sub New(ByRef __Parent As NET.ClientSocketWrapper,
                ByVal ReceiverHandler As dlgProcessReceiveDataHandler,
                ByVal FileReceiverHandler As dlgProcessReceiveFileHandler)

            Me.____isSending = False
            Me.Parent = __Parent
            Me.processReceiveDataHandler = ReceiverHandler
            Me.processReceiveFileHandler = FileReceiverHandler
        End Sub


#End Region

#Region "Properties"

        Private Parent As NET.ClientSocketWrapper = Nothing

        Private ____isSending As Boolean
        Public ReadOnly Property isSending As Boolean
            Get
                Return Me.____isSending
            End Get
        End Property

        Public ReadOnly Property isSendingFile As Boolean
            Get
                Return Me.isSending AndAlso frSendingFrame.DataType = Layer1Packet.PayLoadDataTypes.FILE
            End Get
        End Property

        Private thrSendingThread As Threading.Thread
        Private frSendingFrame As FRAME

        Private ____isSendingCancelledLock As New Object
        Private ____isSendingCancelled As Boolean = False

        Private frReceivingFrame As FRAME
        Private hwndReceivingFileHandler As FileAccumulator



        Private processReceiveDataHandler As dlgProcessReceiveDataHandler = Nothing
        Private processReceiveFileHandler As dlgProcessReceiveFileHandler = Nothing



        Private ReadOnly Property CanSend() As Boolean
            Get
                Return Me.Parent IsNot Nothing AndAlso Not Me.Parent.isDisposed AndAlso Not Me.isSending
            End Get
        End Property

        Private ReadOnly Property AvailablePayLoadSize As UInt16
            Get
                Return CUShort((Me.Parent.Maximum_Buffer_Size - Layer1Packet.HEADER_BYTE_SIZE))
            End Get
        End Property


        Private ReadOnly Property isReceivingFile As Boolean
            Get
                Return Me.hwndReceivingFileHandler IsNot Nothing
            End Get
        End Property


#End Region



#Region "Method Private"

        Private Sub runKeepSendingPackets()
            Try

                REM If thread arrived late .. Not really possible. Theoretically
                If Me.isSending() Then Return

                Me.____isSending = True  REM LOCK

                Dim RequiredPacketCount As UInt16 = 0, CurrentIndex As UInt16 = 0, DataLength As Int32 = 0
                REM Calculate required packets
                Select Case Me.frSendingFrame.DataType
                    Case Layer1Packet.PayLoadDataTypes.FILE
                        REM Get File Size
                        REM I hope the file exist here
                        DataLength = CInt(New System.IO.FileInfo(Me.frSendingFrame.FileName).Length)
                        RequiredPacketCount = Convert.ToUInt16(
                                        Math.Ceiling(
                                            DataLength / Me.AvailablePayLoadSize
                                            )
                                        )

                    Case Else
                        REM 
                        DataLength = Me.frSendingFrame.RawData.Length
                        RequiredPacketCount = Convert.ToUInt16(
                                        Math.Ceiling(DataLength / Me.AvailablePayLoadSize)
                                        )


                End Select


                If BASE.IsDebugMode Then BASE.LocalLogger.Write(String.Format("DataLength: {0}, RequiredPacketCount: {1}, Me.AvailablePayLoadSize: {2}",
                                                                    DataLength, RequiredPacketCount, Me.AvailablePayLoadSize))


                For CurrentIndex = 0 To CUShort(RequiredPacketCount - 1)

                    REM Sync Lock cancel read write
                    Dim __LocalRead__is_SendingCancled As Boolean = Me.IsSendingCancelled
                    REM  If Me.isSendingCancelled Then Return

                    Dim TotalLengthAssumedSent As Int32 = Convert.ToInt32(CInt(CurrentIndex) * CInt(Me.AvailablePayLoadSize))
                    Dim TotalLengthToBeginSending As Int32 = Convert.ToInt32(CInt(CurrentIndex) * CInt(Me.AvailablePayLoadSize))

                    Dim buffer As Byte() = Nothing
                    Dim NextLengthToCopy As UInt16 = Me.AvailablePayLoadSize

                    If CurrentIndex = CUShort(RequiredPacketCount - 1) Then
                        NextLengthToCopy = CUShort(DataLength - CInt(CInt(RequiredPacketCount - 1) * CInt(Me.AvailablePayLoadSize)))
                        TotalLengthAssumedSent = DataLength REM Since this is the last batch
                    End If

                    If BASE.IsDebugMode Then BASE.LocalLogger.Write("CurrentIndex * Me.AvailablePayLoadSize: " &
                                                        TotalLengthAssumedSent.ToString() &
                                                  ", Me.frSendingFrame.RawData: " & DataLength.ToString() &
                                                  ", NextLengthToCopy: " & NextLengthToCopy.ToString() &
                                                  ", CurrentIndex: " & CurrentIndex.ToString()
                                                  )

                    buffer = Array.CreateInstance(GetType(Byte), NextLengthToCopy).Cast(Of Byte).ToArray()

                    Select Case Me.frSendingFrame.DataType
                        Case Layer1Packet.PayLoadDataTypes.FILE
                            REM Get File Buffer
                            REM I hope the file exist here
                            Try

                                REM Read Next Buffer
                                buffer = Me.ReadFileChunks(frSendingFrame.FileName, TotalLengthToBeginSending,
                                                                                                        NextLengthToCopy)

                            Catch ex As Exception
                                REM Error Occurred Sending File
                                BASE.LocalLogger.Write(New EException("Error Reading File for sending: ", ex))
                                If frSendingFrame.ErrNotifier IsNot Nothing Then frSendingFrame.ErrNotifier(New EException("Error Reading File for sending: ", ex).Message)

                                REM Fake Finish
                                REM CurrentIndex = CUShort(RequiredPacketCount - 1)
                                buffer = New Byte() {0, 0}
                                Me.CancelSending()

                            End Try

                        Case Else
                            REM Read Next Buffer
                            Array.Copy(Me.frSendingFrame.RawData, TotalLengthToBeginSending,
                                       buffer, 0, NextLengthToCopy)

                    End Select

                    If BASE.IsDebugMode Then BASE.LocalLogger.Write("__LocalRead__is_SendingCancled before packing Layer1 Packet: " & __LocalRead__is_SendingCancled)
                    REM Create Layer1 raw to send
                    REM Other Side is inform in this packet  if the transmission is cancelled
                    Dim Layer1Pkt As New Layer1Packet(buffer, RequiredPacketCount, CurrentIndex,
                                                      __LocalRead__is_SendingCancled, Me.frSendingFrame.DataType)


                    ''If BASE.IsDebugMode Then BASE.LocalLogger.Write("Layer1Pkt isValid(): " & Layer1Pkt.isValid())
                    If Not Me.RawSend(Layer1Pkt.getBytes(), Me.Parent.ClientSocket) Then
                        REM Log Error Sending
                        If BASE.IsDebugMode Then BASE.LocalLogger.Write("Packet Not Sent")
                        Exit For
                    End If



                    If frSendingFrame.DataType = Layer1Packet.PayLoadDataTypes.FILE AndAlso frSendingFrame.ProgressNotifier IsNot Nothing Then

                        frSendingFrame.ProgressNotifier(frSendingFrame.FileName, TotalLengthAssumedSent, DataLength, Me.Parent.SocketKey)

                    End If



                    If BASE.IsDebugMode Then BASE.LocalLogger.Write("__LocalRead__is_SendingCancled after sending Layer1 Packet: " & __LocalRead__is_SendingCancled)
                    If __LocalRead__is_SendingCancled Then Exit For



                Next






            Catch ex As Exception
                If frSendingFrame.ErrNotifier IsNot Nothing Then frSendingFrame.ErrNotifier(New EException("Exception while Sending: ", ex).Message)
                BASE.LocalLogger.Write(New EException("Exception while Sending: ", ex).Message)
            End Try







            REM Not user can try to send stuff immediately after on this notification
            If Me.frSendingFrame.DataType = Layer1Packet.PayLoadDataTypes.FILE AndAlso
               Me.frSendingFrame.FileSendingCancelledNotifier IsNot Nothing AndAlso Me.isSendingCancelled Then

                Me.SetIsSendingCancelledToFalse() REM Normalize it
                Me.____isSending = False REM Unlock

                If BASE.IsDebugMode Then BASE.LocalLogger.Write("Cancelling Sending: ")
                Me.frSendingFrame.FileSendingCancelledNotifier(Me.Parent.SocketKey)


            ElseIf Me.frSendingFrame.DataType = Layer1Packet.PayLoadDataTypes.FILE AndAlso
               Me.frSendingFrame.FileSentNotifier IsNot Nothing AndAlso Not Me.isSendingCancelled Then
                Me.____isSending = False REM Unlock
                Me.frSendingFrame.FileSentNotifier(Me.Parent.SocketKey)

            Else

                REM Just nullify it
                Me.SetIsSendingCancelledToFalse()  REM Normalize it
                Me.____isSending = False REM Unlock

            End If




            ''If Me.frSendingFrame.DataType = Layer1Packet.PayLoadDataTypes.FILE AndAlso
            ''    Me.frSendingFrame.FileSentNotifier IsNot Nothing Then Me.frSendingFrame.FileSentNotifier(Me.Parent.SocketKey)
            ''REM Note after notify you can't use this structure (frsendingFrame) again because it will be disposed on new send method


            If BASE.IsDebugMode Then BASE.LocalLogger.Write("Me.isSending: __on__thread: " & Me.isSending)
            If BASE.IsDebugMode Then BASE.LocalLogger.Write("Me.isSendingCancelled : " & Me.isSendingCancelled)


        End Sub


        ''' <summary>
        ''' Send Message Accross Sockets. Threaded
        ''' </summary>
        ''' <param name="Sock"></param>
        ''' <remarks></remarks>
        Private Function RawSend(
                                    ByVal Buffer As Byte(),
                                    ByVal Sock As Socket
                                                                   ) As Boolean

            REM If I truly care about its delivery then I Should use send asynchronous

            If Sock IsNot Nothing AndAlso Sock.Connected AndAlso Buffer IsNot Nothing AndAlso Buffer.Length > 0 Then
                Try

                    REM  Thread.Sleep(DELAY_SOCKET_TRANSFER_MILLISECS)
                    If BASE.IsDebugMode Then BASE.LocalLogger.Write("Buffer to Send Size: " & Buffer.Length) REM Should be the size of Header + Payload

                    Dim BytesSent As Integer = Sock.Send(Buffer)

                    REM Make sure the stream is cleared off
                    REM 


                    If BASE.IsDebugMode Then BASE.LocalLogger.Write("BytesSent: " & BytesSent)

                    
                    Return True
                Catch ex As Exception
                    BASE.LocalLogger.Write("Error Sending RawSend: " & ex.Message)
                End Try

            End If

            Return False
        End Function



        Private Sub ClearSendingFrame()

            If Not IsNothing(Me.frSendingFrame) Then Me.frSendingFrame.Dispose()
            Me.frSendingFrame = Nothing

        End Sub


        Private Sub ClearFileReceiving()

            If Me.hwndReceivingFileHandler IsNot Nothing Then Me.hwndReceivingFileHandler.Dispose()
            Me.hwndReceivingFileHandler = Nothing

        End Sub

        Private Sub ClearDataReceiving()
            If Not IsNothing(Me.frReceivingFrame) Then frReceivingFrame.Dispose()
            frReceivingFrame = Nothing
        End Sub

        Private Function ReadFileChunks(ByVal FileName As String, ByVal BeginFromTotalAlreadyReady As Int32,
                                 ByVal FileLengthToRead As Int32) As Byte()

            Try

                If File.Exists(FileName) Then

                    Using fs As FileStream = New FileStream(FileName, FileMode.Open)

                        If fs.CanSeek Then
                            fs.Seek(BeginFromTotalAlreadyReady, SeekOrigin.Begin)
                        End If


                        Dim bBuffer As Byte() = Array.CreateInstance(GetType(Byte), FileLengthToRead).Cast(Of Byte).ToArray()
                        REM ReDim bBuffer(FileLengthToRead) REM When you use redim. you will put the UBound
                        fs.Read(bBuffer, 0, FileLengthToRead)

                        fs.Close()

                        Return bBuffer
                    End Using

                End If

            Catch ex As Exception

            End Try


            Return New Byte() {}

        End Function



#End Region


#Region "Methods Public"

        Public Sub BeginReceivingFile(FileDetails As StringPackages.SocketCommandPackage,
                ByVal ErrNotifier As dlgSocketErrorMessage,
                ByVal ProgressNotifier As dlgFileTransferProgress)
            Me.hwndReceivingFileHandler = New FileAccumulator(FileDetails.Parameters(0), CInt(FileDetails.Parameters(1)),
                                                              ErrNotifier, ProgressNotifier
                                                              )
        End Sub



        Private ReadOnly Property IsSendingCancelled As Boolean
            Get
                SyncLock Me.____isSendingCancelledLock
                    Return Me.____isSendingCancelled
                End SyncLock
            End Get
        End Property
        Public Sub CancelSending()
            SyncLock Me.____isSendingCancelledLock
                Me.____isSendingCancelled = True
            End SyncLock
        End Sub

        Public Sub SetIsSendingCancelledToFalse()
            SyncLock Me.____isSendingCancelledLock
                Me.____isSendingCancelled = False
            End SyncLock
        End Sub



        ''' <summary>
        ''' Use to send Raw data and String
        ''' </summary>
        ''' <param name="__Load"></param>
        ''' <param name="__LoadType"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Send(ByVal __Load As Byte(),
                             Optional ByVal __LoadType As DataXchange.Packets.Layer1Packet.PayLoadDataTypes = Layer1Packet.PayLoadDataTypes.STRING,
                             Optional ByVal Sychronous As Boolean = False) As Boolean


            If BASE.IsDebugMode Then BASE.LocalLogger.Write("About Trying CanSend is true: ")
            If BASE.IsDebugMode Then BASE.LocalLogger.Write("Me.isSending: : " & Me.isSending)
            If Not Me.CanSend() Then Return False
            If BASE.IsDebugMode Then BASE.LocalLogger.Write("Trying CanSend is true: Succeeded")


            Me.ClearSendingFrame()
            Me.frSendingFrame = New FRAME(__Load, Nothing, __LoadType)



            If BASE.IsDebugMode And frSendingFrame.DataType = Layer1Packet.PayLoadDataTypes.STRING Then _
                BASE.LocalLogger.Write("CanSend is true: Sending: " & UTF8Encoding.UTF8.GetString(frSendingFrame.RawData))

            REM If Sychronous
            If Sychronous Then
                Me.runKeepSendingPackets()

            Else

                With New Threading.Thread(AddressOf Me.runKeepSendingPackets)
                    .SetApartmentState(Threading.ApartmentState.MTA)
                    .IsBackground = True
                    .Start()
                End With

            End If



            Return True
        End Function

        Public Function Send(ByVal FileFullPath As String,
                                 ByVal ErrNotifier As dlgSocketErrorMessage,
                                 ByVal ProgressNotifier As dlgFileTransferProgress,
                                 ByVal FileSentNotifier As BASE.dlgFileSent,
                                 ByVal FileSendingCancelledNotifier As BASE.dlgFileSent) As Boolean

            If Not Me.CanSend() Then Return False

            If BASE.IsDebugMode Then BASE.LocalLogger.Write("Sending File Alerter: ")

            REM Alert Receiver
            If Me.Send(
                New SocketCommandPackage(CMD_FILE_TRANSFER_BEGIN, SocketCommandPackage.DEFAULT____PARAM___DELIMITER,
                                                  EIO.getFileName(FileFullPath), New FileInfo(FileFullPath).Length.ToString()).getBytes(), , True
                                             ) Then

                REM Begin transfer
                Me.ClearSendingFrame()
                Me.frSendingFrame = New FRAME(Nothing, FileFullPath, Layer1Packet.PayLoadDataTypes.FILE, ErrNotifier, ProgressNotifier,
                                              FileSentNotifier,
                                              FileSendingCancelledNotifier)

                REM Asynchronously
                With New Threading.Thread(AddressOf Me.runKeepSendingPackets)
                    .SetApartmentState(Threading.ApartmentState.MTA)
                    .IsBackground = True
                    .Start()
                End With

                If BASE.IsDebugMode Then BASE.LocalLogger.Write(String.Format("Launching File Thread: {0} ", frSendingFrame.FileName))

                Return True

            ElseIf ErrNotifier IsNot Nothing Then
                ErrNotifier("Socket could not initiate File transfer")
            End If

            Return False
        End Function


        ''' <summary>
        ''' Returns false only if the packet does not conform with the protocol
        ''' </summary>
        ''' <param name="FileReceivingCancelledNotifier"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Receive(ByVal ReceivedPkt As Layer1Packet,
                           ByVal FileReceivingCancelledNotifier As BASE.dlgFileSent) As Boolean

            ''Dim ReceivedPkt As New Layer1Packet(BytesReceived)
            ''If BASE.IsDebugMode Then BASE.LocalLogger.Write("Bytes Received: " & BytesReceived.Length)

            If Not ReceivedPkt.isValid() Then Return False

            If ReceivedPkt.CancelTransmission Then
                Me.ClearFileReceiving()
                Me.ClearDataReceiving()
                REM Call Received Cancelled
                If FileReceivingCancelledNotifier IsNot Nothing Then FileReceivingCancelledNotifier(Me.Parent.SocketKey)

                If BASE.IsDebugMode Then BASE.LocalLogger.Write("Cancel Transmission Received: " & ReceivedPkt.AcquiredPayloadSize)

                Return True   REM Transmission Cancelled
            End If


            If ReceivedPkt.isValid() AndAlso Not ReceivedPkt.PayLoadDatype = Layer1Packet.PayLoadDataTypes.FILE Then

                REM IF this is suppose to be a file packet
                If Me.isReceivingFile Then

                    BASE.LocalLogger.Write("This is very bad. I was expecting a file packet")
                    BASE.LocalLogger.Write("Packet Received: " & UTF8Encoding.UTF8.GetString(ReceivedPkt.getBytes()))
                    Me.ClearFileReceiving()
                    Return True
                End If

                If ReceivedPkt.IsFirstPacket Then
                    REM In sending we parse into frame payload+headers
                    REM In receiving we parse into frame only payload
                    Me.frReceivingFrame = New FRAME(ReceivedPkt.Payload, Nothing, ReceivedPkt.PayLoadDatype)
                Else
                    Me.frReceivingFrame.Accummulate(ReceivedPkt.Payload)

                End If


                If ReceivedPkt.IsLastFragment Then

                    REM Send the Full Pa
                    If BASE.IsDebugMode Then BASE.LocalLogger.Write("DATA Received: " & UTF8Encoding.UTF8.GetString(frReceivingFrame.RawData))
                    If Me.processReceiveDataHandler IsNot Nothing Then Me.processReceiveDataHandler(Me.Parent, frReceivingFrame)

                    REM Clean UP
                    Me.ClearDataReceiving()

                End If


            ElseIf ReceivedPkt.isValid() Then
                REM For File
                REM There should file accumulator working now
                If Me.isReceivingFile Then

                    Me.hwndReceivingFileHandler.Accumulate(ReceivedPkt.Payload)

                    If BASE.IsDebugMode Then BASE.LocalLogger.Write("File Received: " & ReceivedPkt.Payload.Length)
                    If BASE.IsDebugMode Then BASE.LocalLogger.Write("Payload Size Defined in Header: " & ReceivedPkt.PayLoadSize)
                    If Me.hwndReceivingFileHandler.ProgressNotifier IsNot Nothing Then _
                        Me.hwndReceivingFileHandler.ProgressNotifier(hwndReceivingFileHandler.OriginalFileName,
                                        hwndReceivingFileHandler.TempFileFileSize, hwndReceivingFileHandler.OriginalFileSize,
                                        Me.Parent.SocketKey)
                    If ReceivedPkt.IsLastFragment Then
                        REM Inform user and clear receiving
                        Dim aCopyOfHandler As FileAccumulator = Me.hwndReceivingFileHandler
                        If Me.processReceiveFileHandler IsNot Nothing Then Me.processReceiveFileHandler(Me.Parent, aCopyOfHandler)
                        REM Me.ClearFileReceiving()
                        Me.hwndReceivingFileHandler = Nothing  REM only remove the pointer not dispose it
                    End If
                Else

                    REM Ignore packet
                    BASE.LocalLogger.Write("Received a File Packet while not expecting one")

                End If
            End If

            Return True
        End Function


#End Region



#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    If Me.thrSendingThread IsNot Nothing AndAlso Me.thrSendingThread.IsAlive Then Me.thrSendingThread.Abort()
                    Me.thrSendingThread = Nothing
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
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region



    End Class


End Namespace
