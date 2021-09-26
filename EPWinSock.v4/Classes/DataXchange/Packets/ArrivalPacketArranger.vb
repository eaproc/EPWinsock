
Namespace DataXchange.Packets

    Friend Class ArrivalPacketArranger
        Implements IDisposable


        Sub New(ByVal InitialPacket As Layer1Packet)
            Me._ReadyPackage = InitialPacket
        End Sub

        Sub New(ByVal MalformedHeader As Byte())
            Me.DevelopingHeaderBytes = MalformedHeader
        End Sub




        ''' <summary>
        ''' Returns True if ReadyPackage is Nothing
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property isDevelopingHeader As Boolean
            Get
                Return Me.ReadyPackage Is Nothing
            End Get
        End Property


        Private _ReadyPackage As Layer1Packet = Nothing
        Public ReadOnly Property ReadyPackage As Layer1Packet
            Get
                Return Me._ReadyPackage
            End Get
        End Property


        ''' <summary>
        ''' Confirms Acquired Payload Size is equivalent to the Expected Size
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property hasPayloadCompletelyArrived As Boolean
            Get
                If Me.ReadyPackage IsNot Nothing Then Return Me.ReadyPackage.hasPayloadCompletelyArrived

                Return False

            End Get
        End Property



        Private DevelopingHeaderBytes As Byte()

        ''' <summary>
        ''' Extract the remaining header bytes and return the remaining bytes
        ''' </summary>
        ''' <param name="BytesReceived"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ExtractHeaderRemaining(ByVal BytesReceived As Byte()) As Byte()

            ''BASE.LocalLogger.Write("Me.HeaderByteLeft > BytesReceived.Length: " &
            ''                       (Me.HeaderByteLeft > BytesReceived.Length).ToString())
            If Me.DevelopingHeaderBytes Is Nothing OrElse BytesReceived Is Nothing OrElse
                Me.HeaderByteLeft > BytesReceived.Length Then Return BytesReceived

            Dim ___LocalHeaderByteLeft As UShort = Me.HeaderByteLeft

            If BASE.IsDebugMode Then BASE.LocalLogger.Write("Extracting Header, ___LocalHeaderByteLeft: " & ___LocalHeaderByteLeft)
            Dim TempArr As Array = Array.CreateInstance(GetType(Byte), ___LocalHeaderByteLeft)
            Array.Copy(BytesReceived, 0, TempArr, 0, TempArr.Length)

            Me.DevelopingHeaderBytes = Me.DevelopingHeaderBytes.Concat(TempArr.Cast(Of Byte)).ToArray()

            REM HeaderByte Left will be zero since header as been filled
            TempArr = Nothing
            If (BytesReceived.Length - ___LocalHeaderByteLeft > 0) Then
                TempArr = Array.CreateInstance(GetType(Byte), BytesReceived.Length - ___LocalHeaderByteLeft)
                Array.Copy(BytesReceived, ___LocalHeaderByteLeft, TempArr, 0, BytesReceived.Length - ___LocalHeaderByteLeft)

                Return TempArr.Cast(Of Byte).ToArray()
            End If

            Return New Byte() {}
        End Function

        ''' <summary>
        ''' Sap out the bytes we need for payload from this bytes
        ''' </summary>
        ''' <param name="BytesReceived"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ExtractPayloadRemaining(ByVal BytesReceived As Byte()) As Byte()

            If Me.ReadyPackage Is Nothing OrElse BytesReceived Is Nothing OrElse BytesReceived.Length = 0 OrElse
                Not Me.ReadyPackage.isValid() Then Return New Byte() {}

            Dim pSizeToCopy As UInt16 = Me.ReadyPackage.AwaitingPayloadByteSizeRemaining
            If BytesReceived.Length < pSizeToCopy Then pSizeToCopy = CUShort(BytesReceived.Length)


            Dim TempArr As Array = Array.CreateInstance(GetType(Byte), pSizeToCopy)
            Array.Copy(BytesReceived, 0, TempArr, 0, TempArr.Length)

            Me.ReadyPackage.AccummulatePayload(TempArr.Cast(Of Byte).ToArray())

            TempArr = Nothing
            If (BytesReceived.Length - pSizeToCopy > 0) Then
                TempArr = Array.CreateInstance(GetType(Byte), BytesReceived.Length - pSizeToCopy)
                Array.Copy(BytesReceived, pSizeToCopy, TempArr, 0, BytesReceived.Length - pSizeToCopy)

                Return TempArr.Cast(Of Byte).ToArray()
            End If

            Return New Byte() {}
        End Function

        Private ReadOnly Property IsDevelopingHeaderWellFormed As Boolean
            Get
                Return Me.isDevelopingHeader AndAlso Me.DevelopingHeaderBytes IsNot Nothing AndAlso
                    Me.DevelopingHeaderBytes.Length = Layer1Packet.HEADER_BYTE_SIZE

            End Get
        End Property

        Private ReadOnly Property HeaderByteLeft As UInt16
            Get
                If Me.DevelopingHeaderBytes Is Nothing Then Return 0
                Return CUShort(Layer1Packet.HEADER_BYTE_SIZE - Me.DevelopingHeaderBytes.Length)
            End Get
        End Property


        Public Function CreateNextPacketFromHeader() As Boolean
            Me.DisposeCurrentPacket()
            If Me.isDevelopingHeader AndAlso Me.IsDevelopingHeaderWellFormed Then
                Me._ReadyPackage = New Layer1Packet(Me.DevelopingHeaderBytes)
                Return True
            End If
            Return False
        End Function


        Private Sub DisposeCurrentPacket()
            If Me.ReadyPackage IsNot Nothing Then Me.ReadyPackage.dispose()
            Me._ReadyPackage = Nothing
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
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

