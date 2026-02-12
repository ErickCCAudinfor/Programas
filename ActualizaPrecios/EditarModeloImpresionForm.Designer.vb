<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class EditarModeloImpresionForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(EditarModeloImpresionForm))
        Label1 = New Label()
        Label2 = New Label()
        Label3 = New Label()
        Label4 = New Label()
        Label5 = New Label()
        Label6 = New Label()
        Label7 = New Label()
        Guardar = New Button()
        ComboTipoModelo = New ComboBox()
        TextIdModelo = New TextBox()
        TextEntorno = New TextBox()
        TextDescripModelo = New TextBox()
        TextClassName = New TextBox()
        TextRptFileName = New TextBox()
        Button2 = New Button()
        LabelBinario = New Label()
        PictureBox2 = New PictureBox()
        TextConsultando = New Label()
        CType(PictureBox2, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(31, 33)
        Label1.Name = "Label1"
        Label1.Size = New Size(49, 15)
        Label1.TabIndex = 0
        Label1.Text = "Entorno"
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(31, 60)
        Label2.Name = "Label2"
        Label2.Size = New Size(117, 15)
        Label2.TabIndex = 1
        Label2.Text = "Drescripcion Modelo"
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Location = New Point(31, 88)
        Label3.Name = "Label3"
        Label3.Size = New Size(75, 15)
        Label3.TabIndex = 2
        Label3.Text = "Tipo Modelo"
        Label3.TextAlign = ContentAlignment.BottomRight
        ' 
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.Location = New Point(31, 112)
        Label4.Name = "Label4"
        Label4.Size = New Size(66, 15)
        Label4.TabIndex = 3
        Label4.Text = "ClassName"
        ' 
        ' Label5
        ' 
        Label5.AutoSize = True
        Label5.Location = New Point(31, 137)
        Label5.Name = "Label5"
        Label5.Size = New Size(81, 15)
        Label5.TabIndex = 4
        Label5.Text = "Rpt File Name"
        ' 
        ' Label6
        ' 
        Label6.AutoSize = True
        Label6.Location = New Point(31, 163)
        Label6.Name = "Label6"
        Label6.Size = New Size(48, 15)
        Label6.TabIndex = 5
        Label6.Text = "Modelo"
        ' 
        ' Label7
        ' 
        Label7.AutoSize = True
        Label7.Location = New Point(31, 9)
        Label7.Name = "Label7"
        Label7.Size = New Size(58, 15)
        Label7.TabIndex = 6
        Label7.Text = "IdModelo"
        ' 
        ' Guardar
        ' 
        Guardar.BackColor = Color.FromArgb(CByte(25), CByte(118), CByte(210))
        Guardar.FlatAppearance.BorderColor = Color.FromArgb(CByte(25), CByte(118), CByte(210))
        Guardar.FlatStyle = FlatStyle.Popup
        Guardar.ForeColor = SystemColors.Control
        Guardar.Location = New Point(194, 199)
        Guardar.Name = "Guardar"
        Guardar.Size = New Size(180, 23)
        Guardar.TabIndex = 7
        Guardar.Text = "Guardar"
        Guardar.UseVisualStyleBackColor = False
        ' 
        ' ComboTipoModelo
        ' 
        ComboTipoModelo.FormattingEnabled = True
        ComboTipoModelo.Location = New Point(194, 83)
        ComboTipoModelo.Name = "ComboTipoModelo"
        ComboTipoModelo.Size = New Size(180, 23)
        ComboTipoModelo.TabIndex = 8
        ' 
        ' TextIdModelo
        ' 
        TextIdModelo.Enabled = False
        TextIdModelo.Location = New Point(194, 6)
        TextIdModelo.Name = "TextIdModelo"
        TextIdModelo.Size = New Size(180, 23)
        TextIdModelo.TabIndex = 9
        ' 
        ' TextEntorno
        ' 
        TextEntorno.Location = New Point(194, 31)
        TextEntorno.Name = "TextEntorno"
        TextEntorno.Size = New Size(180, 23)
        TextEntorno.TabIndex = 10
        ' 
        ' TextDescripModelo
        ' 
        TextDescripModelo.Location = New Point(194, 58)
        TextDescripModelo.Name = "TextDescripModelo"
        TextDescripModelo.Size = New Size(180, 23)
        TextDescripModelo.TabIndex = 11
        ' 
        ' TextClassName
        ' 
        TextClassName.Location = New Point(194, 109)
        TextClassName.Name = "TextClassName"
        TextClassName.Size = New Size(180, 23)
        TextClassName.TabIndex = 12
        ' 
        ' TextRptFileName
        ' 
        TextRptFileName.Location = New Point(194, 134)
        TextRptFileName.Name = "TextRptFileName"
        TextRptFileName.Size = New Size(180, 23)
        TextRptFileName.TabIndex = 13
        ' 
        ' Button2
        ' 
        Button2.Location = New Point(194, 158)
        Button2.Name = "Button2"
        Button2.Size = New Size(94, 23)
        Button2.TabIndex = 14
        Button2.Text = "Cargar Modelo"
        Button2.UseVisualStyleBackColor = True
        ' 
        ' LabelBinario
        ' 
        LabelBinario.AutoSize = True
        LabelBinario.Location = New Point(294, 162)
        LabelBinario.Name = "LabelBinario"
        LabelBinario.Size = New Size(0, 15)
        LabelBinario.TabIndex = 15
        ' 
        ' PictureBox2
        ' 
        PictureBox2.Image = CType(resources.GetObject("PictureBox2.Image"), Image)
        PictureBox2.Location = New Point(31, 186)
        PictureBox2.Name = "PictureBox2"
        PictureBox2.Size = New Size(54, 36)
        PictureBox2.TabIndex = 44
        PictureBox2.TabStop = False
        PictureBox2.Visible = False
        ' 
        ' TextConsultando
        ' 
        TextConsultando.Location = New Point(91, 199)
        TextConsultando.MaximumSize = New Size(400, 0)
        TextConsultando.Name = "TextConsultando"
        TextConsultando.Size = New Size(97, 20)
        TextConsultando.TabIndex = 45
        ' 
        ' EditarModeloImpresionForm
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(213), CByte(220), CByte(227))
        ClientSize = New Size(405, 233)
        Controls.Add(TextConsultando)
        Controls.Add(PictureBox2)
        Controls.Add(LabelBinario)
        Controls.Add(Button2)
        Controls.Add(TextRptFileName)
        Controls.Add(TextClassName)
        Controls.Add(TextDescripModelo)
        Controls.Add(TextEntorno)
        Controls.Add(TextIdModelo)
        Controls.Add(ComboTipoModelo)
        Controls.Add(Guardar)
        Controls.Add(Label7)
        Controls.Add(Label6)
        Controls.Add(Label5)
        Controls.Add(Label4)
        Controls.Add(Label3)
        Controls.Add(Label2)
        Controls.Add(Label1)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Name = "EditarModeloImpresionForm"
        Text = "Editar Modelo"
        CType(PictureBox2, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents Guardar As Button
    Friend WithEvents ComboTipoModelo As ComboBox
    Friend WithEvents TextIdModelo As TextBox
    Friend WithEvents TextEntorno As TextBox
    Friend WithEvents TextDescripModelo As TextBox
    Friend WithEvents TextClassName As TextBox
    Friend WithEvents TextRptFileName As TextBox
    Friend WithEvents Button2 As Button
    Friend WithEvents LabelBinario As Label
    Friend WithEvents PictureBox2 As PictureBox
    Friend WithEvents TextConsultando As Label
End Class
