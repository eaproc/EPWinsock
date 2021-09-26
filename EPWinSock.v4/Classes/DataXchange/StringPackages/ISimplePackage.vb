Namespace DataXchange.StringPackages


    Friend Interface ISimplePackage


#Region "Properties"

        ''' <summary>
        ''' Fetch the String Content only of this package without Indentifier
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property getContent() As String



        ''' <summary>
        ''' Return Whole package with indentifier and UTF8 encoded bytes
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property getBytes() As Byte()


        ''' <summary>
        ''' Returns this Class Name
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property PackageIdentifierName As String

        ''' <summary>
        ''' Returns whole package as string
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property getWholeClass() As String

#End Region



    End Interface

End Namespace
