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
        PanelHeader = New Panel()
        LabelTitulo = New Label()
        PanelFooter = New Panel()
        PictureBox2 = New PictureBox()
        TextConsultando = New Label()
        Guardar = New Button()
        Label7 = New Label()
        TextIdModelo = New TextBox()
        Label1 = New Label()
        ComboEntorno = New ComboBox()
        Label2 = New Label()
        TextDescripModelo = New TextBox()
        Label3 = New Label()
        ComboTipoModelo = New ComboBox()
        Label4 = New Label()
        TextClassName = New TextBox()
        Label5 = New Label()
        TextRptFileName = New TextBox()
        Label6 = New Label()
        Button2 = New Button()
        LabelBinario = New Label()
        PanelHeader.SuspendLayout()
        PanelFooter.SuspendLayout()
        CType(PictureBox2, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' PanelHeader
        ' 
        PanelHeader.BackColor = Color.FromArgb(CByte(20), CByte(55), CByte(110))
        PanelHeader.Controls.Add(LabelTitulo)
        PanelHeader.Location = New Point(0, 0)
        PanelHeader.Name = "PanelHeader"
        PanelHeader.Size = New Size(450, 55)
        PanelHeader.TabIndex = 100
        ' 
        ' LabelTitulo
        ' 
        LabelTitulo.AutoSize = True
        LabelTitulo.Font = New Font("Segoe UI Semibold", 11F, FontStyle.Bold, GraphicsUnit.Point)
        LabelTitulo.ForeColor = Color.White
        LabelTitulo.Location = New Point(16, 15)
        LabelTitulo.Name = "LabelTitulo"
        LabelTitulo.TabIndex = 200
        LabelTitulo.Text = "Editar Modelo de Impresión"
        ' 
        ' Label7 (Id Modelo)
        ' 
        Label7.AutoSize = True
        Label7.Font = New Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point)
        Label7.ForeColor = Color.FromArgb(CByte(65), CByte(80), CByte(105))
        Label7.Location = New Point(20, 72)
        Label7.Name = "Label7"
        Label7.TabIndex = 6
        Label7.Text = "Id Modelo"
        ' 
        ' TextIdModelo
        ' 
        TextIdModelo.BackColor = Color.FromArgb(CByte(240), CByte(242), CByte(246))
        TextIdModelo.BorderStyle = BorderStyle.FixedSingle
        TextIdModelo.Enabled = False
        TextIdModelo.Font = New Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point)
        TextIdModelo.ForeColor = Color.FromArgb(CByte(110), CByte(115), CByte(130))
        TextIdModelo.Location = New Point(188, 68)
        TextIdModelo.Name = "TextIdModelo"
        TextIdModelo.Size = New Size(240, 23)
        TextIdModelo.TabIndex = 9
        ' 
        ' Label1 (Entorno)
        ' 
        Label1.AutoSize = True
        Label1.Font = New Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point)
        Label1.ForeColor = Color.FromArgb(CByte(65), CByte(80), CByte(105))
        Label1.Location = New Point(20, 110)
        Label1.Name = "Label1"
        Label1.TabIndex = 0
        Label1.Text = "Entorno"
        ' 
        ' ComboEntorno
        ' 
        ComboEntorno.DropDownStyle = ComboBoxStyle.DropDownList
        ComboEntorno.Font = New Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point)
        ComboEntorno.FormattingEnabled = True
        ComboEntorno.Location = New Point(188, 106)
        ComboEntorno.Name = "ComboEntorno"
        ComboEntorno.Size = New Size(240, 23)
        ComboEntorno.TabIndex = 10
        ' 
        ' Label2 (Descripcion)
        ' 
        Label2.AutoSize = True
        Label2.Font = New Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point)
        Label2.ForeColor = Color.FromArgb(CByte(65), CByte(80), CByte(105))
        Label2.Location = New Point(20, 148)
        Label2.Name = "Label2"
        Label2.TabIndex = 1
        Label2.Text = "Descripción Modelo"
        ' 
        ' TextDescripModelo
        ' 
        TextDescripModelo.BorderStyle = BorderStyle.FixedSingle
        TextDescripModelo.Font = New Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point)
        TextDescripModelo.Location = New Point(188, 144)
        TextDescripModelo.Name = "TextDescripModelo"
        TextDescripModelo.Size = New Size(240, 23)
        TextDescripModelo.TabIndex = 11
        ' 
        ' Label3 (Tipo Modelo)
        ' 
        Label3.AutoSize = True
        Label3.Font = New Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point)
        Label3.ForeColor = Color.FromArgb(CByte(65), CByte(80), CByte(105))
        Label3.Location = New Point(20, 186)
        Label3.Name = "Label3"
        Label3.TabIndex = 2
        Label3.Text = "Tipo Modelo"
        ' 
        ' ComboTipoModelo
        ' 
        ComboTipoModelo.Font = New Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point)
        ComboTipoModelo.FormattingEnabled = True
        ComboTipoModelo.Location = New Point(188, 182)
        ComboTipoModelo.Name = "ComboTipoModelo"
        ComboTipoModelo.Size = New Size(240, 23)
        ComboTipoModelo.TabIndex = 8
        ' 
        ' Label4 (ClassName)
        ' 
        Label4.AutoSize = True
        Label4.Font = New Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point)
        Label4.ForeColor = Color.FromArgb(CByte(65), CByte(80), CByte(105))
        Label4.Location = New Point(20, 224)
        Label4.Name = "Label4"
        Label4.TabIndex = 3
        Label4.Text = "ClassName"
        ' 
        ' TextClassName
        ' 
        TextClassName.BorderStyle = BorderStyle.FixedSingle
        TextClassName.Font = New Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point)
        TextClassName.Location = New Point(188, 220)
        TextClassName.Name = "TextClassName"
        TextClassName.Size = New Size(240, 23)
        TextClassName.TabIndex = 12
        ' 
        ' Label5 (Rpt File Name)
        ' 
        Label5.AutoSize = True
        Label5.Font = New Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point)
        Label5.ForeColor = Color.FromArgb(CByte(65), CByte(80), CByte(105))
        Label5.Location = New Point(20, 262)
        Label5.Name = "Label5"
        Label5.TabIndex = 4
        Label5.Text = "Rpt File Name"
        ' 
        ' TextRptFileName
        ' 
        TextRptFileName.BorderStyle = BorderStyle.FixedSingle
        TextRptFileName.Font = New Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point)
        TextRptFileName.Location = New Point(188, 258)
        TextRptFileName.Name = "TextRptFileName"
        TextRptFileName.Size = New Size(240, 23)
        TextRptFileName.TabIndex = 13
        ' 
        ' Label6 (Modelo)
        ' 
        Label6.AutoSize = True
        Label6.Font = New Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point)
        Label6.ForeColor = Color.FromArgb(CByte(65), CByte(80), CByte(105))
        Label6.Location = New Point(20, 300)
        Label6.Name = "Label6"
        Label6.TabIndex = 5
        Label6.Text = "Modelo"
        ' 
        ' Button2 (Cargar Modelo)
        ' 
        Button2.BackColor = Color.FromArgb(CByte(35), CByte(85), CByte(155))
        Button2.Cursor = Cursors.Hand
        Button2.FlatAppearance.BorderColor = Color.FromArgb(CByte(70), CByte(120), CByte(190))
        Button2.FlatAppearance.BorderSize = 1
        Button2.FlatStyle = FlatStyle.Flat
        Button2.Font = New Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point)
        Button2.ForeColor = Color.White
        Button2.Location = New Point(188, 295)
        Button2.Name = "Button2"
        Button2.Size = New Size(120, 26)
        Button2.TabIndex = 14
        Button2.Text = "Cargar Modelo"
        Button2.UseVisualStyleBackColor = False
        ' 
        ' LabelBinario
        ' 
        LabelBinario.AutoSize = True
        LabelBinario.Font = New Font("Segoe UI", 8.5F, FontStyle.Regular, GraphicsUnit.Point)
        LabelBinario.ForeColor = Color.FromArgb(CByte(80), CByte(120), CByte(170))
        LabelBinario.Location = New Point(315, 299)
        LabelBinario.Name = "LabelBinario"
        LabelBinario.TabIndex = 15
        ' 
        ' PanelFooter
        ' 
        PanelFooter.BackColor = Color.FromArgb(CByte(236), CByte(242), CByte(250))
        PanelFooter.Controls.Add(PictureBox2)
        PanelFooter.Controls.Add(TextConsultando)
        PanelFooter.Controls.Add(Guardar)
        PanelFooter.Location = New Point(0, 334)
        PanelFooter.Name = "PanelFooter"
        PanelFooter.Size = New Size(450, 46)
        PanelFooter.TabIndex = 101
        ' 
        ' PictureBox2
        ' 
        PictureBox2.Image = CType(resources.GetObject("PictureBox2.Image"), Image)
        PictureBox2.Location = New Point(10, 5)
        PictureBox2.Name = "PictureBox2"
        PictureBox2.Size = New Size(36, 36)
        PictureBox2.SizeMode = PictureBoxSizeMode.Zoom
        PictureBox2.TabIndex = 44
        PictureBox2.TabStop = False
        PictureBox2.Visible = False
        ' 
        ' TextConsultando
        ' 
        TextConsultando.Font = New Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point)
        TextConsultando.ForeColor = Color.FromArgb(CByte(50), CByte(100), CByte(170))
        TextConsultando.Location = New Point(54, 15)
        TextConsultando.MaximumSize = New Size(400, 0)
        TextConsultando.Name = "TextConsultando"
        TextConsultando.Size = New Size(97, 18)
        TextConsultando.TabIndex = 45
        ' 
        ' Guardar
        ' 
        Guardar.BackColor = Color.FromArgb(CByte(25), CByte(118), CByte(210))
        Guardar.Cursor = Cursors.Hand
        Guardar.FlatAppearance.BorderColor = Color.FromArgb(CByte(70), CByte(150), CByte(230))
        Guardar.FlatAppearance.BorderSize = 1
        Guardar.FlatStyle = FlatStyle.Flat
        Guardar.Font = New Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point)
        Guardar.ForeColor = Color.White
        Guardar.Location = New Point(258, 9)
        Guardar.Name = "Guardar"
        Guardar.Size = New Size(180, 28)
        Guardar.TabIndex = 7
        Guardar.Text = "Guardar"
        Guardar.UseVisualStyleBackColor = False
        ' 
        ' EditarModeloImpresionForm
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(248), CByte(250), CByte(254))
        ClientSize = New Size(450, 380)
        Controls.Add(Label7)
        Controls.Add(TextIdModelo)
        Controls.Add(Label1)
        Controls.Add(ComboEntorno)
        Controls.Add(Label2)
        Controls.Add(TextDescripModelo)
        Controls.Add(Label3)
        Controls.Add(ComboTipoModelo)
        Controls.Add(Label4)
        Controls.Add(TextClassName)
        Controls.Add(Label5)
        Controls.Add(TextRptFileName)
        Controls.Add(Label6)
        Controls.Add(Button2)
        Controls.Add(LabelBinario)
        Controls.Add(PanelHeader)
        Controls.Add(PanelFooter)
        FormBorderStyle = FormBorderStyle.FixedSingle
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        Name = "EditarModeloImpresionForm"
        Text = "Editar Modelo"
        PanelHeader.ResumeLayout(False)
        PanelHeader.PerformLayout()
        PanelFooter.ResumeLayout(False)
        PanelFooter.PerformLayout()
        CType(PictureBox2, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents PanelHeader As Panel
    Friend WithEvents LabelTitulo As Label
    Friend WithEvents PanelFooter As Panel
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents Guardar As Button
    Friend WithEvents ComboTipoModelo As ComboBox
    Friend WithEvents ComboEntorno As ComboBox
    Friend WithEvents TextIdModelo As TextBox
    Friend WithEvents TextDescripModelo As TextBox
    Friend WithEvents TextClassName As TextBox
    Friend WithEvents TextRptFileName As TextBox
    Friend WithEvents Button2 As Button
    Friend WithEvents LabelBinario As Label
    Friend WithEvents PictureBox2 As PictureBox
    Friend WithEvents TextConsultando As Label
End Class
