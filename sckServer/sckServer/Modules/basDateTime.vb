Module basDateTime
    ''' <summary>
    ''' Get the time difference in Milliseconds [1 sec = 1000 ms]
    ''' </summary>
    ''' <param name="LowerTime"></param>
    ''' <param name="HigherTime"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetTimeDifferenceInMilliseconds(ByVal LowerTime As Date, ByVal HigherTime As Date) As Long
        Dim ms As Long

        If HigherTime.Millisecond < LowerTime.Millisecond Then
            ms = (HigherTime.Millisecond + 1000) - LowerTime.Millisecond
        Else
            ms = HigherTime.Millisecond - LowerTime.Millisecond
        End If

        Return (DateDiff(DateInterval.Second, LowerTime, HigherTime) * 1000) + ms

    End Function
End Module
