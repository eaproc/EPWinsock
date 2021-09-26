Imports EPWinSock.v4.NET
Imports EPWinSock.v4


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
        Me.cmdDisconnect = New System.Windows.Forms.Button()
        Me.cmdConnect = New System.Windows.Forms.Button()
        Me.txtMessageDisplayFull = New System.Windows.Forms.TextBox()
        Me.pnlBackground = New System.Windows.Forms.Panel()
        Me.btnCancelSend = New System.Windows.Forms.Button()
        Me.prgSendingIndicator = New System.Windows.Forms.ProgressBar()
        Me.prgReceivingIndicator = New System.Windows.Forms.ProgressBar()
        Me.chkAutoScroll = New System.Windows.Forms.CheckBox()
        Me.lsvAvailableClients = New System.Windows.Forms.ListView()
        Me.colClientID = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colConnectedToClientID = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.lblReceivingIndicator = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.picMaximize = New System.Windows.Forms.PictureBox()
        Me.picMinimize = New System.Windows.Forms.PictureBox()
        Me.EpWinSock1 = New EPWinSock.v4.NET.ServerSocket(Me.components)
        Me.pnlBackground.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        CType(Me.picMaximize, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picMinimize, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'cmdDisconnect
        '
        Me.cmdDisconnect.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.cmdDisconnect.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdDisconnect.Font = New System.Drawing.Font("Trebuchet MS", 20.25!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Italic), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdDisconnect.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.cmdDisconnect.Location = New System.Drawing.Point(652, 312)
        Me.cmdDisconnect.Margin = New System.Windows.Forms.Padding(4, 6, 4, 6)
        Me.cmdDisconnect.Name = "cmdDisconnect"
        Me.cmdDisconnect.Size = New System.Drawing.Size(210, 53)
        Me.cmdDisconnect.TabIndex = 4
        Me.cmdDisconnect.Text = "Go Offline"
        Me.cmdDisconnect.UseVisualStyleBackColor = False
        '
        'cmdConnect
        '
        Me.cmdConnect.BackColor = System.Drawing.Color.Green
        Me.cmdConnect.Cursor = System.Windows.Forms.Cursors.Hand
        Me.cmdConnect.Font = New System.Drawing.Font("Trebuchet MS", 20.25!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Italic), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdConnect.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.cmdConnect.Location = New System.Drawing.Point(438, 312)
        Me.cmdConnect.Margin = New System.Windows.Forms.Padding(4, 6, 4, 6)
        Me.cmdConnect.Name = "cmdConnect"
        Me.cmdConnect.Size = New System.Drawing.Size(195, 53)
        Me.cmdConnect.TabIndex = 5
        Me.cmdConnect.Text = "Go Online"
        Me.cmdConnect.UseVisualStyleBackColor = False
        '
        'txtMessageDisplayFull
        '
        Me.txtMessageDisplayFull.BackColor = System.Drawing.Color.White
        Me.txtMessageDisplayFull.Font = New System.Drawing.Font("Trebuchet MS", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtMessageDisplayFull.Location = New System.Drawing.Point(12, 45)
        Me.txtMessageDisplayFull.Multiline = True
        Me.txtMessageDisplayFull.Name = "txtMessageDisplayFull"
        Me.txtMessageDisplayFull.ReadOnly = True
        Me.txtMessageDisplayFull.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtMessageDisplayFull.Size = New System.Drawing.Size(239, 299)
        Me.txtMessageDisplayFull.TabIndex = 10
        '
        'pnlBackground
        '
        Me.pnlBackground.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.pnlBackground.Controls.Add(Me.btnCancelSend)
        Me.pnlBackground.Controls.Add(Me.prgSendingIndicator)
        Me.pnlBackground.Controls.Add(Me.prgReceivingIndicator)
        Me.pnlBackground.Controls.Add(Me.chkAutoScroll)
        Me.pnlBackground.Controls.Add(Me.lsvAvailableClients)
        Me.pnlBackground.Controls.Add(Me.txtMessageDisplayFull)
        Me.pnlBackground.Controls.Add(Me.cmdConnect)
        Me.pnlBackground.Controls.Add(Me.Label1)
        Me.pnlBackground.Controls.Add(Me.Label3)
        Me.pnlBackground.Controls.Add(Me.lblReceivingIndicator)
        Me.pnlBackground.Controls.Add(Me.Label2)
        Me.pnlBackground.Controls.Add(Me.cmdDisconnect)
        Me.pnlBackground.Controls.Add(Me.Panel1)
        Me.pnlBackground.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlBackground.Location = New System.Drawing.Point(0, 0)
        Me.pnlBackground.Name = "pnlBackground"
        Me.pnlBackground.Size = New System.Drawing.Size(882, 371)
        Me.pnlBackground.TabIndex = 11
        '
        'btnCancelSend
        '
        Me.btnCancelSend.Location = New System.Drawing.Point(316, 312)
        Me.btnCancelSend.Name = "btnCancelSend"
        Me.btnCancelSend.Size = New System.Drawing.Size(115, 47)
        Me.btnCancelSend.TabIndex = 16
        Me.btnCancelSend.Text = "Cancel"
        Me.btnCancelSend.UseVisualStyleBackColor = True
        '
        'prgSendingIndicator
        '
        Me.prgSendingIndicator.Location = New System.Drawing.Point(571, 274)
        Me.prgSendingIndicator.Name = "prgSendingIndicator"
        Me.prgSendingIndicator.Size = New System.Drawing.Size(291, 23)
        Me.prgSendingIndicator.TabIndex = 15
        '
        'prgReceivingIndicator
        '
        Me.prgReceivingIndicator.Location = New System.Drawing.Point(571, 245)
        Me.prgReceivingIndicator.Name = "prgReceivingIndicator"
        Me.prgReceivingIndicator.Size = New System.Drawing.Size(291, 23)
        Me.prgReceivingIndicator.TabIndex = 15
        '
        'chkAutoScroll
        '
        Me.chkAutoScroll.AutoSize = True
        Me.chkAutoScroll.Checked = True
        Me.chkAutoScroll.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkAutoScroll.Location = New System.Drawing.Point(148, 16)
        Me.chkAutoScroll.Name = "chkAutoScroll"
        Me.chkAutoScroll.Size = New System.Drawing.Size(103, 26)
        Me.chkAutoScroll.TabIndex = 12
        Me.chkAutoScroll.Text = "Auto scroll"
        Me.chkAutoScroll.UseVisualStyleBackColor = True
        '
        'lsvAvailableClients
        '
        Me.lsvAvailableClients.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colClientID, Me.colConnectedToClientID})
        Me.lsvAvailableClients.FullRowSelect = True
        Me.lsvAvailableClients.Location = New System.Drawing.Point(328, 45)
        Me.lsvAvailableClients.Name = "lsvAvailableClients"
        Me.lsvAvailableClients.Size = New System.Drawing.Size(534, 194)
        Me.lsvAvailableClients.TabIndex = 11
        Me.lsvAvailableClients.UseCompatibleStateImageBehavior = False
        Me.lsvAvailableClients.View = System.Windows.Forms.View.Details
        '
        'colClientID
        '
        Me.colClientID.Text = "Client"
        Me.colClientID.Width = 292
        '
        'colConnectedToClientID
        '
        Me.colConnectedToClientID.Text = "Connected to:"
        Me.colConnectedToClientID.Width = 231
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 20)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(73, 22)
        Me.Label1.TabIndex = 9
        Me.Label1.Text = "Messages"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(426, 274)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(139, 22)
        Me.Label3.TabIndex = 9
        Me.Label3.Text = "Sending Indicator:"
        '
        'lblReceivingIndicator
        '
        Me.lblReceivingIndicator.AutoSize = True
        Me.lblReceivingIndicator.Location = New System.Drawing.Point(411, 246)
        Me.lblReceivingIndicator.Name = "lblReceivingIndicator"
        Me.lblReceivingIndicator.Size = New System.Drawing.Size(154, 22)
        Me.lblReceivingIndicator.TabIndex = 9
        Me.lblReceivingIndicator.Text = "Receiving Indicator:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(324, 16)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(128, 22)
        Me.Label2.TabIndex = 9
        Me.Label2.Text = "Available Clients"
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.Gainsboro
        Me.Panel1.Controls.Add(Me.Panel2)
        Me.Panel1.Location = New System.Drawing.Point(249, 141)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(65, 69)
        Me.Panel1.TabIndex = 14
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
        'EpWinSock1
        '
        Me.EpWinSock1.InActivityTimeout = CType(25000US, UShort)
        Me.EpWinSock1.ParentControl = Me
        Me.EpWinSock1.Port = CType(1302, Short)
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 22.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(882, 371)
        Me.Controls.Add(Me.pnlBackground)
        Me.Font = New System.Drawing.Font("Trebuchet MS", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(4, 6, 4, 6)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(898, 409)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(317, 399)
        Me.Name = "Form1"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Server App"
        Me.pnlBackground.ResumeLayout(False)
        Me.pnlBackground.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        CType(Me.picMaximize, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picMinimize, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents cmdDisconnect As System.Windows.Forms.Button
    Friend WithEvents cmdConnect As System.Windows.Forms.Button
    Friend WithEvents EpWinSock1 As ServerSocket
    Friend WithEvents txtMessageDisplayFull As System.Windows.Forms.TextBox
    Friend WithEvents pnlBackground As System.Windows.Forms.Panel
    Friend WithEvents lsvAvailableClients As System.Windows.Forms.ListView
    Friend WithEvents colClientID As System.Windows.Forms.ColumnHeader
    Friend WithEvents colConnectedToClientID As System.Windows.Forms.ColumnHeader
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents chkAutoScroll As System.Windows.Forms.CheckBox
    Friend WithEvents picMaximize As System.Windows.Forms.PictureBox
    Friend WithEvents picMinimize As System.Windows.Forms.PictureBox
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents prgSendingIndicator As System.Windows.Forms.ProgressBar
    Friend WithEvents prgReceivingIndicator As System.Windows.Forms.ProgressBar
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents lblReceivingIndicator As System.Windows.Forms.Label
    Friend WithEvents btnCancelSend As System.Windows.Forms.Button


End Class
