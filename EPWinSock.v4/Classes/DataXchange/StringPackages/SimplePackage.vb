Imports System.Text
Imports CODERiT.Logger.v._3._5.Exceptions



Namespace DataXchange.StringPackages

    Public Class SimplePackage
        Implements ISimplePackage




#Region "Constructors"

        ''' <summary>
        ''' For sending
        ''' </summary>
        ''' <param name="_____Content"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal _____Content As String)

            Me.___Content = _____Content

        End Sub


        Public Sub New(ByVal __SocketBytes As Byte())

            Try


                If __SocketBytes IsNot Nothing AndAlso __SocketBytes.Length > 0 Then

                    Dim sSplitted As String() = UTF8Encoding.UTF8.GetString(__SocketBytes).Split(
                                                    New String() {PACKAGE__INDENTIFIER___DELIMITER}, StringSplitOptions.RemoveEmptyEntries
                                                    )
                    If sSplitted IsNot Nothing AndAlso sSplitted.Length = 2 AndAlso sSplitted(0) = Me.PackageIdentifierName Then

                        Me.___Content = sSplitted(1)

                    End If

                End If


            Catch ex As Exception

                BASE.LocalLogger.Write(New EException(ex))

            End Try

        End Sub






#End Region



#Region "Enum and Consts"

        ''' <summary>
        ''' It is used to Indentify what type of package is this
        ''' </summary>
        ''' <remarks></remarks>
        Protected Const PACKAGE__INDENTIFIER___DELIMITER As String = "(^_^)"


        Enum PackageTypes
            SIMPLE_PACKAGE
            COMMAND_PACKAGE
            SOCKET_COMMAND_PACKAGE
            SOCKET_SIMPLE_PACKAGE
            USER_COMMAND_PACKAGE
            USER_SIMPLE_PACKAGE
            UNKNOWN
        End Enum


#End Region


#Region "Properties"


        Protected ___Content As String

        ''' <summary>
        ''' Fetch the String Content only of this package without Indentifier
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property getContent() As String Implements ISimplePackage.getContent
            Get
                Return Me.___Content
            End Get
        End Property



        ''' <summary>
        ''' Return Whole package with indentifier and UTF8 encoded bytes
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable ReadOnly Property getBytes() As Byte() Implements ISimplePackage.getBytes
            Get
                Return UTF8Encoding.UTF8.GetBytes(
                    String.Format("{0}{1}{2}", Me.PackageIdentifierName, PACKAGE__INDENTIFIER___DELIMITER, Me.getContent())
                    )
            End Get
        End Property

        ''' <summary>
        ''' Returns this class data as string
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable ReadOnly Property getWholeClass() As String Implements ISimplePackage.getWholeClass
            Get
                Return String.Format("{0}{1}{2}", Me.PackageIdentifierName, PACKAGE__INDENTIFIER___DELIMITER, Me.getContent())

            End Get
        End Property


        ''' <summary>
        ''' Returns this Class Name
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>NOT Overridable</remarks>
        Protected Friend ReadOnly Property PackageIdentifierName As String Implements ISimplePackage.PackageIdentifierName
            Get
                REM Force the return of the ClassName allowed for identification
                Dim rst As String
                Dim r As Type = Me.GetType()
                Do
                    rst = r.Name
                    If Not RecognizedPackages.Contains(rst) Then r = r.BaseType
                Loop While Not RecognizedPackages.Contains(rst)

                Return rst
            End Get
        End Property



        Private RecognizedPackages() As String = New String() {
                                                GetType(SimplePackage).Name,
                                                GetType(CommandPackage).Name,
                                                GetType(SocketCommandPackage).Name,
                                                GetType(SocketSimplePackage).Name,
                                                GetType(UserSimplePackage).Name,
                                                GetType(UserCommandPackage).Name
                                                }

#End Region



#Region "Methods"

        Public Overridable Function IsValid() As Boolean
            Return Me.getContent() IsNot Nothing
        End Function



        Public Shared Function readStringPackageType(ByVal __bytes As Byte()) As PackageTypes

            If __bytes Is Nothing OrElse __bytes.Length = 0 Then Return PackageTypes.UNKNOWN

            Dim sSplitted As String() = UTF8Encoding.UTF8.GetString(__bytes).Split(
                                                   New String() {PACKAGE__INDENTIFIER___DELIMITER}, StringSplitOptions.RemoveEmptyEntries
                                                   )
            If sSplitted IsNot Nothing AndAlso sSplitted.Length > 0 Then

                Dim PackageName As String = sSplitted(0)

                Select Case PackageName
                    Case Is = GetType(SimplePackage).Name
                        Return PackageTypes.SIMPLE_PACKAGE
                    Case Is = GetType(CommandPackage).Name
                        Return PackageTypes.COMMAND_PACKAGE
                    Case Is = GetType(SocketCommandPackage).Name
                        Return PackageTypes.SOCKET_COMMAND_PACKAGE
                    Case Is = GetType(SocketSimplePackage).Name
                        Return PackageTypes.SOCKET_SIMPLE_PACKAGE
                    Case Is = GetType(UserSimplePackage).Name
                        Return PackageTypes.USER_SIMPLE_PACKAGE
                    Case Is = GetType(UserCommandPackage).Name
                        Return PackageTypes.USER_COMMAND_PACKAGE
                    Case Else
                        Return PackageTypes.UNKNOWN
                End Select

            Else
                Return PackageTypes.UNKNOWN
            End If

        End Function


        Public Shared Function parseStringPackage(ByVal __bytes As Byte()) As Object

            Select Case readStringPackageType(__bytes)

                Case PackageTypes.SIMPLE_PACKAGE
                    Return New SimplePackage(__bytes)

                Case PackageTypes.COMMAND_PACKAGE
                    Return New CommandPackage(__bytes)

                Case PackageTypes.SOCKET_COMMAND_PACKAGE
                    Return New SocketCommandPackage(__bytes)

                Case PackageTypes.SOCKET_SIMPLE_PACKAGE
                    Return New SocketSimplePackage(__bytes)

                Case PackageTypes.USER_COMMAND_PACKAGE
                    Return New UserCommandPackage(__bytes)

                Case PackageTypes.USER_SIMPLE_PACKAGE
                    Return New UserSimplePackage(__bytes)

                Case Else
                    Throw New Exception("This is weird. I can't understand this package")


            End Select


        End Function



        Public Overrides Function ToString() As String
            Return Me.getContent()
        End Function

#End Region




    End Class


End Namespace
