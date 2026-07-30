<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class PedirDatosForm
    Inherits System.Windows.Forms.Form

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

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        pnlCabecera = New Panel()
        lblTitulo = New Label()
        lblPeticion = New Label()
        txtDatos = New TextBox()
        pnlPie = New Panel()
        btnAceptar = New Button()
        btnOmitir = New Button()
        btnAbrirExcel = New Button()
        lblContador = New Label()
        pnlCabecera.SuspendLayout()
        pnlPie.SuspendLayout()
        SuspendLayout()
        '
        ' pnlCabecera
        '
        pnlCabecera.BackColor = Color.FromArgb(CByte(25), CByte(65), CByte(120))
        pnlCabecera.Controls.Add(lblTitulo)
        pnlCabecera.Dock = DockStyle.Top
        pnlCabecera.Location = New Point(0, 0)
        pnlCabecera.Name = "pnlCabecera"
        pnlCabecera.Size = New Size(504, 44)
        pnlCabecera.TabIndex = 0
        '
        ' lblTitulo
        '
        lblTitulo.AutoSize = True
        lblTitulo.Font = New Font("Segoe UI Semibold", 11F, FontStyle.Bold, GraphicsUnit.Point)
        lblTitulo.ForeColor = Color.White
        lblTitulo.Location = New Point(16, 11)
        lblTitulo.Name = "lblTitulo"
        lblTitulo.Size = New Size(120, 20)
        lblTitulo.TabIndex = 0
        lblTitulo.Text = "Segundo paso"
        '
        ' lblPeticion
        '
        lblPeticion.BackColor = Color.Transparent
        lblPeticion.Font = New Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point)
        lblPeticion.ForeColor = Color.FromArgb(CByte(55), CByte(65), CByte(81))
        lblPeticion.Location = New Point(16, 56)
        lblPeticion.Name = "lblPeticion"
        lblPeticion.Size = New Size(472, 56)
        lblPeticion.TabIndex = 1
        '
        ' txtDatos
        '
        txtDatos.Font = New Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point)
        txtDatos.Location = New Point(16, 116)
        txtDatos.Multiline = True
        txtDatos.Name = "txtDatos"
        txtDatos.ScrollBars = ScrollBars.Vertical
        txtDatos.Size = New Size(472, 150)
        txtDatos.TabIndex = 2
        '
        ' lblContador
        '
        lblContador.AutoSize = True
        lblContador.BackColor = Color.Transparent
        lblContador.Font = New Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point)
        lblContador.ForeColor = Color.FromArgb(CByte(110), CByte(135), CByte(175))
        lblContador.Location = New Point(16, 271)
        lblContador.Name = "lblContador"
        lblContador.Size = New Size(100, 13)
        lblContador.TabIndex = 3
        '
        ' pnlPie
        '
        pnlPie.BackColor = Color.FromArgb(CByte(238), CByte(243), CByte(250))
        pnlPie.Controls.Add(btnAbrirExcel)
        pnlPie.Controls.Add(btnOmitir)
        pnlPie.Controls.Add(btnAceptar)
        pnlPie.Dock = DockStyle.Bottom
        pnlPie.Location = New Point(0, 294)
        pnlPie.Name = "pnlPie"
        pnlPie.Size = New Size(504, 54)
        pnlPie.TabIndex = 4
        '
        ' btnAbrirExcel
        '
        btnAbrirExcel.Cursor = Cursors.Hand
        btnAbrirExcel.FlatStyle = FlatStyle.Flat
        btnAbrirExcel.Font = New Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point)
        btnAbrirExcel.Location = New Point(16, 12)
        btnAbrirExcel.Name = "btnAbrirExcel"
        btnAbrirExcel.Size = New Size(150, 30)
        btnAbrirExcel.TabIndex = 0
        btnAbrirExcel.Text = "Abrir Excel del paso 1"
        btnAbrirExcel.UseVisualStyleBackColor = False
        '
        ' btnOmitir
        '
        btnOmitir.Cursor = Cursors.Hand
        btnOmitir.FlatStyle = FlatStyle.Flat
        btnOmitir.Font = New Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point)
        btnOmitir.Location = New Point(276, 12)
        btnOmitir.Name = "btnOmitir"
        btnOmitir.Size = New Size(100, 30)
        btnOmitir.TabIndex = 1
        btnOmitir.Text = "Omitir paso 2"
        btnOmitir.UseVisualStyleBackColor = False
        '
        ' btnAceptar
        '
        btnAceptar.Cursor = Cursors.Hand
        btnAceptar.FlatStyle = FlatStyle.Flat
        btnAceptar.Font = New Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point)
        btnAceptar.Location = New Point(388, 12)
        btnAceptar.Name = "btnAceptar"
        btnAceptar.Size = New Size(100, 30)
        btnAceptar.TabIndex = 2
        btnAceptar.Text = "Continuar"
        btnAceptar.UseVisualStyleBackColor = False
        '
        ' PedirDatosForm
        '
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(248), CByte(251), CByte(255))
        ClientSize = New Size(504, 348)
        Controls.Add(lblContador)
        Controls.Add(txtDatos)
        Controls.Add(lblPeticion)
        Controls.Add(pnlPie)
        Controls.Add(pnlCabecera)
        DoubleBuffered = True
        FormBorderStyle = FormBorderStyle.FixedDialog
        MaximizeBox = False
        MinimizeBox = False
        Name = "PedirDatosForm"
        ShowInTaskbar = False
        StartPosition = FormStartPosition.CenterParent
        Text = "Segundo paso"
        pnlCabecera.ResumeLayout(False)
        pnlCabecera.PerformLayout()
        pnlPie.ResumeLayout(False)
        ResumeLayout(False)
    End Sub

    Friend WithEvents pnlCabecera As Panel
    Friend WithEvents lblTitulo As Label
    Friend WithEvents lblPeticion As Label
    Friend WithEvents txtDatos As TextBox
    Friend WithEvents lblContador As Label
    Friend WithEvents pnlPie As Panel
    Friend WithEvents btnAceptar As Button
    Friend WithEvents btnOmitir As Button
    Friend WithEvents btnAbrirExcel As Button
End Class
