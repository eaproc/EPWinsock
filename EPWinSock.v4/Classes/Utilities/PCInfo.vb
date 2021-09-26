Option Explicit On
Option Strict On

Imports EPWinSock.v4.DataXchange.StringPackages
Imports EPWinSock.v4.BASE

Imports EPRO.Library.v3._5.Network
Imports EPRO.Library.v4


Namespace Utilities



    ''' <summary>
    ''' Contains PC Information
    ''' </summary>
    ''' <remarks></remarks>
    Public Class PCInfo
        Inherits CommandPackage


#Region "Classes"


        Public Class InvalidPCInfoException
            Inherits Exception
            Sub New()
                MyBase.New("Invalid PC Information")
            End Sub

        End Class


#End Region


#Region "Constructors"

        Sub New(ByVal ID As Int32)

            ''MyBase.New(CMD_CLIENT_PC_INFO, SYNC_DELIMITER, 5,
            ''            ID.ToString(),
            ''            My.Computer.Name,
            ''            String.Empty,
            ''            Shell.OperatingSystem.getOSType().ToString().Replace("_", " "),
            ''            String.Empty
            ''            )

            MyBase.New(CMD_CLIENT_PC_INFO, SYNC_DELIMITER, 5,
                        ID.ToString(),
                        My.Computer.Name,
                        Network.getAllIpsOnMyComputer,
                        Shell.OperatingSystem.getOSType().ToString().Replace("_", " "),
                        Network.GetMacAddress()
                        )

            If Me.IsValid() Then
                Me._id = CInt(Me.Parameters(ParamsOrder.PC_ACQUIRED_UNIQUE_ID))
                Me._pcName = Me.Parameters(ParamsOrder.PC_NAME)
                Me._pcIPs = Me.Parameters(ParamsOrder.PC_IP)
                Me._pcOS = Me.Parameters(ParamsOrder.PC_OS)
                Me._pcMac = Me.Parameters(ParamsOrder.PC_MAC)

            End If

        End Sub


        Sub New(ByVal SynchronizedData As CommandPackage)

            MyBase.New(SynchronizedData.getBytes())

            ''Debug.Print("SynchronizedData : " & SynchronizedData.getContent())

            If Me.IsValid() Then
                Me._id = CInt(Me.Parameters(ParamsOrder.PC_ACQUIRED_UNIQUE_ID))
                Me._pcName = Me.Parameters(ParamsOrder.PC_NAME)
                Me._pcIPs = Me.Parameters(ParamsOrder.PC_IP)
                Me._pcOS = Me.Parameters(ParamsOrder.PC_OS)
                Me._pcMac = Me.Parameters(ParamsOrder.PC_MAC)

            End If
        End Sub




#End Region

#Region "Enums"

        Public Enum ParamsOrder
            PC_ACQUIRED_UNIQUE_ID = 0
            PC_NAME = 1
            PC_IP
            PC_OS
            PC_MAC
        End Enum


#End Region

#Region "Properties"


        Private _id As Int32 = 0
        Private _pcName As String = String.Empty
        Private _pcMac As String = String.Empty
        Private _pcIPs As String = String.Empty
        Private _pcOS As String = String.Empty

        Private Const SYNC_DELIMITER As String = "__=__"




        Public ReadOnly Property ID As Int32
            Get
                Return Me._id
            End Get
        End Property


        Public ReadOnly Property PCName As String
            Get
                Return Me._pcName
            End Get
        End Property


        Public ReadOnly Property IPAddresses As String()
            Get
                If Me._pcIPs IsNot Nothing And Me._pcIPs <> String.Empty Then Return Split(Me._pcIPs, ",")
                Return New String() {}
            End Get
        End Property


        Public ReadOnly Property AnyIPAddress As String
            Get
                If Me.IPAddresses.Count > 0 Then Return Me.IPAddresses(0)
                Return String.Empty
            End Get
        End Property


        Public ReadOnly Property AnyMacAddress As String
            Get

                Return Me._pcMac

            End Get
        End Property


        Public ReadOnly Property OperatingSystem As String
            Get
                Return Me._pcOS
            End Get
        End Property



#End Region

#Region "Method"



#End Region



    End Class

End Namespace