Imports EPWinSock.v4.NET


<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.cmdSend = New System.Windows.Forms.Button()
        Me.txtMessage = New System.Windows.Forms.TextBox()
        Me.chkAutoScroll = New System.Windows.Forms.CheckBox()
        Me.lsvAvailableClients = New System.Windows.Forms.ListView()
        Me.colClientID = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.txtMessageDisplayFull = New System.Windows.Forms.TextBox()
        Me.btnGoOnline = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.btnGoOffline = New System.Windows.Forms.Button()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.picMaximize = New System.Windows.Forms.PictureBox()
        Me.picMinimize = New System.Windows.Forms.PictureBox()
        Me.pnlBackground = New System.Windows.Forms.Panel()
        Me.prgSendingIndicator = New System.Windows.Forms.ProgressBar()
        Me.prgReceivingIndicator = New System.Windows.Forms.ProgressBar()
        Me.txtServerIP = New System.Windows.Forms.TextBox()
        Me.txtNickName = New System.Windows.Forms.TextBox()
        Me.lblClientID = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.EpWinSock1 = New EPWinSock.v4.NET.ClientSocket(Me.components)
        Me.Panel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        CType(Me.picMaximize, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picMinimize, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlBackground.SuspendLayout()
        Me.SuspendLayout()
        '
        'cmdSend
        '
        Me.cmdSend.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.cmdSend.Location = New System.Drawing.Point(86, 366)
        Me.cmdSend.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.cmdSend.Name = "cmdSend"
        Me.cmdSend.Size = New System.Drawing.Size(154, 28)
        Me.cmdSend.TabIndex = 1
        Me.cmdSend.Text = "&Send"
        Me.cmdSend.UseVisualStyleBackColor = True
        '
        'txtMessage
        '
        Me.txtMessage.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtMessage.Location = New System.Drawing.Point(1, 312)
        Me.txtMessage.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.txtMessage.Multiline = True
        Me.txtMessage.Name = "txtMessage"
        Me.txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtMessage.Size = New System.Drawing.Size(239, 47)
        Me.txtMessage.TabIndex = 1
        '
        'chkAutoScroll
        '
        Me.chkAutoScroll.AutoSize = True
        Me.chkAutoScroll.Checked = True
        Me.chkAutoScroll.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkAutoScroll.Location = New System.Drawing.Point(137, 81)
        Me.chkAutoScroll.Name = "chkAutoScroll"
        Me.chkAutoScroll.Size = New System.Drawing.Size(103, 26)
        Me.chkAutoScroll.TabIndex = 21
        Me.chkAutoScroll.Text = "Auto scroll"
        Me.chkAutoScroll.UseVisualStyleBackColor = True
        '
        'lsvAvailableClients
        '
        Me.lsvAvailableClients.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colClientID})
        Me.lsvAvailableClients.FullRowSelect = True
        Me.lsvAvailableClients.Location = New System.Drawing.Point(377, 100)
        Me.lsvAvailableClients.Name = "lsvAvailableClients"
        Me.lsvAvailableClients.Size = New System.Drawing.Size(376, 113)
        Me.lsvAvailableClients.TabIndex = 20
        Me.lsvAvailableClients.UseCompatibleStateImageBehavior = False
        Me.lsvAvailableClients.View = System.Windows.Forms.View.Details
        '
        'colClientID
        '
        Me.colClientID.Text = "Client"
        Me.colClientID.Width = 370
        '
        'txtMessageDisplayFull
        '
        Me.txtMessageDisplayFull.BackColor = System.Drawing.Color.White
        Me.txtMessageDisplayFull.Font = New System.Drawing.Font("Trebuchet MS", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtMessageDisplayFull.Location = New System.Drawing.Point(1, 110)
        Me.txtMessageDisplayFull.Multiline = True
        Me.txtMessageDisplayFull.Name = "txtMessageDisplayFull"
        Me.txtMessageDisplayFull.ReadOnly = True
        Me.txtMessageDisplayFull.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtMessageDisplayFull.Size = New System.Drawing.Size(239, 194)
        Me.txtMessageDisplayFull.TabIndex = 19
        '
        'btnGoOnline
        '
        Me.btnGoOnline.BackColor = System.Drawing.Color.Green
        Me.btnGoOnline.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnGoOnline.Font = New System.Drawing.Font("Trebuchet MS", 15.75!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Italic), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnGoOnline.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.btnGoOnline.Location = New System.Drawing.Point(408, 356)
        Me.btnGoOnline.Margin = New System.Windows.Forms.Padding(4, 6, 4, 6)
        Me.btnGoOnline.Name = "btnGoOnline"
        Me.btnGoOnline.Size = New System.Drawing.Size(160, 45)
        Me.btnGoOnline.TabIndex = 16
        Me.btnGoOnline.Text = "Go Online"
        Me.btnGoOnline.UseVisualStyleBackColor = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(1, 85)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(73, 22)
        Me.Label1.TabIndex = 17
        Me.Label1.Text = "Messages"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(373, 71)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(128, 22)
        Me.Label2.TabIndex = 18
        Me.Label2.Text = "Available Clients"
        '
        'btnGoOffline
        '
        Me.btnGoOffline.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.btnGoOffline.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnGoOffline.Font = New System.Drawing.Font("Trebuchet MS", 15.75!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Italic), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnGoOffline.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.btnGoOffline.Location = New System.Drawing.Point(593, 355)
        Me.btnGoOffline.Margin = New System.Windows.Forms.Padding(4, 6, 4, 6)
        Me.btnGoOffline.Name = "btnGoOffline"
        Me.btnGoOffline.Size = New System.Drawing.Size(160, 45)
        Me.btnGoOffline.TabIndex = 15
        Me.btnGoOffline.Text = "Go Offline"
        Me.btnGoOffline.UseVisualStyleBackColor = False
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.Gainsboro
        Me.Panel1.Controls.Add(Me.Panel2)
        Me.Panel1.Location = New System.Drawing.Point(238, 195)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(65, 69)
        Me.Panel1.TabIndex = 22
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.Color.White
        Me.Panel2.Controls.Add(Me.picMaximize)
        Me.Panel2.Controls.Add(Me.picMinimize)
        Me.Panel2.Location = New System.Drawing.Point(6, 8)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(65, 54)
        Me.Panel2.TabIndex = 14
        '
        'picMaximize
        '
        Me.picMaximize.BackColor = System.Drawing.Color.Transparent
        Me.picMaximize.Cursor = System.Windows.Forms.Cursors.Hand
        Me.picMaximize.Image = CType(resources.GetObject("picMaximize.Image"), System.Drawing.Image)
        Me.picMaximize.Location = New System.Drawing.Point(14, 11)
        Me.picMaximize.Name = "picMaximize"
        Me.picMaximize.Size = New System.Drawing.Size(38, 29)
        Me.picMaximize.TabIndex = 13
        Me.picMaximize.TabStop = False
        Me.picMaximize.Visible = False
        '
        'picMinimize
        '
        Me.picMinimize.BackColor = System.Drawing.Color.Transparent
        Me.picMinimize.Cursor = System.Windows.Forms.Cursors.Hand
        Me.picMinimize.Image = CType(resources.GetObject("picMinimize.Image"), System.Drawing.Image)
        Me.picMinimize.Location = New System.Drawing.Point(14, 11)
        Me.picMinimize.Name = "picMinimize"
        Me.picMinimize.Size = New System.Drawing.Size(38, 29)
        Me.picMinimize.TabIndex = 13
        Me.picMinimize.TabStop = False
        '
        'pnlBackground
        '
        Me.pnlBackground.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.pnlBackground.Controls.Add(Me.prgSendingIndicator)
        Me.pnlBackground.Controls.Add(Me.prgReceivingIndicator)
        Me.pnlBackground.Controls.Add(Me.txtServerIP)
        Me.pnlBackground.Controls.Add(Me.txtNickName)
        Me.pnlBackground.Controls.Add(Me.txtMessage)
        Me.pnlBackground.Controls.Add(Me.btnGoOffline)
        Me.pnlBackground.Controls.Add(Me.chkAutoScroll)
        Me.pnlBackground.Controls.Add(Me.cmdSend)
        Me.pnlBackground.Controls.Add(Me.lsvAvailableClients)
        Me.pnlBackground.Controls.Add(Me.lblClientID)
        Me.pnlBackground.Controls.Add(Me.Label1)
        Me.pnlBackground.Controls.Add(Me.txtMessageDisplayFull)
        Me.pnlBackground.Controls.Add(Me.Panel1)
        Me.pnlBackground.Controls.Add(Me.btnGoOnline)
        Me.pnlBackground.Controls.Add(Me.Label3)
        Me.pnlBackground.Controls.Add(Me.Label5)
        Me.pnlBackground.Controls.Add(Me.Label4)
        Me.pnlBackground.Controls.Add(Me.Label2)
        Me.pnlBackground.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlBackground.Location = New System.Drawing.Point(0, 0)
        Me.pnlBackground.Name = "pnlBackground"
        Me.pnlBackground.Size = New System.Drawing.Size(770, 410)
        Me.pnlBackground.TabIndex = 23
        '
        'prgSendingIndicator
        '
        Me.prgSendingIndicator.Location = New System.Drawing.Point(501, 322)
        Me.prgSendingIndicator.Name = "prgSendingIndicator"
        Me.prgSendingIndicator.Size = New System.Drawing.Size(252, 23)
        Me.prgSendingIndicator.TabIndex = 23
        '
        'prgReceivingIndicator
        '
        Me.prgReceivingIndicator.Location = New System.Drawing.Point(501, 293)
        Me.prgReceivingIndicator.Name = "prgReceivingIndicator"
        Me.prgReceivingIndicator.Size = New System.Drawing.Size(252, 23)
        Me.prgReceivingIndicator.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        Me.prgReceivingIndicator.TabIndex = 23
        Me.prgReceivingIndicator.Value = 50
        '
        'txtServerIP
        '
        Me.txtServerIP.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtServerIP.Location = New System.Drawing.Point(377, 238)
        Me.txtServerIP.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.txtServerIP.Name = "txtServerIP"
        Me.txtServerIP.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtServerIP.Size = New System.Drawing.Size(376, 29)
        Me.txtServerIP.TabIndex = 2
        Me.txtServerIP.Text = "192.168.0.1"
        '
        'txtNickName
        '
        Me.txtNickName.Font = New System.Drawing.Font("Segoe UI", 26.25!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Italic), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtNickName.ForeColor = System.Drawing.Color.DarkGray
        Me.txtNickName.Location = New System.Drawing.Point(1, 0)
        Me.txtNickName.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.txtNickName.Name = "txtNickName"
        Me.txtNickName.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtNickName.Size = New System.Drawing.Size(239, 54)
        Me.txtNickName.TabIndex = 2
        Me.txtNickName.Text = "Nick Name"
        '
        'lblClientID
        '
        Me.lblClientID.AutoSize = True
        Me.lblClientID.Font = New System.Drawing.Font("Segoe UI", 26.25!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Italic), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblClientID.Location = New System.Drawing.Point(369, 9)
        Me.lblClientID.Name = "lblClientID"
        Me.lblClientID.Size = New System.Drawing.Size(134, 47)
        Me.lblClientID.TabIndex = 17
        Me.lblClientID.Text = "- 1193 "
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Trebuchet MS", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
        Me.Label3.Location = New System.Drawing.Point(375, 221)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(66, 18)
        Me.Label3.TabIndex = 18
        Me.Label3.Text = "Server IP:"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(356, 323)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(139, 22)
        Me.Label5.TabIndex = 18
        Me.Label5.Text = "Sending Indicator:"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(341, 293)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(154, 22)
        Me.Label4.TabIndex = 18
        Me.Label4.Text = "Receiving Indicator:"
        '
        'EpWinSock1
        '
        Me.EpWinSock1.InActivityTimeout = CType(25000US, UShort)
        Me.EpWinSock1.PacketSize = EPWinSock.v4.BASE.SocketBufferSize.LARGE
        Me.EpWinSock1.ParentControl = Me
        Me.EpWinSock1.Port = CType(1302, Short)
        Me.EpWinSock1.ServerIP = "192.168.0.107"
        '
        'Form1
        '
        Me.AcceptButton = Me.cmdSend
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 22.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(770, 410)
        Me.Controls.Add(Me.pnlBackground)
        Me.Font = New System.Drawing.Font("Trebuchet MS", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(786, 448)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(321, 448)
        Me.Name = "Form1"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Client App"
        Me.Panel1.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        CType(Me.picMaximize, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picMinimize, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlBackground.ResumeLayout(False)
        Me.pnlBackground.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents cmdSend As System.Windows.Forms.Button
    Friend WithEvents txtMessage As System.Windows.Forms.TextBox
    Friend WithEvents EpWinSock1 As ClientSocket
    Friend WithEvents pnlBackground As System.Windows.Forms.Panel
    Friend WithEvents btnGoOffline As System.Windows.Forms.Button
    Friend WithEvents chkAutoScroll As System.Windows.Forms.CheckBox
    Friend WithEvents lsvAvailableClients As System.Windows.Forms.ListView
    Friend WithEvents colClientID As System.Windows.Forms.ColumnHeader
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents txtMessageDisplayFull As System.Windows.Forms.TextBox
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents picMaximize As System.Windows.Forms.PictureBox
    Friend WithEvents picMinimize As System.Windows.Forms.PictureBox
    Friend WithEvents btnGoOnline As System.Windows.Forms.Button
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents txtNickName As System.Windows.Forms.TextBox
    Friend WithEvents lblClientID As System.Windows.Forms.Label
    Friend WithEvents txtServerIP As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents prgSendingIndicator As System.Windows.Forms.ProgressBar
    Friend WithEvents prgReceivingIndicator As System.Windows.Forms.ProgressBar
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label

End Class
