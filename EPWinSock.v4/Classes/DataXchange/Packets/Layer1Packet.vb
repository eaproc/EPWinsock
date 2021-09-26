
Namespace DataXchange.Packets

    Friend Class Layer1Packet
        Implements IDisposable


#Region "Constructors"
        ''' <summary>
        ''' For receiving
        ''' </summary>
        ''' <param name="__bytes"></param>
        ''' <remarks></remarks>
        Sub New(ByVal __bytes As Byte())
            Me.__Payload = New Byte() {}


            If __bytes Is Nothing OrElse __bytes.Count < HEADER_BYTE_SIZE Then Return


            Dim bHeader As Array = Array.CreateInstance(GetType(Byte), HEADER_BYTE_SIZE)
            Array.Copy(__bytes, bHeader, HEADER_BYTE_SIZE)

            Dim sHeader As String = System.Text.ASCIIEncoding.ASCII.GetString(bHeader.Cast(Of Byte).ToArray())

            Try


                '   Header Order
                '   CancelTransmission|PayLoadDataType|TotalFragmentCount|FragmentIndex|PayLoadSize
                '   1                 | 1             |5                 | 5            | 5
                Me.__CancelTransmission = Convert.ToBoolean(Convert.ToByte(sHeader.Substring(0, 1)))
                Me.__PayLoadDatype = CType(Convert.ToByte(sHeader.Substring(1, 1)), PayLoadDataTypes)
                Me.__TotalFragmentCount = Convert.ToUInt16(sHeader.Substring(2, 5))
                Me.__FragmentIndex = Convert.ToUInt16(sHeader.Substring(7, 5))
                Me.__PayLoadSize = Convert.ToUInt16(sHeader.Substring(12, 5))
                Me.__Payload = Array.CreateInstance(GetType(Byte), Me.PayLoadSize).Cast(Of Byte).ToArray()
                Me.__AcquiredPayloadSize = CUShort(__bytes.Length - HEADER_BYTE_SIZE)
            Catch ex As Exception
                BASE.LocalLogger.Print("INVALID PACKET RECEIVED: ", ex)
                REM Protocol bridged. Stop the connection
                Return
            End Try

            If BASE.IsDebugMode Then BASE.LocalLogger.Write("L1 __ctor PayLoad Size parsed: " & Me.PayLoadSize)
            If BASE.IsDebugMode Then BASE.LocalLogger.Write("L1 __ctor Real PayLoad in bank: " & Me.AcquiredPayloadSize.ToString())

            If Me.PayLoadSize > 0 Then _
                Array.Copy(__bytes, HEADER_BYTE_SIZE, Me.__Payload, 0, Me.AcquiredPayloadSize) REM Copy only acquired


            Me.__isValid = True

        End Sub

        ''' <summary>
        ''' for sending
        ''' </summary>
        ''' <param name="____payload"></param>
        ''' <param name="____TotalFragmentCount"></param>
        ''' <param name="____FragmentIndex"></param>
        ''' <param name="____CancelTransmission"></param>
        ''' <param name="____PayLoadDataType"></param>
        ''' <remarks></remarks>
        Sub New(ByVal ____payload As Byte(),
                ByVal ____TotalFragmentCount As UInt16,
                ByVal ____FragmentIndex As UInt16,
                 Optional ByVal ____CancelTransmission As Boolean = False,
                 Optional ByVal ____PayLoadDataType As PayLoadDataTypes = PayLoadDataTypes.STRING
                 )

            '   Header Order
            '   CancelTransmission|PayLoadDataType|TotalFragmentCount|FragmentIndex|PayLoadSize
            '   1                 | 1             |5                 | 5            | 5

            Me.__CancelTransmission = ____CancelTransmission
            Me.__PayLoadDatype = ____PayLoadDataType
            Me.__TotalFragmentCount = ____TotalFragmentCount
            Me.__FragmentIndex = ____FragmentIndex
            Me.__PayLoadSize = CUShort(____payload.Length)

            Me.__Payload = ____payload


            Me.__isValid = Not (Me.Payload Is Nothing OrElse Me.Payload.Count = 0)

        End Sub

#End Region


        Public Enum PayLoadDataTypes
            ''' <summary>
            ''' Just File Content Description in string
            ''' </summary>
            ''' <remarks></remarks>
            [FILE] = 0
            ''' <summary>
            ''' Binaries
            ''' </summary>
            ''' <remarks></remarks>
            [RAW_DATA] = 1
            ''' <summary>
            ''' Other strings
            ''' </summary>
            ''' <remarks></remarks>
            [STRING] = 2
        End Enum




