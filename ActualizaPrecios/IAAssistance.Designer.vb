<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class IAAssistance
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(IAAssistance))
        txtPregunta = New TextBox()
        btnPreguntar = New Button()
        txtRespuesta = New TextBox()
        Label1 = New Label()
        Label2 = New Label()
        PictureBox1 = New PictureBox()
        CType(PictureBox1, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' txtPregunta
        ' 
        txtPregunta.Location = New Point(122, 12)
        txtPregunta.Multiline = True
        txtPregunta.Name = "txtPregunta"
        txtPregunta.PlaceholderText = "Pregunta algo a JADE"
        txtPregunta.Size = New Size(388, 76)
        txtPregunta.TabIndex = 0
        ' 
        ' btnPreguntar
        ' 
        btnPreguntar.Location = New Point(122, 380)
        btnPreguntar.Name = "btnPreguntar"
        btnPreguntar.Size = New Size(388, 42)
        btnPreguntar.TabIndex = 1
        btnPreguntar.Text = "Preguntar"
        btnPreguntar.UseVisualStyleBackColor = True
        ' 
        ' txtRespuesta
        ' 
        txtRespuesta.Enabled = False
        txtRespuesta.Font = New Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point)
        txtRespuesta.Location = New Point(122, 94)
        txtRespuesta.Multiline = True
        txtRespuesta.Name = "txtRespuesta"
        txtRespuesta.PlaceholderText = "🤖 Hola, soy Jade, tu asistente inteligente"
        txtRespuesta.Size = New Size(388, 280)
        txtRespuesta.TabIndex = 2
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(22, 15)
        Label1.Name = "Label1"
        Label1.Size = New Size(59, 15)
        Label1.TabIndex = 3
        Label1.Text = "Preguntar"
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(22, 94)
        Label2.Name = "Label2"
        Label2.Size = New Size(33, 15)
        Label2.TabIndex = 4
        Label2.Text = "JADE"
        ' 
        ' PictureBox1
        ' 
        PictureBox1.Image = CType(resources.GetObject("PictureBox1.Image"), Image)
        PictureBox1.InitialImage = Nothing
        PictureBox1.Location = New Point(22, 355)
        PictureBox1.Name = "PictureBox1"
        PictureBox1.Size = New Size(94, 67)
        PictureBox1.TabIndex = 5
        PictureBox1.TabStop = False
        ' 
        ' IAAssistance
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = SystemColors.GradientInactiveCaption
        ClientSize = New Size(540, 434)
        Controls.Add(PictureBox1)
        Controls.Add(Label2)
        Controls.Add(Label1)
        Controls.Add(txtRespuesta)
        Controls.Add(btnPreguntar)
        Controls.Add(txtPregunta)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Name = "IAAssistance"
        Text = "IA JADE asistente"
        CType(PictureBox1, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents txtPregunta As TextBox
    Friend WithEvents btnPreguntar As Button
    Friend WithEvents txtRespuesta As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents PictureBox1 As PictureBox
End Class
