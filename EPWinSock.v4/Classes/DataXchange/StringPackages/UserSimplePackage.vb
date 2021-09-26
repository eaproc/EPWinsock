Namespace DataXchange.StringPackages

    REM For now this is not allowed
    Friend Class UserSimplePackage
        Inherits SimplePackage


#Region "Constructors"

        ''' <summary>
        ''' For sending
        ''' </summary>
        ''' <param name="_____Content"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal _____Content As String)

            MyBase.New(_____Content)

        End Sub


        Public Sub New(ByVal __SocketBytes As Byte())
            MyBase.New(__SocketBytes)


        End Sub






#End Region


    End Class


End Namespace