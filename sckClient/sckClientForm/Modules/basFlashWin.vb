Imports System.Runtime.InteropServices

Module basFlashWin

    ''Private Declare Function FlashWindowEx Lib "User32" (ByRef fwInfo As FLASHWINFO) As Boolean

    ' '' As defined by: http://msdn.microsoft.com/en-us/library/ms679347(v=vs.85).aspx
    ''Public Enum FlashWindowFlags As UInteger
    ''    ' Stop flashing. The system restores the window to its original state.
    ''    FLASHW_STOP = 0
    ''    ' Flash the window caption.
    ''    FLASHW_CAPTION = 1
    ''    ' Flash the taskbar button.
    ''    FLASHW_TRAY = 2
    ''    ' Flash both the window caption and taskbar button.
    ''    ' This is equivalent to setting the FLASHW_CAPTION | FLASHW_TRAY flags.
    ''    FLASHW_ALL = 3
    ''    ' Flash continuously, until the FLASHW_STOP flag is set.
    ''    FLASHW_TIMER = 4
    ''    ' Flash continuously until the window comes to the foreground.
    ''    FLASHW_TIMERNOFG = 12
    ''End Enum

    ''Public Structure FLASHWINFO
    ''    Public cbSize As Long
    ''    Public hwnd As Long
    ''    Public dwFlags As FlashWindowFlags
    ''    Public uCount As Long
    ''    Public dwTimeout As Long
    ''End Structure

    ''Public Function FlashWindow(ByRef frm As Windows.Forms.Form) As Boolean
    ''    Return FlashWindow(frm, True, True, 5)
    ''End Function
    ''Public Function FlashWindow(ByRef frm As Windows.Forms.Form, ByVal FlashTitleBar As Boolean, ByVal FlashTray As Boolean) As Boolean
    ''    Return FlashWindow(frm, FlashTitleBar, FlashTray, 5)
    ''End Function
    ''Public Function FlashWindow(ByRef frm As Windows.Forms.Form, ByVal FlashCount As Integer) As Boolean
    ''    Return FlashWindow(frm, True, True, FlashCount)
    ''End Function
    ''Public Function FlashWindow(ByRef frm As Windows.Forms.Form, ByVal FlashTitleBar As Boolean, ByVal FlashTray As Boolean, ByVal FlashCount As Integer) As Boolean
    ''    If frm Is Nothing Then Return False
    ''    If frm.IsDisposed Then Return False
    ''    If frm.Handle = 0 Then Return False

    ''    ''Try
    ''    ''    Dim fwi As New FLASHWINFO
    ''    ''    With fwi
    ''    ''        .hwnd = frm.Handle
    ''    ''        If FlashTitleBar Then .dwFlags = .dwFlags Or FlashWindowFlags.FLASHW_CAPTION
    ''    ''        If FlashTray Then .dwFlags = .dwFlags Or FlashWindowFlags.FLASHW_TRAY
    ''    ''        .uCount = FlashCount
    ''    ''        If FlashCount = 0 Then .dwFlags = .dwFlags Or FlashWindowFlags.FLASHW_TIMERNOFG
    ''    ''        .dwTimeout = 0 ' Use the default cursor blink rate.
    ''    ''        .cbSize = System.Runtime.InteropServices.Marshal.SizeOf(fwi)
    ''    ''    End With

    ''    ''    Return FlashWindowEx(fwi)
    ''    ''Catch
    ''    ''    Return False
    ''    ''End Try
    ''    Return FlashWindow(frm.Handle, FlashTitleBar, FlashTray, FlashCount)

    ''End Function

    ''Public Function FlashWindow(ByRef frmHwnd As IntPtr, ByVal FlashTitleBar As Boolean, ByVal FlashTray As Boolean, ByVal FlashCount As Integer) As Boolean

    ''    If frmHwnd = 0 Then Return False
    ''    Debug.Print(frmHwnd)

    ''    Try
    ''        Dim fwi As New FLASHWINFO
    ''        With fwi
    ''            .hwnd = frmHwnd
    ''            If FlashTitleBar Then .dwFlags = .dwFlags Or FlashWindowFlags.FLASHW_CAPTION
    ''            If FlashTray Then .dwFlags = .dwFlags Or FlashWindowFlags.FLASHW_TRAY
    ''            .uCount = FlashCount
    ''            If FlashCount = 0 Then .dwFlags = .dwFlags Or FlashWindowFlags.FLASHW_TIMERNOFG
    ''            .dwTimeout = 0 ' Use the default cursor blink rate.
    ''            .cbSize = System.Runtime.InteropServices.Marshal.SizeOf(fwi)
    ''        End With

    ''        Return FlashWindowEx(fwi)
    ''    Catch
    ''        Return False
    ''    End Try
    ''End Function


    <DllImport("user32")> _
    Public Function FlashWindow(ByVal hwnd As Integer, ByVal bInvert As Integer) As Integer

    End Function


    ''' <summary>
    ''' Keeps flashing until it is activated
    ''' </summary>
    ''' <param name="frm"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FlashWindow(ByVal frm As Form) As Boolean
        Try

            With New FlashWindowController(frm)

            End With

            Return True
        Catch ex As Exception

        End Try

        Return False
    End Function


    Private Class FlashWindowController

        Private frm As Form = Nothing
        REM Fetching Handle goes on cross thread
        REM To Avoid Invoke keep it
        Private frmHWnd As IntPtr = 0
        Private thatsOk As Boolean = False
        Private Const BLINK_SPEED As Int16 = 300


        Sub New(ByRef frm As Form)

            Me.frm = frm

            If Me.isFormStillOk Then
                Me.frmHWnd = Me.frm.Handle
                AddHandler frm.Activated, AddressOf Me.OurFormIsActivated
                Me.startFlashing()
            End If

        End Sub


        Private Sub OurFormIsActivated(ByVal sender As Object, ByVal e As EventArgs)
            Me.thatsOk = True
            Try

            
                If frm IsNot Nothing Then RemoveHandler frm.Activated, AddressOf Me.OurFormIsActivated

            Catch ex As Exception

            End Try
        End Sub

        Private Function isFormStillOk() As Boolean
            If frm Is Nothing Then Return False
            If frm.IsDisposed Then Return False
            If Not frm.IsHandleCreated Then Return False

            Return True
        End Function


        Private Sub startFlashing()
            With New Threading.Thread(
                            New Threading.ThreadStart(AddressOf Me.FlashWindow____Threaded)
                            )
                .SetApartmentState(Threading.ApartmentState.MTA)
                .Start()
            End With
        End Sub

        Private Sub FlashWindow____Threaded()

            While (Me.isFormStillOk And (Not Me.thatsOk))
                Try


                    basFlashWin.FlashWindow(Me.frmHWnd, 1)

                    Threading.Thread.Sleep(BLINK_SPEED)



                Catch ex As Exception
                    Exit While
                End Try

            End While


        End Sub



    End Class

    




End Module
