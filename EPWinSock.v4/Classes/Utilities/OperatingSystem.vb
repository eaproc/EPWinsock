Option Explicit On
Option Strict On


Namespace Utilities


    Public NotInheritable Class OperatingSystem




        Public Const WOW_64_FOLDER As String = "C:\Windows\SysWOW64"


        ''' <summary>
        ''' Microsoft Operating System Types.
        ''' Currently Supporting a few
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum MicrosoftOS
            WINDOWS_XP
            WINDOWS_VISTA
            WINDOWS_7
            WINDOWS_8
            WINDOWS_8_1
            UNKNOWN
        End Enum



        ''' <summary>
        ''' Get the major versions of Microsoft Operating Systems
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function getOSType() As MicrosoftOS

            Dim OSver As Version = Environment.OSVersion.Version
            If OSver.Major = 5 Then
                Return MicrosoftOS.WINDOWS_XP

            ElseIf OSver.Major = 6 And OSver.Minor = 0 Then
                Return MicrosoftOS.WINDOWS_VISTA


            ElseIf OSver.Major = 6 And OSver.Minor = 1 Then
                Return MicrosoftOS.WINDOWS_7


            ElseIf OSver.Major = 6 And OSver.Minor = 2 Then
                Return MicrosoftOS.WINDOWS_8


            ElseIf OSver.Major = 6 And OSver.Minor = 3 Then
                Return MicrosoftOS.WINDOWS_8_1


            Else
                Return MicrosoftOS.UNKNOWN

            End If

        End Function



        ''' <summary>
        ''' check if this folder exist [C:\Windows\SysWOW64] Which means is it 64bit
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function isThereWow64Folder() As Boolean
            REM The only problem with this method for now is 
            REM If user installed the OS in another drive not C:
            REM

            REM Get OS Installed Path

            Return FileIO.FileSystem.DirectoryExists(
                                           WOW_64_FOLDER
                                         )

        End Function




    End Class

End Namespace