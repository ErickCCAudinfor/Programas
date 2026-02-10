<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TrocearXMLForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(TrocearXMLForm))
        LabelEntrada = New Label()
        LabelSalida = New Label()
        Label3 = New Label()
        Label4 = New Label()
        TextRuta = New TextBox()
        TextSalida = New TextBox()
        LabelTipoNodo = New Label()
        ComboTipoXml = New ComboBox()
        BotonTrocear = New Button()
        TextoTamaño = New TextBox()
        LabelTamaño = New Label()
        CargarImagen = New PictureBox()
        TextConsultando = New Label()
        CType(CargarImagen, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' LabelEntrada
        ' 
        LabelEntrada.AutoSize = True
        LabelEntrada.Location = New Point(33, 19)
        LabelEntrada.Name = "LabelEntrada"
        LabelEntrada.Size = New Size(31, 15)
        LabelEntrada.TabIndex = 0
        LabelEntrada.Text = "Ruta"
        ' 
        ' LabelSalida
        ' 
        LabelSalida.AutoSize = True
        LabelSalida.Location = New Point(33, 46)
        LabelSalida.Name = "LabelSalida"
        LabelSalida.Size = New Size(38, 15)
        LabelSalida.TabIndex = 1
        LabelSalida.Text = "Salida"
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Location = New Point(33, 78)
        Label3.Name = "Label3"
        Label3.Size = New Size(55, 15)
        Label3.TabIndex = 2
        Label3.Text = "Tipo Xml"
        ' 
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.Location = New Point(33, 103)
        Label4.Name = "Label4"
        Label4.Size = New Size(61, 15)
        Label4.TabIndex = 3
        Label4.Text = "Nodo Raiz"
        ' 
        ' TextRuta
        ' 
        TextRuta.Location = New Point(91, 12)
        TextRuta.Name = "TextRuta"
        TextRuta.Size = New Size(322, 23)
        TextRuta.TabIndex = 5
        ' 
        ' TextSalida
        ' 
        TextSalida.Location = New Point(91, 44)
        TextSalida.Name = "TextSalida"
        TextSalida.Size = New Size(322, 23)
        TextSalida.TabIndex = 6
        ' 
        ' LabelTipoNodo
        ' 
        LabelTipoNodo.AutoSize = True
        LabelTipoNodo.Location = New Point(92, 103)
        LabelTipoNodo.Name = "LabelTipoNodo"
        LabelTipoNodo.Size = New Size(0, 15)
        LabelTipoNodo.TabIndex = 7
        ' 
        ' ComboTipoXml
        ' 
        ComboTipoXml.FormattingEnabled = True
        ComboTipoXml.Location = New Point(90, 74)
        ComboTipoXml.Name = "ComboTipoXml"
        ComboTipoXml.Size = New Size(121, 23)
        ComboTipoXml.TabIndex = 8
        ' 
        ' BotonTrocear
        ' 
        BotonTrocear.BackColor = Color.FromArgb(CByte(25), CByte(118), CByte(210))
        BotonTrocear.FlatAppearance.BorderColor = Color.FromArgb(CByte(25), CByte(118), CByte(210))
        BotonTrocear.FlatStyle = FlatStyle.Popup
        BotonTrocear.ForeColor = SystemColors.Control
        BotonTrocear.Location = New Point(310, 127)
        BotonTrocear.Name = "BotonTrocear"
        BotonTrocear.Size = New Size(94, 23)
        BotonTrocear.TabIndex = 9
        BotonTrocear.Text = "Trocear XML"
        BotonTrocear.UseVisualStyleBackColor = False
        ' 
        ' TextoTamaño
        ' 
        TextoTamaño.Location = New Point(337, 75)
        TextoTamaño.Name = "TextoTamaño"
        TextoTamaño.Size = New Size(76, 23)
        TextoTamaño.TabIndex = 10
        TextoTamaño.Text = "9999"
        ' 
        ' LabelTamaño
        ' 
        LabelTamaño.AutoSize = True
        LabelTamaño.Location = New Point(255, 78)
        LabelTamaño.Name = "LabelTamaño"
        LabelTamaño.Size = New Size(78, 15)
        LabelTamaño.TabIndex = 11
        LabelTamaño.Text = "Tamaño Max."
        ' 
        ' CargarImagen
        ' 
        CargarImagen.Image = CType(resources.GetObject("CargarImagen.Image"), Image)
        CargarImagen.Location = New Point(33, 125)
        CargarImagen.Name = "CargarImagen"
        CargarImagen.Size = New Size(54, 36)
        CargarImagen.TabIndex = 44
        CargarImagen.TabStop = False
        CargarImagen.Visible = False
        ' 
        ' TextConsultando
        ' 
        TextConsultando.Location = New Point(90, 125)
        TextConsultando.MaximumSize = New Size(400, 0)
        TextConsultando.Name = "TextConsultando"
        TextConsultando.Size = New Size(214, 35)
        TextConsultando.TabIndex = 45
        TextConsultando.Text = "Troceando..."
        TextConsultando.Visible = False
        ' 
        ' TrocearXMLForm
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(213), CByte(220), CByte(227))
        ClientSize = New Size(416, 162)
        Controls.Add(TextConsultando)
        Controls.Add(CargarImagen)
        Controls.Add(LabelTamaño)
        Controls.Add(TextoTamaño)
        Controls.Add(BotonTrocear)
        Controls.Add(ComboTipoXml)
        Controls.Add(LabelTipoNodo)
        Controls.Add(TextSalida)
        Controls.Add(TextRuta)
        Controls.Add(Label4)
        Controls.Add(Label3)
        Controls.Add(LabelSalida)
        Controls.Add(LabelEntrada)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Name = "TrocearXMLForm"
        Text = "Trocear XML"
        CType(CargarImagen, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents LabelEntrada As Label
    Friend WithEvents LabelSalida As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents TextRuta As TextBox
    Friend WithEvents TextSalida As TextBox
    Friend WithEvents LabelTipoNodo As Label
    Friend WithEvents ComboTipoXml As ComboBox
    Friend WithEvents BotonTrocear As Button
    Friend WithEvents TextoTamaño As TextBox
    Friend WithEvents LabelTamaño As Label
    Friend WithEvents CargarImagen As PictureBox
    Friend WithEvents TextConsultando As Label
End Class
