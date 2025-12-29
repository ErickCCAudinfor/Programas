<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ProductosAsig
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ProductosAsig))
        ComboBox1 = New ComboBox()
        TextBox1 = New TextBox()
        Label1 = New Label()
        Label2 = New Label()
        FechaInicialPicker = New DateTimePicker()
        Label3 = New Label()
        CheckBox1 = New CheckBox()
        CheckBox4 = New CheckBox()
        CheckBox5 = New CheckBox()
        Button1 = New Button()
        Label4 = New Label()
        NumericUpDown1 = New NumericUpDown()
        ComboBox2 = New ComboBox()
        Label5 = New Label()
        CheckBox2 = New CheckBox()
        Label6 = New Label()
        CheckBox3 = New CheckBox()
        Label7 = New Label()
        FechaFinalPicker = New DateTimePicker()
        BotonImportarProducto = New Button()
        BotonPlantilla = New Button()
        CheckRedondear = New CheckBox()
        CType(NumericUpDown1, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' ComboBox1
        ' 
        ComboBox1.FormattingEnabled = True
        ComboBox1.Location = New Point(172, 36)
        ComboBox1.Name = "ComboBox1"
        ComboBox1.Size = New Size(313, 23)
        ComboBox1.TabIndex = 0
        ' 
        ' TextBox1
        ' 
        TextBox1.Enabled = False
        TextBox1.Location = New Point(172, 65)
        TextBox1.Name = "TextBox1"
        TextBox1.Size = New Size(313, 23)
        TextBox1.TabIndex = 1
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(12, 39)
        Label1.Name = "Label1"
        Label1.Size = New Size(56, 15)
        Label1.TabIndex = 2
        Label1.Text = "Producto"
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(12, 68)
        Label2.Name = "Label2"
        Label2.Size = New Size(92, 15)
        Label2.TabIndex = 3
        Label2.Text = "Producto Grupo"
        ' 
        ' FechaInicialPicker
        ' 
        FechaInicialPicker.CustomFormat = "dd/MM/yyyy"
        FechaInicialPicker.Format = DateTimePickerFormat.Custom
        FechaInicialPicker.Location = New Point(172, 95)
        FechaInicialPicker.Name = "FechaInicialPicker"
        FechaInicialPicker.Size = New Size(100, 23)
        FechaInicialPicker.TabIndex = 4
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Location = New Point(12, 125)
        Label3.Name = "Label3"
        Label3.Size = New Size(49, 15)
        Label3.TabIndex = 5
        Label3.Text = "Importe"
        ' 
        ' CheckBox1
        ' 
        CheckBox1.AutoSize = True
        CheckBox1.Location = New Point(172, 163)
        CheckBox1.Name = "CheckBox1"
        CheckBox1.Size = New Size(68, 19)
        CheckBox1.TabIndex = 7
        CheckBox1.Text = "Antes IE"
        CheckBox1.UseVisualStyleBackColor = True
        ' 
        ' CheckBox4
        ' 
        CheckBox4.AutoSize = True
        CheckBox4.Location = New Point(246, 163)
        CheckBox4.Name = "CheckBox4"
        CheckBox4.Size = New Size(82, 19)
        CheckBox4.TabIndex = 10
        CheckBox4.Text = "Aplicar s/c"
        CheckBox4.UseVisualStyleBackColor = True
        ' 
        ' CheckBox5
        ' 
        CheckBox5.AutoSize = True
        CheckBox5.Location = New Point(401, 163)
        CheckBox5.Name = "CheckBox5"
        CheckBox5.Size = New Size(84, 19)
        CheckBox5.TabIndex = 11
        CheckBox5.Text = "Aplicar p/c"
        CheckBox5.UseVisualStyleBackColor = True
        ' 
        ' Button1
        ' 
        Button1.BackColor = Color.FromArgb(CByte(25), CByte(118), CByte(210))
        Button1.FlatStyle = FlatStyle.Popup
        Button1.ForeColor = Color.White
        Button1.Location = New Point(382, 237)
        Button1.Name = "Button1"
        Button1.Size = New Size(103, 23)
        Button1.TabIndex = 13
        Button1.Text = "Insertar"
        Button1.UseVisualStyleBackColor = False
        ' 
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.Location = New Point(12, 101)
        Label4.Name = "Label4"
        Label4.Size = New Size(70, 15)
        Label4.TabIndex = 14
        Label4.Text = "Fecha Inicio"
        ' 
        ' NumericUpDown1
        ' 
        NumericUpDown1.DecimalPlaces = 6
        NumericUpDown1.Location = New Point(172, 125)
        NumericUpDown1.Name = "NumericUpDown1"
        NumericUpDown1.Size = New Size(120, 23)
        NumericUpDown1.TabIndex = 15
        ' 
        ' ComboBox2
        ' 
        ComboBox2.FormattingEnabled = True
        ComboBox2.Location = New Point(172, 194)
        ComboBox2.Name = "ComboBox2"
        ComboBox2.Size = New Size(191, 23)
        ComboBox2.TabIndex = 16
        ' 
        ' Label5
        ' 
        Label5.AutoSize = True
        Label5.Location = New Point(12, 202)
        Label5.Name = "Label5"
        Label5.Size = New Size(84, 15)
        Label5.TabIndex = 17
        Label5.Text = "Tipo Impuesto"
        ' 
        ' CheckBox2
        ' 
        CheckBox2.AutoSize = True
        CheckBox2.Checked = True
        CheckBox2.CheckState = CheckState.Checked
        CheckBox2.Enabled = False
        CheckBox2.Location = New Point(310, 242)
        CheckBox2.Name = "CheckBox2"
        CheckBox2.Size = New Size(15, 14)
        CheckBox2.TabIndex = 18
        CheckBox2.UseVisualStyleBackColor = True
        ' 
        ' Label6
        ' 
        Label6.AutoSize = True
        Label6.Location = New Point(167, 241)
        Label6.Name = "Label6"
        Label6.Size = New Size(137, 15)
        Label6.TabIndex = 19
        Label6.Text = "Desmarca para el update"
        ' 
        ' CheckBox3
        ' 
        CheckBox3.AutoSize = True
        CheckBox3.Location = New Point(334, 163)
        CheckBox3.Name = "CheckBox3"
        CheckBox3.Size = New Size(54, 19)
        CheckBox3.TabIndex = 20
        CheckBox3.Text = "p/día"
        CheckBox3.UseVisualStyleBackColor = True
        ' 
        ' Label7
        ' 
        Label7.AutoSize = True
        Label7.Location = New Point(310, 101)
        Label7.Name = "Label7"
        Label7.Size = New Size(66, 15)
        Label7.TabIndex = 22
        Label7.Text = "Fecha Final"
        ' 
        ' FechaFinalPicker
        ' 
        FechaFinalPicker.CustomFormat = " "
        FechaFinalPicker.Format = DateTimePickerFormat.Custom
        FechaFinalPicker.Location = New Point(382, 94)
        FechaFinalPicker.Name = "FechaFinalPicker"
        FechaFinalPicker.Size = New Size(103, 23)
        FechaFinalPicker.TabIndex = 21
        ' 
        ' BotonImportarProducto
        ' 
        BotonImportarProducto.BackColor = Color.FromArgb(CByte(25), CByte(118), CByte(210))
        BotonImportarProducto.FlatStyle = FlatStyle.Popup
        BotonImportarProducto.ForeColor = Color.White
        BotonImportarProducto.Location = New Point(226, 7)
        BotonImportarProducto.Name = "BotonImportarProducto"
        BotonImportarProducto.Size = New Size(133, 23)
        BotonImportarProducto.TabIndex = 23
        BotonImportarProducto.Text = "Importar desde excel"
        BotonImportarProducto.UseVisualStyleBackColor = False
        ' 
        ' BotonPlantilla
        ' 
        BotonPlantilla.BackColor = Color.FromArgb(CByte(25), CByte(118), CByte(210))
        BotonPlantilla.FlatStyle = FlatStyle.Popup
        BotonPlantilla.ForeColor = Color.White
        BotonPlantilla.Location = New Point(87, 7)
        BotonPlantilla.Name = "BotonPlantilla"
        BotonPlantilla.Size = New Size(133, 23)
        BotonPlantilla.TabIndex = 24
        BotonPlantilla.Text = "Generar Plantilla"
        BotonPlantilla.UseVisualStyleBackColor = False
        ' 
        ' CheckRedondear
        ' 
        CheckRedondear.AutoSize = True
        CheckRedondear.Location = New Point(365, 10)
        CheckRedondear.Name = "CheckRedondear"
        CheckRedondear.Size = New Size(121, 19)
        CheckRedondear.TabIndex = 25
        CheckRedondear.Text = "Redondear a 2Dec"
        CheckRedondear.UseVisualStyleBackColor = True
        ' 
        ' ProductosAsig
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(490, 265)
        Controls.Add(CheckRedondear)
        Controls.Add(BotonPlantilla)
        Controls.Add(BotonImportarProducto)
        Controls.Add(Label7)
        Controls.Add(FechaFinalPicker)
        Controls.Add(CheckBox3)
        Controls.Add(Label6)
        Controls.Add(CheckBox2)
        Controls.Add(Label5)
        Controls.Add(ComboBox2)
        Controls.Add(NumericUpDown1)
        Controls.Add(Label4)
        Controls.Add(Button1)
        Controls.Add(CheckBox5)
        Controls.Add(CheckBox4)
        Controls.Add(CheckBox1)
        Controls.Add(Label3)
        Controls.Add(FechaInicialPicker)
        Controls.Add(Label2)
        Controls.Add(Label1)
        Controls.Add(TextBox1)
        Controls.Add(ComboBox1)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Name = "ProductosAsig"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Productos"
        CType(NumericUpDown1, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents ComboBox1 As ComboBox
    Friend WithEvents TextBox1 As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents FechaInicialPicker As DateTimePicker
    Friend WithEvents Label3 As Label
    Friend WithEvents CheckBox1 As CheckBox
    Friend WithEvents CheckBox4 As CheckBox
    Friend WithEvents CheckBox5 As CheckBox
    Friend WithEvents Button1 As Button
    Friend WithEvents Label4 As Label
    Friend WithEvents NumericUpDown1 As NumericUpDown
    Friend WithEvents ComboBox2 As ComboBox
    Friend WithEvents Label5 As Label
    Friend WithEvents CheckBox2 As CheckBox
    Friend WithEvents Label6 As Label
    Friend WithEvents CheckBox3 As CheckBox
    Friend WithEvents Label7 As Label
    Friend WithEvents FechaFinalPicker As DateTimePicker
    Friend WithEvents BotonImportarProducto As Button
    Friend WithEvents BotonPlantilla As Button
    Friend WithEvents CheckRedondear As CheckBox
End Class
