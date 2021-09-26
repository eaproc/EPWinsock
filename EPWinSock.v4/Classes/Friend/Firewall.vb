Option Strict Off


''' <summary>
''' perform Firewall Operations
''' </summary>
''' <remarks></remarks>
Friend Class Firewall

    ''' <summary>
    ''' Only for 64 Bit Systems and Windows XP. Adds ICMPv4 to Firewall
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function AddICMPv4Exception() As Boolean

        If Utilities.OperatingSystem.getOSType = Utilities.OperatingSystem.MicrosoftOS.WINDOWS_XP Then

            Try
                'SYSTEM\CurrentControlSet\services\SharedAccess\Parameters\FirewallPolicy\StandardProfile\IcmpSettings",
                '"AllowInboundEchoRequest"

                If Registry.CreateASubKey(Registry.RegEditKeys.HKEY_LOCAL_MACHINE,
                                                                 "SYSTEM\CurrentControlSet\services\SharedAccess\Parameters\FirewallPolicy\StandardProfile",
                                                                 "IcmpSettings"
                                                                  ) IsNot Nothing Then

                    If Registry.CreateAKey(Registry.RegEditKeys.HKEY_LOCAL_MACHINE,
                                                               "SYSTEM\CurrentControlSet\services\SharedAccess\Parameters\FirewallPolicy\StandardProfile\IcmpSettings",
                                                                "AllowInboundEchoRequest", "1",
                                                                Microsoft.Win32.RegistryValueKind.DWord
                                                                 ) Then
                        Return True
                    End If

                End If

                Return False

            Catch ex As Exception
                Return False
            End Try
        Else

            Try

                Dim CurrentProfiles As Object

                Const NET_FW_IP_PROTOCOL_ICMPv4 = 1
                'Const NET_FW_IP_PROTOCOL_ICMPv6 = 58

                'Action
                Const NET_FW_ACTION_ALLOW = 1

                ' Create the FwPolicy2 object.
                Dim fwPolicy2 As Object
                fwPolicy2 = CreateObject("HNetCfg.FwPolicy2")

                ' Get the Rules object
                Dim RulesObject As Object
                RulesObject = fwPolicy2.Rules

                CurrentProfiles = fwPolicy2.CurrentProfileTypes

                'Create a Rule Object.
                Dim NewRule As Object
                NewRule = CreateObject("HNetCfg.FWRule")

                NewRule.Name = "ICMP_Rule"
                NewRule.Description = "Allow ICMP network traffic"
                NewRule.Protocol = NET_FW_IP_PROTOCOL_ICMPv4
                '            NewRule.IcmpTypesAndCodes = "*:*"
                NewRule.IcmpTypesAndCodes = "2:*,3:*,4:*,5:*,8:*,9:*,10:*,11:*,12:*,13:*,17:*"
                NewRule.Enabled = True
                NewRule.Grouping = "@firewallapi.dll,-23255"
                NewRule.Profiles = CurrentProfiles
                NewRule.Action = NET_FW_ACTION_ALLOW

                'Add a new rule
                RulesObject.Add(NewRule)

                Return True

            Catch ex As Exception

                Return False

            End Try



        End If


    End Function

    ''' <summary>
    ''' Checks if the ICMPv4 is Already Enabled
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function isICMPv4Allowed() As Boolean
        If Utilities.OperatingSystem.getOSType = Utilities.OperatingSystem.MicrosoftOS.WINDOWS_XP Then

            Return Registry.searchKeyLikeInSubFolder(
                                                        Registry.RegEditKeys.HKEY_LOCAL_MACHINE,
                                                       "SYSTEM\CurrentControlSet\services\SharedAccess\Parameters\FirewallPolicy\StandardProfile\IcmpSettings",
                                                       "AllowInboundEchoRequest"
                                                         )
        Else
            Return Registry.searchDataLikeInSubFolder(
                                                        Registry.RegEditKeys.HKEY_LOCAL_MACHINE,
                                                       "SYSTEM\CurrentControlSet\services\SharedAccess\Parameters\FirewallPolicy\FirewallRules",
                                                       "ICMP_Rule"
                                                         )
        End If


    End Function




End Class
