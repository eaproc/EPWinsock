Namespace DataXchange.StringPackages

    Public Class UserCommandPackage
        Inherits CommandPackage

#Region "Constructors"

        ''' <summary>
        ''' Extract Minimum Paramets and Paramseperator from Bytes
        ''' </summary>
        ''' <param name="__SocketBytes"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal __SocketBytes As Byte()
                     )
            MyBase.New(__SocketBytes)
        End Sub

        ''' <summary>
        ''' For sending
        ''' </summary>
        ''' <param name="____MinimumParameters">Minimum no of parameters apart from the command name. Default is 1</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal _____CommandName As String,
                       ByVal ___ParamSeperator As String,
                       ByVal ____MinimumParameters As Byte,
                       ParamArray ___Params() As String)

            MyBase.New(_____CommandName, ___ParamSeperator, ____MinimumParameters, ___Params)
            REM Contents are just parameters only

        End Sub

        ''' <summary>
        ''' For sending
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New(ByVal _____CommandName As String,
                       ByVal ___ParamSeperator As String,
                       ParamArray ___Params() As String)

            MyBase.New(_____CommandName, ___ParamSeperator, ___Params)
        End Sub

        ''' <summary>
        ''' For sending
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New(ByVal _____CommandName As String,
                       ParamArray ___Params() As CommandPackage)

            MyBase.New(_____CommandName, ___Params)

        End Sub



        Public Sub New(ByVal __SocketBytes As Byte(),
                        ByVal ___ParamSeperator As String,
                       ByVal ____MinimumParameters As Byte
                       )

            MyBase.New(__SocketBytes, ___ParamSeperator, ____MinimumParameters)

        End Sub



        Public Sub New(ByVal __SocketBytes As Byte(),
                        ByVal ___ParamSeperator As String
                       )
            MyBase.New(__SocketBytes, ___ParamSeperator)
        End Sub



#End Region


    End Class


End Namespace