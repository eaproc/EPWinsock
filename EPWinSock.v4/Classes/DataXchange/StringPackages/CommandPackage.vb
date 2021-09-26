Imports System.Text
Imports EPRO.Ciphers.v4.Substitutional
Imports CODERiT.Logger.v._3._5.Exceptions

Namespace DataXchange.StringPackages

    Public Class CommandPackage
        Inherits SimplePackage

        ' NOTE: This does not support no parameter. If you need no parameter. Just send message :)
        '



#Region "Constructors"

        ''' <summary>
        ''' For sending
        ''' </summary>
        ''' <param name="____MinimumParameters">Minimum no of parameters apart from the command name. Default is 1</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal _____CommandName As String,
                       ByVal ___ParamSeperator As String,
                       ByVal ____MinimumParameters As Byte,
                       ParamArray ___Params() As String)

            MyBase.New(compressCommandParameters(___ParamSeperator, ___Params))
            REM Contents are just parameters only

            Me.__ParameterSeparator = ___ParamSeperator
            Me.____CommandName = _____CommandName
            Me.MinimumParameters = ____MinimumParameters

        End Sub

        ''' <summary>
        ''' For sending
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New(ByVal _____CommandName As String,
                       ByVal ___ParamSeperator As String,
                       ParamArray ___Params() As String)

            Me.New(_____CommandName, ___ParamSeperator, 1, ___Params)
        End Sub


        ''' <summary>
        ''' For sending
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New(ByVal _____CommandName As String,
                       ParamArray ___Params() As CommandPackage)


            Me.New(_____CommandName, DEFAULT____PARAM___DELIMITER, CByte(___Params.Length),
                   ___Params.ToList.ConvertAll(Of String)(AddressOf MyClassConverter).ToArray()
                   )

        End Sub



        Public Sub New(ByVal __SocketBytes As Byte(),
                        ByVal ___ParamSeperator As String,
                       ByVal ____MinimumParameters As Byte,
                       Optional ByVal __Use_Byte_Params As Boolean = False
                       )

            MyBase.New(CType(Nothing, Byte()))
            Me.__ParameterSeparator = ___ParamSeperator
            Me.__MIN_PARAMS = ____MinimumParameters


            Try


                If __SocketBytes IsNot Nothing AndAlso __SocketBytes.Length > 0 Then

                    Dim sSplitted As String() = UTF8Encoding.UTF8.GetString(__SocketBytes).Split(
                                                    New String() {PACKAGE__INDENTIFIER___DELIMITER}, StringSplitOptions.RemoveEmptyEntries
                                                    )

                    REM If sSplitted IsNot Nothing AndAlso sSplitted.Length = 4 AndAlso sSplitted(0) = Me.Name Then
                    REM 2 is enough for validation because of inheritance
                    REM sSplitted(0) = Me.Name might not be consistent when extended

                    If BASE.IsDebugMode Then BASE.LocalLogger.Write("sSplitted.Length : " & sSplitted.Length)

                    If sSplitted IsNot Nothing AndAlso sSplitted.Length = 4 Then

                        If __Use_Byte_Params Then

                            Me.__ParameterSeparator = sSplitted(1)
                            Me.__MIN_PARAMS = CByte(sSplitted(2))

                        End If

                        Dim cSplitted As String() = sSplitted(3).Split(
                                                    New String() {COMMAND__NAME__SEPARATOR}, StringSplitOptions.RemoveEmptyEntries
                                                    )
                        If BASE.IsDebugMode Then BASE.LocalLogger.Write("cSplitted.Length : " & cSplitted.Length)
                        If cSplitted IsNot Nothing AndAlso cSplitted.Length = 2 Then

                            Me.____CommandName = cSplitted(0)


                            REM The remaining segment is param
                            Me.___Content = cSplitted(1)
                            If BASE.IsDebugMode Then BASE.LocalLogger.Write("Me.___Content : " & Me.___Content)

                        End If

                    End If

                End If


            Catch ex As Exception

                BASE.LocalLogger.Write(New EException(ex))

            End Try

        End Sub



        Public Sub New(ByVal __SocketBytes As Byte(),
                        ByVal ___ParamSeperator As String
                       )
            Me.New(__SocketBytes, ___ParamSeperator, 1)
        End Sub

        ''' <summary>
        ''' Extract Minimum Paramets and Paramseperator from Bytes
        ''' </summary>
        ''' <param name="__SocketBytes"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal __SocketBytes As Byte()
                     )
            Me.New(__SocketBytes, String.Empty, 0, True)
        End Sub