#Region "Headers"

        '   Header Order
        '   CancelTransmission|PayLoadDataType|TotalFragmentCount|FragmentIndex|PayLoadSize
        '   1                 | 1             |5                 | 5            | 5
        '   Total Header Bytes 17bytes

        ''' <summary>
        ''' The bytes Size of the header in each Layer1 Packet
        ''' </summary>
        ''' <remarks></remarks>
        Public Const HEADER_BYTE_SIZE As Byte = 17

        Public ReadOnly Property IsFirstPacket As Boolean
            Get
                Return Me.FragmentIndex = 0
            End Get
        End Property

        Public ReadOnly Property IsLastFragment As Boolean
            Get
                Return Me.FragmentIndex + 1 = Me.TotalFragmentCount
            End Get
        End Property

        Public ReadOnly Property IsFragmented As Boolean
            Get
                Return Me.TotalFragmentCount > 1
            End Get
        End Property


        Private __CancelTransmission As Boolean
        Public ReadOnly Property CancelTransmission As Boolean
            Get
                Return Me.__CancelTransmission
            End Get
        End Property


        '   All Headers uses ASCII Encoding
        '
        '

        '   Bool - One char 1 or 0 
        '

        '   0 - 65535
        '   5 charters lenght Max ==>5bytes

        '   Maximum Payload Size for now is < 16KB =16384bytes which is 5 chars = 5 bytes
        '

        Private __PayLoadDatype As PayLoadDataTypes
        Public ReadOnly Property PayLoadDatype As PayLoadDataTypes
            Get
                Return Me.__PayLoadDatype
            End Get
        End Property


        Private __TotalFragmentCount As UInt16
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property TotalFragmentCount As UInt16
            Get
                Return Me.__TotalFragmentCount
            End Get
        End Property


        Private __FragmentIndex As UInt16
        ''' <summary>
        ''' Zero based
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property FragmentIndex As UInt16
            Get
                Return Me.__FragmentIndex
            End Get
        End Property


        Private __PayLoadSize As UInt16
        Public ReadOnly Property PayLoadSize As UInt16
            Get
                Return Me.__PayLoadSize
            End Get
        End Property


        Private ReadOnly Property Headers As Byte()
            Get
                If Me.isValid() Then

                    '   Header Order
                    '   CancelTransmission|PayLoadDataType|TotalFragmentCount|FragmentIndex|PayLoadSize
                    '   1                 | 1             |5                 | 5            | 5
                    '   Total Header Bytes 17bytes

                    Dim lHeader As String = String.Empty

                    lHeader &= (Math.Abs(CInt(Me.CancelTransmission)).ToString())
                    lHeader &= (CInt(Me.PayLoadDatype).ToString())
                    lHeader &= (CInt(Me.TotalFragmentCount).ToString().PadLeft(5, "0".ToCharArray()(0)))
                    lHeader &= (CInt(Me.FragmentIndex).ToString().PadLeft(5, "0".ToCharArray()(0)))
                    lHeader &= (CInt(Me.PayLoadSize).ToString().PadLeft(5, "0".ToCharArray()(0)))

                    Return System.Text.ASCIIEncoding.ASCII.GetBytes(lHeader)

                End If

                Return New Byte() {}
            End Get
        End Property

#End Region



#Region "PayLoad"

        Private __Payload As Byte()
        Public ReadOnly Property Payload As Byte()
            Get
                Return Me.__Payload
            End Get
        End Property

        Public ReadOnly Property hasPayloadCompletelyArrived As Boolean
            Get
                Return Me.PayLoadSize = Me.AcquiredPayloadSize
            End Get
        End Property

        Private __AcquiredPayloadSize As UInt16
        Public ReadOnly Property AcquiredPayloadSize As UInt16
            Get
                Return __AcquiredPayloadSize
            End Get
        End Property


        ''' <summary>
        ''' Returns the difference in Size of rECEIVED and Expectation
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property AwaitingPayloadByteSizeRemaining As UInt16
            Get
                Return Me.PayLoadSize - Me.AcquiredPayloadSize
            End Get
        End Property

#End Region


        ''' <summary>
        ''' You must not pass in bytes greater than expected size. It adds to Payload
        ''' </summary>
        ''' <param name="__bytes"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function AccummulatePayload(ByVal __bytes As Byte()) As Boolean

            If __bytes Is Nothing OrElse __bytes.Length > Me.AwaitingPayloadByteSizeRemaining Then Return False

            Array.Copy(__bytes, 0, Me.Payload, Me.AcquiredPayloadSize, __bytes.Length)
            Me.__AcquiredPayloadSize = CUShort(Me.__AcquiredPayloadSize + __bytes.Length)

            Return True
        End Function


        Public Function getBytes() As Byte()

            If Me.isValid() Then


                Dim _____Load As List(Of Byte) = New List(Of Byte)()
                _____Load.AddRange(Me.Headers)
                _____Load.AddRange(Me.Payload)


                Return _____Load.ToArray()

            End If


            Return New Byte() {}
        End Function



        Private __isValid As Boolean = False

        ''' <summary>
        ''' Returns true if the Headers are well set
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function isValid() As Boolean
            Return Me.__isValid
        End Function



#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    Me.__Payload = Nothing

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

