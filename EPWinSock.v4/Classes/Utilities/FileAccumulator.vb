Option Explicit On
Option Strict On


Imports EPWinSock.v4.BASE
Imports EPRO.Library.v3._5

Imports System.IO


Namespace Utilities


    Public Class FileAccumulator
        Implements IDisposable


        Friend Sub New(ByVal FileOriginalName As String, ByVal OriginalFileSizeBytes As Int32,
                ByVal ErrNotifier As dlgSocketErrorMessage,
                ByVal ProgressNotifier As dlgFileTransferProgress)

            Me.___OriginalFileName = FileOriginalName
            Me.___OriginalFileSize = OriginalFileSizeBytes
            Me.ErrNotifier = ErrNotifier
            Me.ProgressNotifier = ProgressNotifier


            If Not Directory.Exists(Me.FilesTemporaryFolder) Then Directory.CreateDirectory(Me.FilesTemporaryFolder)

            Me.__TempFileName = Now.Ticks.ToString()

        End Sub


#Region "Properties"

        Friend ErrNotifier As dlgSocketErrorMessage


        Friend ProgressNotifier As dlgFileTransferProgress


        Private ___OriginalFileName As String
        Public ReadOnly Property OriginalFileName As String
            Get
                Return Me.___OriginalFileName
            End Get
        End Property

        Private ___OriginalFileSize As Int32
        ''' <summary>
        ''' In Bytes
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property OriginalFileSize As Int32
            Get
                Return Me.___OriginalFileSize
            End Get
        End Property

        ''' <summary>
        ''' In Bytes
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property TempFileFileSize As Int32
            Get
                If File.Exists(Me.TempFileFullPath) Then Return CInt(New FileInfo(Me.TempFileFullPath).Length)
                Return 0
            End Get
        End Property


        Private __TempFileName As String
        Public ReadOnly Property TempFileFullPath As String
            Get
                Return String.Format("{0}\{1}", Me.FilesTemporaryFolder, Me.__TempFileName)
            End Get
        End Property



        Private ReadOnly Property FilesTemporaryFolder As String
            Get
                Return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) & "\EpWinsock"
            End Get
        End Property

#End Region



#Region "Methods"

        Friend Function Accumulate(ByVal __Contents As Byte()) As Boolean

            Try


                If __Contents IsNot Nothing AndAlso __Contents.Length > 0 Then
                    Using fs As New FileStream(Me.TempFileFullPath, FileMode.Append)
                        fs.Write(__Contents, 0, __Contents.Length)
                        fs.Flush()
                        fs.Close()
                    End Using
                    If IsDebugMode Then LocalLogger.Print("Wrote File Content to Drive: " & Me.TempFileFullPath)
                    Return True
                End If
            Catch ex As Exception
                LocalLogger.Print("Error Writing File Content to Drive: " & Me.TempFileFullPath, ex)
            End Try

            Return False
        End Function


        ''' <summary>
        ''' Save Received File in your desired location
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub Save()
            Try

                Using sfd As New SaveFileDialog()
                    With sfd
                        .CheckPathExists = True
                        .FileName = Me.OriginalFileName
                        .InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                        If .ShowDialog() = DialogResult.OK Then Me.SaveAs(.FileName)

                    End With
                End Using
            Catch ex As Exception

            End Try
        End Sub


        Public Function SaveAs(ByVal FileFullPath As String) As Boolean
            Try


                FileIO.FileSystem.CopyFile(Me.TempFileFullPath, FileFullPath, True)
                Return True

            Catch ex As Exception
                Return False
            End Try


        End Function






#End Region



#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    Try

                    ' TODO: dispose managed state (managed objects).
                        If File.Exists(Me.TempFileFullPath) Then File.Delete(Me.TempFileFullPath)

                    Catch ex As Exception

                    End Try

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