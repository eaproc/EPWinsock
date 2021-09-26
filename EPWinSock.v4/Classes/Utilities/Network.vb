Option Explicit On
Option Strict On

Imports System.Net.NetworkInformation
Imports System.Net


Namespace Utilities

    ''' <summary>
    ''' Exposes some functions to the whole project
    ''' </summary>
    ''' <remarks>This is a static Class ==Equivalent to Module</remarks>
    Public NotInheritable Class Network

        ''' <summary>
        ''' Check Whether the IP Entered is in a correct format [IP v4]. 
        ''' Min: 0.0.0.0
        ''' Max: 255.255.255.255
        ''' </summary>
        ''' <param name="IP">Ip Address to check</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function parseIP(ByVal IP As String) As Boolean
            REM 3 Locations of (.)
            REM [.] must not start and must not end
            REM Must not contain [..]
            REM Must Contain 4 Fragments
            REM Each Fragment Must not exceed a byte 255
            REM Run thru each fragment, Each must not exceed Max: 3chars and Min: 1
            REM All Fragment must be numeric


            If IP.Length = 0 Then Return False
            If IP.Substring(0, 1).Equals(".") Then Return False
            If IP.Substring(IP.Length - 1, 1).Equals(".") Then Return False
            If IP.IndexOf("..") > 0 Then Return False

            Dim IPSegs() As String = Split(IP, ".")
            If IPSegs.Length <> 4 Then Return False


            REM Let us check the content of each segment

            For Each Segment As String In IPSegs
                If Segment Is Nothing Then Return False
                REM Trim incase of space
                If Segment.Length > 3 Or Segment.Trim.Length < 1 Then Return False

                REM Assume no space and still meet the length
                If Not IsNumeric(Segment) Then Return False

                REM Since it is numeric, check if it is a byte value
                If Val(Segment) > 255 Then Return False

            Next




            REM If all segments passed the test then return true
            Return True
        End Function


        ''' <summary>
        ''' Check Whether the IP Entered is in a correct format [IP v4]. 
        ''' Min: 0.0.0.0
        ''' Max: 255.255.255.255
        ''' </summary>
        ''' <param name="IP">Ip Address to check</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function parseIP(ByVal IP As IPAddress) As Boolean
            REM 3 Locations of (.)
            REM [.] must not start and must not end
            REM Must Contain 4 Fragments
            REM Each Fragment Must not exceed a byte 255
            REM Run thru each fragment, Each must not exceed Max: 3chars and Min: 1
            REM All Fragment must be numeric


            Return parseIP(IP.ToString())

        End Function


        ''' <summary>
        ''' Returns any of this system IP Address
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared Function getAnyIpAddress() As String

            Try
                If My.Computer.Network.IsAvailable Then

                    For Each NIC As NetworkInterface In NetworkInterface.GetAllNetworkInterfaces


                        If NIC.NetworkInterfaceType = NetworkInterfaceType.Wireless80211 Or
                            NIC.NetworkInterfaceType = NetworkInterfaceType.Ethernet Then


                            For Each ip As UnicastIPAddressInformation In NIC.GetIPProperties().UnicastAddresses

                                If ip.Address.AddressFamily = System.Net.Sockets.AddressFamily.InterNetwork Then


                                    Return ip.Address.ToString()


                                End If

                            Next


                        End If




                    Next

                End If


                Return "0.0.0.0"

            Catch ex As Exception
                Return "0.0.0.0"
            End Try
        End Function


        ''' <summary>
        ''' Returns all valid Ip connection on this Computer. Separated with Comma(,)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function getAllIpsOnMyComputer(Optional ByVal Delimiter As String = ",") As String

            Try
                Dim Result As String = String.Empty

                If My.Computer.Network.IsAvailable Then

                    ' Dim Ips As System.Net.IPHostEntry = System.Net.Dns.GetHostByName(My.Computer.Name)

                    Dim Ips() As System.Net.IPAddress = System.Net.Dns.GetHostAddresses("")

                    'Return Ips.AddressList(0).ToString
                    For Each IP As IPAddress In Ips

                        If parseIP(IP) Then Result &= IP.ToString & Delimiter

                    Next

                    If Result <> vbNullString Then Result = Left(Result, Len(Result) - Len(Delimiter))


                End If


                Return Result

            Catch ex As Exception
                Return "0.0.0.0"
            End Try



        End Function


        ''' <summary>
        ''' Get Default NIC Mac Address .. because by default it returns it in order of activity
        ''' </summary>
        ''' <returns>The Mac Address without any delimiter [12 Chars]</returns>
        ''' <remarks></remarks>
        Public Shared Function GetAnyMacAddress() As String
            Dim nic As NetworkInterface = Nothing
            Dim macAddress As String = ""

            For Each nic In NetworkInterface.GetAllNetworkInterfaces
                macAddress = nic.GetPhysicalAddress().ToString.Trim()
                If macAddress <> "" Then
                    Return macAddress
                End If
            Next

            Return macAddress
        End Function


    End Class

End Namespace