#End Region



#Region "Enum and Consts"

        ''' <summary>
        ''' It is used to separate the parameters
        ''' </summary>
        ''' <remarks></remarks>
        Public Const DEFAULT____PARAM___DELIMITER As String = "|-__-|"
        Private Const COMMAND__NAME__SEPARATOR As String = "(^.O)"



#End Region


#Region "Properties"

        Private __MIN_PARAMS As Byte = 1
        ''' <summary>
        ''' Minimum no of parameters apart from the command name. Default is 1
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Overridable Property MinimumParameters As Byte
            Get
                Return Me.__MIN_PARAMS
            End Get
            Set(value As Byte)
                Me.__MIN_PARAMS = value
            End Set
        End Property

        Private __ParameterSeparator As String
        Public ReadOnly Property ParameterSeparator As String
            Get
                Return Me.__ParameterSeparator
            End Get
        End Property



        ''' <summary>
        ''' Return Whole package with indentifier and UTF8 encoded bytes
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property getBytes() As Byte()
            Get
                If Me.IsValid() Then
                    Return UTF8Encoding.UTF8.GetBytes(
                            getWholeClass()
                        )
                End If

                Return New Byte() {}
            End Get
        End Property

        Public Overrides ReadOnly Property getWholeClass() As String
            Get
                Return String.Format("{0}{1}{2}{1}{3}{1}{4}{5}{6}", Me.PackageIdentifierName, PACKAGE__INDENTIFIER___DELIMITER,
                                                                    Me.ParameterSeparator, Me.MinimumParameters,
                                                        Me.CommandName, COMMAND__NAME__SEPARATOR,
                                                                             Me.getContent()
                                      )
            End Get
        End Property

        Public ReadOnly Property Parameters As String()
            Get

                Try
                    If Me.getContent() IsNot Nothing AndAlso Me.getContent <> String.Empty Then _
                    Return Me.getContent().Split(
                        New String() {Me.ParameterSeparator}, StringSplitOptions.RemoveEmptyEntries
                        )
                Catch ex As Exception
                    BASE.LocalLogger.Write(New EException(ex))
                End Try

                Return New String() {}

            End Get
        End Property

        Private ____CommandName As String
        Public ReadOnly Property CommandName As String
            Get
                Return Me.____CommandName
            End Get
        End Property


#End Region



#Region "Methods"

        ''' <summary>
        ''' Checks if it has any parameters at all
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function IsValid() As Boolean
            Return Me.Parameters.Length > 0
        End Function

        Public Function hasParameters() As Boolean
            Return Me.IsValid()
        End Function

        Public Function isParameterCountCorrect() As Boolean
            Return Me.Parameters.Length >= Me.MinimumParameters
        End Function


        Public Shared Function compressCommandParameters(ByVal delimiter As String,
                                                         ParamArray params As String()) As String

            If (params Is Nothing OrElse params.Length = 0) Then Return String.Empty

            Dim rst As String = String.Empty

            For Each p As String In params

                rst &= p
                If (params.ToList().IndexOf(p) < params.Count() - 1) Then rst &= delimiter

            Next

            Return rst
        End Function


        REM Cast to String
        Public Shared Narrowing Operator CType(ByVal ___v As CommandPackage) As String
            Try

                Using K As New CS.Keyless()
                    Return K.Encrypt(___v.getWholeClass())
                End Using

            Catch ex As Exception

            End Try

            Return String.Empty
        End Operator

        Public Shared Widening Operator CType(ByVal ___v As String) As CommandPackage

            Try
                Using K As New CS.Keyless()
                    If BASE.IsDebugMode Then BASE.LocalLogger.Write("K.Decrypt(___v): " & K.Decrypt(___v))
                    Return New CommandPackage(UTF8Encoding.UTF8.GetBytes(K.Decrypt(___v)))
                End Using

            Catch ex As Exception

            End Try


            Return New CommandPackage(New Byte() {})
        End Operator


        Public Shared Function MyClassConverter(Of T As CommandPackage)(ByVal obj As T) As String
            Return CType(obj, String)
        End Function

#End Region




    End Class


End Namespace
