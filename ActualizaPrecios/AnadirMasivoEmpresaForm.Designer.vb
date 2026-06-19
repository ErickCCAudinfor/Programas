<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AnadirMasivoEmpresaForm
    Inherits System.Windows.Forms.Form

    'Form reemplaza a Dispose para limpiar la lista de componentes.
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

    'Requerido por el Diseñador de Windows Forms
    Private components As System.ComponentModel.IContainer

    'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
    'Se puede modificar usando el Diseñador de Windows Forms.  
    'No lo modifique con el editor de código.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim DataGridViewCellStyle1 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As DataGridViewCellStyle = New DataGridViewCellStyle()
        CheckSeleccionarTodo = New CheckBox()
        GridEmpresas = New DataGridView()
        ColSeleccion = New DataGridViewCheckBoxColumn()
        ColNombre = New DataGridViewTextBoxColumn()
        ColVPN = New DataGridViewTextBoxColumn()
        ColCadena = New DataGridViewTextBoxColumn()
        LabelDescripcion = New Label()
        TextDescripcion = New TextBox()
        LabelEntorno = New Label()
        ComboEntorno = New ComboBox()
        LabelTipoModelo = New Label()
        ComboTipoModelo = New ComboBox()
        LabelClassName = New Label()
        TextClassName = New TextBox()
        BotonElegirReport = New Button()
        LabelReport = New Label()
        BotonSubirMasivo = New Button()
        BotonComprobarModelo = New Button()
        BotonActualizarMasivo = New Button()
        LabelEstado = New Label()
        CType(GridEmpresas, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' CheckSeleccionarTodo
        ' 
        CheckSeleccionarTodo.AutoSize = True
        CheckSeleccionarTodo.Location = New Point(16, 12)
        CheckSeleccionarTodo.Name = "CheckSeleccionarTodo"
        CheckSeleccionarTodo.Size = New Size(112, 19)
        CheckSeleccionarTodo.TabIndex = 0
        CheckSeleccionarTodo.Text = "Seleccionar todo"
        CheckSeleccionarTodo.UseVisualStyleBackColor = True
        ' 
        ' GridEmpresas
        ' 
        GridEmpresas.AllowUserToAddRows = False
        GridEmpresas.AllowUserToDeleteRows = False
        GridEmpresas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        GridEmpresas.BackgroundColor = Color.FromArgb(CByte(248), CByte(250), CByte(254))
        GridEmpresas.BorderStyle = BorderStyle.None
        GridEmpresas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        GridEmpresas.Columns.AddRange(New DataGridViewColumn() {ColSeleccion, ColNombre, ColVPN, ColCadena})
        GridEmpresas.EnableHeadersVisualStyles = False
        GridEmpresas.Location = New Point(16, 40)
        GridEmpresas.Name = "GridEmpresas"
        GridEmpresas.RowHeadersVisible = False
        GridEmpresas.RowTemplate.Height = 28
        GridEmpresas.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        GridEmpresas.Size = New Size(860, 280)
        GridEmpresas.TabIndex = 1
        DataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = Color.FromArgb(CByte(25), CByte(65), CByte(120))
        DataGridViewCellStyle1.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point)
        DataGridViewCellStyle1.ForeColor = Color.White
        DataGridViewCellStyle1.SelectionBackColor = Color.FromArgb(CByte(0), CByte(110), CByte(195))
        DataGridViewCellStyle1.SelectionForeColor = Color.White
        GridEmpresas.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        DataGridViewCellStyle2.BackColor = Color.White
        DataGridViewCellStyle2.Font = New Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point)
        DataGridViewCellStyle2.ForeColor = Color.FromArgb(CByte(45), CByte(52), CByte(68))
        DataGridViewCellStyle2.SelectionBackColor = Color.FromArgb(CByte(25), CByte(118), CByte(210))
        DataGridViewCellStyle2.SelectionForeColor = Color.White
        GridEmpresas.DefaultCellStyle = DataGridViewCellStyle2
        ' 
        ' ColSeleccion
        ' 
        ColSeleccion.FillWeight = 12F
        ColSeleccion.HeaderText = ""
        ColSeleccion.Name = "ColSeleccion"
        ' 
        ' ColNombre
        ' 
        ColNombre.FillWeight = 25F
        ColNombre.HeaderText = "Empresa"
        ColNombre.Name = "ColNombre"
        ColNombre.ReadOnly = True
        ' 
        ' ColVPN
        ' 
        ColVPN.FillWeight = 13F
        ColVPN.HeaderText = "VPN"
        ColVPN.Name = "ColVPN"
        ColVPN.ReadOnly = True
        ' 
        ' ColCadena
        ' 
        ColCadena.FillWeight = 50F
        ColCadena.HeaderText = "Cadena de conexión"
        ColCadena.Name = "ColCadena"
        ColCadena.ReadOnly = True
        ' 
        ' LabelDescripcion
        ' 
        LabelDescripcion.AutoSize = True
        LabelDescripcion.Location = New Point(16, 338)
        LabelDescripcion.Name = "LabelDescripcion"
        LabelDescripcion.Size = New Size(72, 15)
        LabelDescripcion.TabIndex = 2
        LabelDescripcion.Text = "Descripción"
        ' 
        ' TextDescripcion
        ' 
        TextDescripcion.Location = New Point(110, 335)
        TextDescripcion.Name = "TextDescripcion"
        TextDescripcion.Size = New Size(250, 23)
        TextDescripcion.TabIndex = 3
        ' 
        ' LabelEntorno
        ' 
        LabelEntorno.AutoSize = True
        LabelEntorno.Location = New Point(380, 338)
        LabelEntorno.Name = "LabelEntorno"
        LabelEntorno.Size = New Size(50, 15)
        LabelEntorno.TabIndex = 4
        LabelEntorno.Text = "Entorno"
        ' 
        ' ComboEntorno
        ' 
        ComboEntorno.DropDownStyle = ComboBoxStyle.DropDownList
        ComboEntorno.FormattingEnabled = True
        ComboEntorno.Location = New Point(440, 335)
        ComboEntorno.Name = "ComboEntorno"
        ComboEntorno.Size = New Size(150, 23)
        ComboEntorno.TabIndex = 5
        ' 
        ' LabelTipoModelo
        ' 
        LabelTipoModelo.AutoSize = True
        LabelTipoModelo.Location = New Point(610, 338)
        LabelTipoModelo.Name = "LabelTipoModelo"
        LabelTipoModelo.Size = New Size(31, 15)
        LabelTipoModelo.TabIndex = 6
        LabelTipoModelo.Text = "Tipo"
        ' 
        ' ComboTipoModelo
        ' 
        ComboTipoModelo.DropDownStyle = ComboBoxStyle.DropDownList
        ComboTipoModelo.FormattingEnabled = True
        ComboTipoModelo.Location = New Point(656, 335)
        ComboTipoModelo.Name = "ComboTipoModelo"
        ComboTipoModelo.Size = New Size(220, 23)
        ComboTipoModelo.TabIndex = 7
        ' 
        ' LabelClassName
        ' 
        LabelClassName.AutoSize = True
        LabelClassName.Location = New Point(16, 372)
        LabelClassName.Name = "LabelClassName"
        LabelClassName.Size = New Size(68, 15)
        LabelClassName.TabIndex = 8
        LabelClassName.Text = "ClassName"
        ' 
        ' TextClassName
        ' 
        TextClassName.Location = New Point(110, 369)
        TextClassName.Name = "TextClassName"
        TextClassName.Size = New Size(250, 23)
        TextClassName.TabIndex = 9
        ' 
        ' BotonElegirReport
        ' 
        BotonElegirReport.BackColor = Color.FromArgb(CByte(35), CByte(85), CByte(155))
        BotonElegirReport.Cursor = Cursors.Hand
        BotonElegirReport.Enabled = False
        BotonElegirReport.FlatAppearance.BorderColor = Color.FromArgb(CByte(70), CByte(120), CByte(190))
        BotonElegirReport.FlatAppearance.BorderSize = 1
        BotonElegirReport.FlatStyle = FlatStyle.Flat
        BotonElegirReport.Font = New Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point)
        BotonElegirReport.ForeColor = Color.White
        BotonElegirReport.Location = New Point(16, 410)
        BotonElegirReport.Name = "BotonElegirReport"
        BotonElegirReport.Size = New Size(140, 30)
        BotonElegirReport.TabIndex = 10
        BotonElegirReport.Text = "Elegir Report"
        BotonElegirReport.UseVisualStyleBackColor = False
        ' 
        ' LabelReport
        ' 
        LabelReport.AutoSize = True
        LabelReport.Location = New Point(170, 417)
        LabelReport.Name = "LabelReport"
        LabelReport.Size = New Size(0, 15)
        LabelReport.TabIndex = 11
        ' 
        ' BotonSubirMasivo
        ' 
        BotonSubirMasivo.BackColor = Color.FromArgb(CByte(25), CByte(118), CByte(210))
        BotonSubirMasivo.Cursor = Cursors.Hand
        BotonSubirMasivo.Enabled = False
        BotonSubirMasivo.FlatAppearance.BorderColor = Color.FromArgb(CByte(70), CByte(150), CByte(230))
        BotonSubirMasivo.FlatAppearance.BorderSize = 1
        BotonSubirMasivo.FlatStyle = FlatStyle.Flat
        BotonSubirMasivo.Font = New Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point)
        BotonSubirMasivo.ForeColor = Color.White
        BotonSubirMasivo.Location = New Point(16, 455)
        BotonSubirMasivo.Name = "BotonSubirMasivo"
        BotonSubirMasivo.Size = New Size(160, 30)
        BotonSubirMasivo.TabIndex = 12
        BotonSubirMasivo.Text = "Subir Report Masivo"
        BotonSubirMasivo.UseVisualStyleBackColor = False
        ' 
        ' BotonComprobarModelo
        ' 
        BotonComprobarModelo.BackColor = Color.FromArgb(CByte(35), CByte(85), CByte(155))
        BotonComprobarModelo.Cursor = Cursors.Hand
        BotonComprobarModelo.Enabled = False
        BotonComprobarModelo.FlatAppearance.BorderColor = Color.FromArgb(CByte(70), CByte(120), CByte(190))
        BotonComprobarModelo.FlatAppearance.BorderSize = 1
        BotonComprobarModelo.FlatStyle = FlatStyle.Flat
        BotonComprobarModelo.Font = New Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point)
        BotonComprobarModelo.ForeColor = Color.White
        BotonComprobarModelo.Location = New Point(192, 455)
        BotonComprobarModelo.Name = "BotonComprobarModelo"
        BotonComprobarModelo.Size = New Size(160, 30)
        BotonComprobarModelo.TabIndex = 14
        BotonComprobarModelo.Text = "Comprobar Modelo"
        BotonComprobarModelo.UseVisualStyleBackColor = False
        ' 
        ' BotonActualizarMasivo
        ' 
        BotonActualizarMasivo.BackColor = Color.FromArgb(CByte(21), CByte(128), CByte(61))
        BotonActualizarMasivo.Cursor = Cursors.Hand
        BotonActualizarMasivo.Enabled = False
        BotonActualizarMasivo.FlatAppearance.BorderColor = Color.FromArgb(CByte(60), CByte(170), CByte(100))
        BotonActualizarMasivo.FlatAppearance.BorderSize = 1
        BotonActualizarMasivo.FlatStyle = FlatStyle.Flat
        BotonActualizarMasivo.Font = New Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point)
        BotonActualizarMasivo.ForeColor = Color.White
        BotonActualizarMasivo.Location = New Point(368, 455)
        BotonActualizarMasivo.Name = "BotonActualizarMasivo"
        BotonActualizarMasivo.Size = New Size(175, 30)
        BotonActualizarMasivo.TabIndex = 15
        BotonActualizarMasivo.Text = "Actualizar Report Masivo"
        BotonActualizarMasivo.UseVisualStyleBackColor = False
        ' 
        ' LabelEstado
        ' 
        LabelEstado.AutoSize = True
        LabelEstado.ForeColor = Color.FromArgb(CByte(25), CByte(65), CByte(120))
        LabelEstado.Location = New Point(558, 462)
        LabelEstado.Name = "LabelEstado"
        LabelEstado.Size = New Size(0, 15)
        LabelEstado.TabIndex = 13
        ' 
        ' AnadirMasivoEmpresaForm
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(248), CByte(250), CByte(254))
        ClientSize = New Size(892, 500)
        Controls.Add(LabelEstado)
        Controls.Add(BotonActualizarMasivo)
        Controls.Add(BotonComprobarModelo)
        Controls.Add(BotonSubirMasivo)
        Controls.Add(LabelReport)
        Controls.Add(BotonElegirReport)
        Controls.Add(TextClassName)
        Controls.Add(LabelClassName)
        Controls.Add(ComboTipoModelo)
        Controls.Add(LabelTipoModelo)
        Controls.Add(ComboEntorno)
        Controls.Add(LabelEntorno)
        Controls.Add(TextDescripcion)
        Controls.Add(LabelDescripcion)
        Controls.Add(GridEmpresas)
        Controls.Add(CheckSeleccionarTodo)
        FormBorderStyle = FormBorderStyle.FixedSingle
        MaximizeBox = False
        Name = "AnadirMasivoEmpresaForm"
        StartPosition = FormStartPosition.CenterParent
        Text = "Añadir masivo Empresa"
        CType(GridEmpresas, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents CheckSeleccionarTodo As CheckBox
    Friend WithEvents GridEmpresas As DataGridView
    Friend WithEvents ColSeleccion As DataGridViewCheckBoxColumn
    Friend WithEvents ColNombre As DataGridViewTextBoxColumn
    Friend WithEvents ColVPN As DataGridViewTextBoxColumn
    Friend WithEvents ColCadena As DataGridViewTextBoxColumn
    Friend WithEvents LabelDescripcion As Label
    Friend WithEvents TextDescripcion As TextBox
    Friend WithEvents LabelEntorno As Label
    Friend WithEvents ComboEntorno As ComboBox
    Friend WithEvents LabelTipoModelo As Label
    Friend WithEvents ComboTipoModelo As ComboBox
    Friend WithEvents LabelClassName As Label
    Friend WithEvents TextClassName As TextBox
    Friend WithEvents BotonElegirReport As Button
    Friend WithEvents LabelReport As Label
    Friend WithEvents BotonSubirMasivo As Button
    Friend WithEvents BotonComprobarModelo As Button
    Friend WithEvents BotonActualizarMasivo As Button
    Friend WithEvents LabelEstado As Label
End Class
