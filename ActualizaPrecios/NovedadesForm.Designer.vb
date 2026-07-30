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
        tabNovedades = New TabControl()
        tabActual = New TabPage()
        pnlLista = New Panel()
        tabAnteriores = New TabPage()
        pnlHistorial = New Panel()
        pnlPie = New Panel()
        btnCerrar = New Button()
        pnlCabecera.SuspendLayout()
        tabNovedades.SuspendLayout()
        tabActual.SuspendLayout()
        tabAnteriores.SuspendLayout()
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
        ' tabNovedades
        '
        tabNovedades.Controls.Add(tabActual)
        tabNovedades.Controls.Add(tabAnteriores)
        tabNovedades.Dock = DockStyle.Fill
        tabNovedades.Font = New Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point)
        tabNovedades.Location = New Point(0, 64)
        tabNovedades.Name = "tabNovedades"
        tabNovedades.Padding = New Point(12, 4)
        tabNovedades.SelectedIndex = 0
        tabNovedades.Size = New Size(544, 393)
        tabNovedades.TabIndex = 1
        '
        ' tabActual
        '
        tabActual.BackColor = Color.FromArgb(CByte(248), CByte(251), CByte(255))
        tabActual.Controls.Add(pnlLista)
        tabActual.Location = New Point(4, 26)
        tabActual.Name = "tabActual"
        tabActual.Size = New Size(536, 363)
        tabActual.TabIndex = 0
        tabActual.Text = "Novedades"
        '
        ' pnlLista
        '
        pnlLista.AutoScroll = True
        pnlLista.BackColor = Color.FromArgb(CByte(248), CByte(251), CByte(255))
        pnlLista.Dock = DockStyle.Fill
        pnlLista.Location = New Point(0, 0)
        pnlLista.Name = "pnlLista"
        pnlLista.Padding = New Padding(16, 12, 16, 12)
        pnlLista.Size = New Size(536, 363)
        pnlLista.TabIndex = 0
        '
        ' tabAnteriores
        '
        tabAnteriores.BackColor = Color.FromArgb(CByte(248), CByte(251), CByte(255))
        tabAnteriores.Controls.Add(pnlHistorial)
        tabAnteriores.Location = New Point(4, 26)
        tabAnteriores.Name = "tabAnteriores"
        tabAnteriores.Size = New Size(536, 363)
        tabAnteriores.TabIndex = 1
        tabAnteriores.Text = "Versiones anteriores"
        '
        ' pnlHistorial
        '
        pnlHistorial.AutoScroll = True
        pnlHistorial.BackColor = Color.FromArgb(CByte(248), CByte(251), CByte(255))
        pnlHistorial.Dock = DockStyle.Fill
        pnlHistorial.Location = New Point(0, 0)
        pnlHistorial.Name = "pnlHistorial"
        pnlHistorial.Padding = New Padding(16, 12, 16, 12)
        pnlHistorial.Size = New Size(536, 363)
        pnlHistorial.TabIndex = 0
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
        Controls.Add(tabNovedades)
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
        tabNovedades.ResumeLayout(False)
        tabActual.ResumeLayout(False)
        tabAnteriores.ResumeLayout(False)
        pnlPie.ResumeLayout(False)
        ResumeLayout(False)
    End Sub

    Friend WithEvents pnlCabecera As Panel
    Friend WithEvents lblTitulo As Label
    Friend WithEvents lblSubtitulo As Label
    Friend WithEvents tabNovedades As TabControl
    Friend WithEvents tabActual As TabPage
    Friend WithEvents pnlLista As Panel
    Friend WithEvents tabAnteriores As TabPage
    Friend WithEvents pnlHistorial As Panel
    Friend WithEvents pnlPie As Panel
    Friend WithEvents btnCerrar As Button
End Class
