<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class NovedadesForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        pnlCabecera = New Panel()
        lblTitulo = New Label()
        lblSubtitulo = New Label()
        pnlLista = New Panel()
        pnlPie = New Panel()
        btnCerrar = New Button()
        pnlCabecera.SuspendLayout()
        pnlPie.SuspendLayout()
        SuspendLayout()
        '
        ' pnlCabecera
        '
        pnlCabecera.BackColor = Color.FromArgb(CByte(25), CByte(65), CByte(120))
        pnlCabecera.Controls.Add(lblSubtitulo)
        pnlCabecera.Controls.Add(lblTitulo)
        pnlCabecera.Dock = DockStyle.Top
        pnlCabecera.Location = New Point(0, 0)
        pnlCabecera.Name = "pnlCabecera"
        pnlCabecera.Size = New Size(544, 64)
        pnlCabecera.TabIndex = 0
        '
        ' lblTitulo
        '
        lblTitulo.AutoSize = True
        lblTitulo.Font = New Font("Segoe UI Semibold", 13F, FontStyle.Bold, GraphicsUnit.Point)
        lblTitulo.ForeColor = Color.White
        lblTitulo.Location = New Point(18, 12)
        lblTitulo.Name = "lblTitulo"
        lblTitulo.Size = New Size(120, 25)
        lblTitulo.TabIndex = 0
        lblTitulo.Text = "Novedades"
        '
        ' lblSubtitulo
        '
        lblSubtitulo.AutoSize = True
        lblSubtitulo.Font = New Font("Segoe UI", 8.5F, FontStyle.Regular, GraphicsUnit.Point)
        lblSubtitulo.ForeColor = Color.FromArgb(CByte(170), CByte(200), CByte(240))
        lblSubtitulo.Location = New Point(20, 40)
        lblSubtitulo.Name = "lblSubtitulo"
        lblSubtitulo.Size = New Size(200, 15)
        lblSubtitulo.TabIndex = 1
        lblSubtitulo.Text = "Cambios y mejoras del Gestor de Datos SIGE"
        '
        ' pnlLista
        '
        pnlLista.AutoScroll = True
        pnlLista.BackColor = Color.FromArgb(CByte(248), CByte(251), CByte(255))
        pnlLista.Dock = DockStyle.Fill
        pnlLista.Location = New Point(0, 64)
        pnlLista.Name = "pnlLista"
        pnlLista.Padding = New Padding(16, 12, 16, 12)
        pnlLista.Size = New Size(544, 393)
        pnlLista.TabIndex = 1
        '
        ' pnlPie
        '
        pnlPie.BackColor = Color.FromArgb(CByte(238), CByte(243), CByte(250))
        pnlPie.Controls.Add(btnCerrar)
        pnlPie.Dock = DockStyle.Bottom
        pnlPie.Location = New Point(0, 457)
        pnlPie.Name = "pnlPie"
        pnlPie.Size = New Size(544, 54)
        pnlPie.TabIndex = 2
        '
        ' btnCerrar
        '
        btnCerrar.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnCerrar.Cursor = Cursors.Hand
        btnCerrar.FlatStyle = FlatStyle.Flat
        btnCerrar.Font = New Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point)
        btnCerrar.Location = New Point(420, 12)
        btnCerrar.Name = "btnCerrar"
        btnCerrar.Size = New Size(108, 30)
        btnCerrar.TabIndex = 0
        btnCerrar.Text = "Entendido"
        btnCerrar.UseVisualStyleBackColor = False
        '
        ' NovedadesForm
        '
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(248), CByte(251), CByte(255))
        ClientSize = New Size(544, 511)
        Controls.Add(pnlLista)
        Controls.Add(pnlPie)
        Controls.Add(pnlCabecera)
        DoubleBuffered = True
        FormBorderStyle = FormBorderStyle.FixedDialog
        MaximizeBox = False
        MinimizeBox = False
        Name = "NovedadesForm"
        ShowInTaskbar = False
        StartPosition = FormStartPosition.CenterParent
        Text = "Novedades"
        pnlCabecera.ResumeLayout(False)
        pnlCabecera.PerformLayout()
        pnlPie.ResumeLayout(False)
        ResumeLayout(False)
    End Sub

    Friend WithEvents pnlCabecera As Panel
    Friend WithEvents lblTitulo As Label
    Friend WithEvents lblSubtitulo As Label
    Friend WithEvents pnlLista As Panel
    Friend WithEvents pnlPie As Panel
    Friend WithEvents btnCerrar As Button
End Class
