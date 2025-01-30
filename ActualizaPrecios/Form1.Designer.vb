Imports System.Runtime.InteropServices

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    ' Constantes para establecer el estilo del borde
    Private Const GWL_STYLE As Integer = -16
    Private Const WS_BORDER As Integer = &H800000
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

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Button1 = New Button()
        CheckBox1 = New CheckBox()
        CheckBox2 = New CheckBox()
        CheckBox3 = New CheckBox()
        TextBox1 = New TextBox()
        TextBox2 = New TextBox()
        Label1 = New Label()
        Label2 = New Label()
        Label3 = New Label()
        Button2 = New Button()
        Button3 = New Button()
        Button4 = New Button()
        Button5 = New Button()
        Button6 = New Button()
        Button7 = New Button()
        Label4 = New Label()
        Label5 = New Label()
        Button8 = New Button()
        CheckBox4 = New CheckBox()
        TextBox3 = New TextBox()
        Panel1 = New Panel()
        RadioButton2 = New RadioButton()
        RadioButton1 = New RadioButton()
        Panel2 = New Panel()
        Button18 = New Button()
        Button14 = New Button()
        Button13 = New Button()
        Button12 = New Button()
        Button11 = New Button()
        Button9 = New Button()
        PictureBox1 = New PictureBox()
        Button15 = New Button()
        Panel3 = New Panel()
        Label6 = New Label()
        DateTimePicker1 = New DateTimePicker()
        Panel4 = New Panel()
        ToolTop1 = New ToolTip(components)
        Timer1 = New Timer(components)
        Button10 = New Button()
        RadioButton3 = New RadioButton()
        Button17 = New Button()
        Button16 = New Button()
        Button19 = New Button()
        Panel1.SuspendLayout()
        Panel2.SuspendLayout()
        CType(PictureBox1, ComponentModel.ISupportInitialize).BeginInit()
        Panel3.SuspendLayout()
        Panel4.SuspendLayout()
        SuspendLayout()
        ' 
        ' Button1
        ' 
        Button1.Enabled = False
        Button1.Location = New Point(363, 53)
        Button1.Name = "Button1"
        Button1.Size = New Size(75, 23)
        Button1.TabIndex = 0
        Button1.Text = "Actualizar"
        Button1.UseVisualStyleBackColor = True
        ' 
        ' CheckBox1
        ' 
        CheckBox1.AutoSize = True
        CheckBox1.Location = New Point(137, 9)
        CheckBox1.Name = "CheckBox1"
        CheckBox1.Size = New Size(55, 19)
        CheckBox1.TabIndex = 1
        CheckBox1.Text = "CUPS"
        CheckBox1.UseVisualStyleBackColor = True
        ' 
        ' CheckBox2
        ' 
        CheckBox2.AutoSize = True
        CheckBox2.Location = New Point(237, 9)
        CheckBox2.Name = "CheckBox2"
        CheckBox2.Size = New Size(73, 19)
        CheckBox2.TabIndex = 2
        CheckBox2.Text = "Contrato"
        CheckBox2.UseVisualStyleBackColor = True
        ' 
        ' CheckBox3
        ' 
        CheckBox3.AutoSize = True
        CheckBox3.Location = New Point(328, 9)
        CheckBox3.Name = "CheckBox3"
        CheckBox3.Size = New Size(63, 19)
        CheckBox3.TabIndex = 3
        CheckBox3.Text = "Cliente"
        CheckBox3.UseVisualStyleBackColor = True
        ' 
        ' TextBox1
        ' 
        TextBox1.Enabled = False
        TextBox1.Location = New Point(7, 23)
        TextBox1.Name = "TextBox1"
        TextBox1.PlaceholderText = "Ingrese la nueva tarifa grupo"
        TextBox1.Size = New Size(343, 23)
        TextBox1.TabIndex = 4
        ' 
        ' TextBox2
        ' 
        TextBox2.Enabled = False
        TextBox2.HideSelection = False
        TextBox2.Location = New Point(8, 3)
        TextBox2.Multiline = True
        TextBox2.Name = "TextBox2"
        TextBox2.PlaceholderText = "Selecciona un filtro y escriba aqui para buscar: 216, 456789,231,24,...etc."
        TextBox2.ScrollBars = ScrollBars.Vertical
        TextBox2.Size = New Size(525, 43)
        TextBox2.TabIndex = 5
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(8, 9)
        Label1.Name = "Label1"
        Label1.Size = New Size(71, 15)
        Label1.TabIndex = 6
        Label1.Text = "Tarifa Grupo"
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(12, 9)
        Label2.Name = "Label2"
        Label2.Size = New Size(109, 15)
        Label2.TabIndex = 7
        Label2.Text = "Contratos a buscar:"
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Location = New Point(23, 9)
        Label3.Name = "Label3"
        Label3.Size = New Size(86, 15)
        Label3.TabIndex = 8
        Label3.Text = "Otras opciones"
        ' 
        ' Button2
        ' 
        Button2.Location = New Point(23, 27)
        Button2.Name = "Button2"
        Button2.Size = New Size(181, 23)
        Button2.TabIndex = 9
        Button2.Text = "Añadir Productos Contratos"
        Button2.UseVisualStyleBackColor = True
        ' 
        ' Button3
        ' 
        Button3.Location = New Point(23, 58)
        Button3.Name = "Button3"
        Button3.Size = New Size(180, 23)
        Button3.TabIndex = 10
        Button3.Text = "Volver a renovar"
        Button3.UseVisualStyleBackColor = True
        ' 
        ' Button4
        ' 
        Button4.Location = New Point(434, 145)
        Button4.Name = "Button4"
        Button4.Size = New Size(180, 23)
        Button4.TabIndex = 11
        Button4.Text = "Validaciones"
        Button4.UseVisualStyleBackColor = True
        ' 
        ' Button5
        ' 
        Button5.Location = New Point(23, 87)
        Button5.Name = "Button5"
        Button5.Size = New Size(180, 23)
        Button5.TabIndex = 12
        Button5.Text = "Añadir CodigosDIR"
        Button5.UseVisualStyleBackColor = True
        ' 
        ' Button6
        ' 
        Button6.Location = New Point(434, 27)
        Button6.Name = "Button6"
        Button6.Size = New Size(180, 23)
        Button6.TabIndex = 13
        Button6.Text = "Actualizar CNAE Excel"
        Button6.UseVisualStyleBackColor = True
        ' 
        ' Button7
        ' 
        Button7.Location = New Point(13, 96)
        Button7.Name = "Button7"
        Button7.Size = New Size(103, 23)
        Button7.TabIndex = 15
        Button7.Text = "Revisar"
        Button7.UseVisualStyleBackColor = True
        ' 
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.Location = New Point(7, 78)
        Label4.Name = "Label4"
        Label4.Size = New Size(236, 15)
        Label4.TabIndex = 16
        Label4.Text = "Revisar Tarifa Precio Contrato Personalizada"
        ' 
        ' Label5
        ' 
        Label5.AutoSize = True
        Label5.BorderStyle = BorderStyle.FixedSingle
        Label5.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point)
        Label5.ForeColor = SystemColors.HotTrack
        Label5.Location = New Point(6, 2)
        Label5.Name = "Label5"
        Label5.Size = New Size(23, 17)
        Label5.TabIndex = 17
        Label5.Text = "Ip:"
        ' 
        ' Button8
        ' 
        Button8.Location = New Point(434, 56)
        Button8.Name = "Button8"
        Button8.Size = New Size(180, 23)
        Button8.TabIndex = 18
        Button8.Text = "Actualizar Email Excel"
        Button8.UseVisualStyleBackColor = True
        ' 
        ' CheckBox4
        ' 
        CheckBox4.AutoSize = True
        CheckBox4.Checked = True
        CheckBox4.CheckState = CheckState.Checked
        CheckBox4.Location = New Point(444, 52)
        CheckBox4.Name = "CheckBox4"
        CheckBox4.Size = New Size(98, 19)
        CheckBox4.TabIndex = 19
        CheckBox4.Text = "Personalizada"
        CheckBox4.UseVisualStyleBackColor = True
        ' 
        ' TextBox3
        ' 
        TextBox3.Enabled = False
        TextBox3.Location = New Point(7, 52)
        TextBox3.Name = "TextBox3"
        TextBox3.PlaceholderText = "Tarifa grupo actual..."
        TextBox3.Size = New Size(343, 23)
        TextBox3.TabIndex = 20
        ' 
        ' Panel1
        ' 
        Panel1.Controls.Add(RadioButton2)
        Panel1.Controls.Add(RadioButton1)
        Panel1.Controls.Add(Label5)
        Panel1.Location = New Point(13, 416)
        Panel1.Name = "Panel1"
        Panel1.Size = New Size(561, 22)
        Panel1.TabIndex = 21
        ' 
        ' RadioButton2
        ' 
        RadioButton2.AutoSize = True
        RadioButton2.Location = New Point(497, 0)
        RadioButton2.Name = "RadioButton2"
        RadioButton2.Size = New Size(63, 19)
        RadioButton2.TabIndex = 27
        RadioButton2.TabStop = True
        RadioButton2.Text = "Replica"
        RadioButton2.UseVisualStyleBackColor = True
        ' 
        ' RadioButton1
        ' 
        RadioButton1.AutoSize = True
        RadioButton1.Checked = True
        RadioButton1.Location = New Point(405, 0)
        RadioButton1.Name = "RadioButton1"
        RadioButton1.Size = New Size(86, 19)
        RadioButton1.TabIndex = 26
        RadioButton1.TabStop = True
        RadioButton1.Text = "Producción"
        RadioButton1.UseVisualStyleBackColor = True
        ' 
        ' Panel2
        ' 
        Panel2.Controls.Add(Button19)
        Panel2.Controls.Add(Button18)
        Panel2.Controls.Add(Button14)
        Panel2.Controls.Add(Button13)
        Panel2.Controls.Add(Button12)
        Panel2.Controls.Add(Button11)
        Panel2.Controls.Add(Button9)
        Panel2.Controls.Add(PictureBox1)
        Panel2.Controls.Add(Label3)
        Panel2.Controls.Add(Button2)
        Panel2.Controls.Add(Button3)
        Panel2.Controls.Add(Button4)
        Panel2.Controls.Add(Button8)
        Panel2.Controls.Add(Button5)
        Panel2.Controls.Add(Button6)
        Panel2.Location = New Point(12, 211)
        Panel2.Name = "Panel2"
        Panel2.Size = New Size(617, 167)
        Panel2.TabIndex = 22
        ' 
        ' Button18
        ' 
        Button18.Location = New Point(229, 58)
        Button18.Name = "Button18"
        Button18.Size = New Size(180, 23)
        Button18.TabIndex = 27
        Button18.Text = "Consulta TOP"
        Button18.UseVisualStyleBackColor = True
        ' 
        ' Button14
        ' 
        Button14.Location = New Point(434, 116)
        Button14.Name = "Button14"
        Button14.Size = New Size(180, 23)
        Button14.TabIndex = 25
        Button14.Text = "Extraer CSV Varios"
        Button14.UseVisualStyleBackColor = True
        ' 
        ' Button13
        ' 
        Button13.Location = New Point(229, 27)
        Button13.Name = "Button13"
        Button13.Size = New Size(180, 23)
        Button13.TabIndex = 24
        Button13.Text = "OpenItems"
        Button13.UseVisualStyleBackColor = True
        ' 
        ' Button12
        ' 
        Button12.Location = New Point(434, 87)
        Button12.Name = "Button12"
        Button12.Size = New Size(180, 23)
        Button12.TabIndex = 23
        Button12.Text = "Extraer PDF Facs"
        Button12.UseVisualStyleBackColor = True
        ' 
        ' Button11
        ' 
        Button11.Location = New Point(23, 144)
        Button11.Name = "Button11"
        Button11.Size = New Size(180, 23)
        Button11.TabIndex = 22
        Button11.Text = "Cambiar Administrador Contrato"
        Button11.UseVisualStyleBackColor = True
        ' 
        ' Button9
        ' 
        Button9.Location = New Point(23, 116)
        Button9.Name = "Button9"
        Button9.Size = New Size(180, 23)
        Button9.TabIndex = 21
        Button9.Text = "Cambiar Agente Contrato"
        Button9.UseVisualStyleBackColor = True
        ' 
        ' PictureBox1
        ' 
        PictureBox1.Image = CType(resources.GetObject("PictureBox1.Image"), Image)
        PictureBox1.Location = New Point(539, 3)
        PictureBox1.Name = "PictureBox1"
        PictureBox1.Size = New Size(22, 21)
        PictureBox1.TabIndex = 20
        PictureBox1.TabStop = False
        ToolTop1.SetToolTip(PictureBox1, resources.GetString("PictureBox1.ToolTip"))
        ' 
        ' Button15
        ' 
        Button15.Location = New Point(442, 384)
        Button15.Name = "Button15"
        Button15.Size = New Size(180, 23)
        Button15.TabIndex = 26
        Button15.Text = "Desglosar click Luz"
        Button15.UseVisualStyleBackColor = True
        ' 
        ' Panel3
        ' 
        Panel3.Controls.Add(Label6)
        Panel3.Controls.Add(DateTimePicker1)
        Panel3.Controls.Add(Label1)
        Panel3.Controls.Add(Button1)
        Panel3.Controls.Add(TextBox1)
        Panel3.Controls.Add(TextBox3)
        Panel3.Controls.Add(Button7)
        Panel3.Controls.Add(CheckBox4)
        Panel3.Controls.Add(Label4)
        Panel3.Location = New Point(12, 77)
        Panel3.Name = "Panel3"
        Panel3.Size = New Size(617, 128)
        Panel3.TabIndex = 23
        ' 
        ' Label6
        ' 
        Label6.AutoSize = True
        Label6.Location = New Point(315, 79)
        Label6.Name = "Label6"
        Label6.Size = New Size(295, 15)
        Label6.TabIndex = 22
        Label6.Text = "Aplicación de Precios (según tarifa grupo configurada)"
        ' 
        ' DateTimePicker1
        ' 
        DateTimePicker1.Location = New Point(315, 96)
        DateTimePicker1.Name = "DateTimePicker1"
        DateTimePicker1.Size = New Size(200, 23)
        DateTimePicker1.TabIndex = 21
        ' 
        ' Panel4
        ' 
        Panel4.Controls.Add(TextBox2)
        Panel4.Location = New Point(12, 27)
        Panel4.Name = "Panel4"
        Panel4.Size = New Size(536, 49)
        Panel4.TabIndex = 24
        ' 
        ' ToolTop1
        ' 
        ToolTop1.ToolTipTitle = "Ayuda"
        ' 
        ' Button10
        ' 
        Button10.Location = New Point(551, 39)
        Button10.Name = "Button10"
        Button10.Size = New Size(57, 21)
        Button10.TabIndex = 25
        Button10.Text = "Limpiar"
        Button10.UseVisualStyleBackColor = True
        ' 
        ' RadioButton3
        ' 
        RadioButton3.AutoSize = True
        RadioButton3.Location = New Point(580, 416)
        RadioButton3.Name = "RadioButton3"
        RadioButton3.Size = New Size(46, 19)
        RadioButton3.TabIndex = 28
        RadioButton3.TabStop = True
        RadioButton3.Text = "UAT"
        RadioButton3.UseVisualStyleBackColor = True
        ' 
        ' Button17
        ' 
        Button17.Enabled = False
        Button17.Location = New Point(533, 173)
        Button17.Name = "Button17"
        Button17.Size = New Size(96, 23)
        Button17.TabIndex = 21
        Button17.Text = "Aplicar Precios"
        Button17.UseVisualStyleBackColor = True
        ' 
        ' Button16
        ' 
        Button16.Location = New Point(36, 387)
        Button16.Name = "Button16"
        Button16.Size = New Size(180, 23)
        Button16.TabIndex = 26
        Button16.Text = "Penalizaciones"
        Button16.UseVisualStyleBackColor = True
        ' 
        ' Button19
        ' 
        Button19.Location = New Point(229, 87)
        Button19.Name = "Button19"
        Button19.Size = New Size(180, 23)
        Button19.TabIndex = 29
        Button19.Text = "Verificar Licitacion"
        Button19.UseVisualStyleBackColor = True
        ' 
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.White
        ClientSize = New Size(641, 447)
        Controls.Add(Button16)
        Controls.Add(Button15)
        Controls.Add(Button17)
        Controls.Add(RadioButton3)
        Controls.Add(Button10)
        Controls.Add(Panel4)
        Controls.Add(Panel3)
        Controls.Add(Panel2)
        Controls.Add(Panel1)
        Controls.Add(Label2)
        Controls.Add(CheckBox3)
        Controls.Add(CheckBox2)
        Controls.Add(CheckBox1)
        DoubleBuffered = True
        FormBorderStyle = FormBorderStyle.FixedSingle
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Name = "Form1"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Gestor de Datos SIGE"
        TransparencyKey = Color.YellowGreen
        Panel1.ResumeLayout(False)
        Panel1.PerformLayout()
        Panel2.ResumeLayout(False)
        Panel2.PerformLayout()
        CType(PictureBox1, ComponentModel.ISupportInitialize).EndInit()
        Panel3.ResumeLayout(False)
        Panel3.PerformLayout()
        Panel4.ResumeLayout(False)
        Panel4.PerformLayout()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents Button1 As Button
    Friend WithEvents CheckBox1 As CheckBox
    Friend WithEvents CheckBox2 As CheckBox
    Friend WithEvents CheckBox3 As CheckBox
    Friend WithEvents TextBox1 As TextBox
    Friend WithEvents TextBox2 As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Button2 As Button
    Friend WithEvents Button3 As Button
    Friend WithEvents Button4 As Button
    Friend WithEvents Button5 As Button
    Friend WithEvents Button6 As Button
    Friend WithEvents Button7 As Button
    Friend WithEvents Label4 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents Button8 As Button
    Friend WithEvents CheckBox4 As CheckBox
    Friend WithEvents TextBox3 As TextBox
    Friend WithEvents Panel1 As Panel
    Friend WithEvents Panel2 As Panel
    Friend WithEvents Panel3 As Panel
    Friend WithEvents Panel4 As Panel

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)

        ' Dibujar el borde personalizado
        Dim borderColor As Color = Color.FromArgb(191, 24, 27)
        Dim borderThickness As Integer = 2
        Dim rect As New Rectangle(0, 0, Me.ClientSize.Width - 1, Me.ClientSize.Height - 1)
        Dim borderPen As New Pen(borderColor, borderThickness)
        e.Graphics.DrawRectangle(borderPen, rect)
    End Sub

    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents ToolTop1 As ToolTip
    Friend WithEvents Button9 As Button
    Friend WithEvents Timer1 As Timer
    Friend WithEvents Button10 As Button
    Friend WithEvents Button11 As Button
    Friend WithEvents Button12 As Button
    Friend WithEvents Button13 As Button
    Friend WithEvents Button14 As Button
    Friend WithEvents Button15 As Button
    Friend WithEvents RadioButton2 As RadioButton
    Friend WithEvents RadioButton1 As RadioButton
    Friend WithEvents RadioButton3 As RadioButton
    Friend WithEvents Button17 As Button
    Friend WithEvents Label6 As Label
    Friend WithEvents DateTimePicker1 As DateTimePicker
    Friend WithEvents Button16 As Button
    Friend WithEvents Button18 As Button
    Friend WithEvents Button19 As Button
End Class
