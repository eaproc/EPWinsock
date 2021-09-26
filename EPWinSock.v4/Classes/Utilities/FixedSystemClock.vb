Imports System.Threading
Imports CODERiT.Logger.v._3._5.Exceptions

Namespace Utilities

    ''' <summary>
    ''' Create a Fixed System Clock That does  not get affected when user changes the system time
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FixedSystemClock
        Implements IDisposable

        Sub New()
            REM Me.___time = DateTime.Now
        End Sub


#Region "Properties"

        ''' <summary>
        ''' Interval in millisecs
        ''' </summary>
        ''' <remarks></remarks>
        Public Const INTERVAL As UInt16 = 1000

        Private thrTimer As Thread = Nothing

        Private ___time As DateTime

        Private ReadOnly Property IsRunning As Boolean
            Get
                Return Me.thrTimer IsNot Nothing AndAlso Me.thrTimer.IsAlive()
            End Get
        End Property


        Public ReadOnly Property [NOW] As DateTime
            Get
                If Me.IsRunning Then Return Me.___time
                Return System.DateTime.Now
            End Get
        End Property


#End Region



#Region "Methods"

        Private Sub Run()
            Me.___time = DateTime.Now

            Try
                While True
                    Thread.Sleep(INTERVAL) REM 1s

                    Me.___time = Me.___time.AddMilliseconds(INTERVAL)
                    REM Debug.Print("Inner: " & Me.___time.ToString())
                End While
            Catch ex As ThreadAbortException
                REM IGNORE
            Catch ex As Exception
                BASE.LocalLogger.Write(New EException(ex))
            End Try
        End Sub

        Public Sub [Stop]()
            If Me.IsRunning Then Me.thrTimer.Abort()
            Me.thrTimer = Nothing
        End Sub

        Public Function Start() As FixedSystemClock
            If Me.IsRunning Then Return Me


            Me.thrTimer = New Thread(AddressOf Me.Run)
            Me.thrTimer.IsBackground = True
            Me.thrTimer.Start()

            Me.___time = DateTime.Now REM For immediate usage

            Return Me

        End Function


#End Region




#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    Me.Stop()
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
