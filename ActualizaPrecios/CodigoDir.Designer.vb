<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class CodigoDir
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(CodigoDir))
        Button1 = New Button()
        Label1 = New Label()
        Label2 = New Label()
        Label3 = New Label()
        TextBox1 = New TextBox()
        TextBox2 = New TextBox()
        TextBox3 = New TextBox()
        SuspendLayout()
        ' 
        ' Button1
        ' 
        Button1.BackColor = Color.FromArgb(CByte(25), CByte(118), CByte(210))
        Button1.FlatStyle = FlatStyle.Popup
        Button1.ForeColor = Color.White
        Button1.Location = New Point(170, 124)
        Button1.Name = "Button1"
        Button1.Size = New Size(171, 23)
        Button1.TabIndex = 0
        Button1.Text = "Actualizar"
        Button1.UseVisualStyleBackColor = False
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(11, 32)
        Label1.Name = "Label1"
        Label1.Size = New Size(154, 15)
        Label1.TabIndex = 1
        Label1.Text = "Codigo Unidad Tramitadora"
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(11, 68)
        Label2.Name = "Label2"
        Label2.Size = New Size(138, 15)
        Label2.TabIndex = 2
        Label2.Text = "Codigo Oficina Contable"
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Location = New Point(11, 98)
        Label3.Name = "Label3"
        Label3.Size = New Size(126, 15)
        Label3.TabIndex = 3
        Label3.Text = "Codigo Organo Gestor"
        ' 
        ' TextBox1
        ' 
        TextBox1.Location = New Point(170, 29)
        TextBox1.Name = "TextBox1"
        TextBox1.Size = New Size(171, 23)
        TextBox1.TabIndex = 4
        ' 
        ' TextBox2
        ' 
        TextBox2.Location = New Point(170, 65)
        TextBox2.Name = "TextBox2"
        TextBox2.Size = New Size(171, 23)
        TextBox2.TabIndex = 5
        ' 
        ' TextBox3
        ' 
        TextBox3.Location = New Point(170, 95)
        TextBox3.Name = "TextBox3"
        TextBox3.Size = New Size(171, 23)
        TextBox3.TabIndex = 6
        ' 
        ' CodigoDir
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(358, 161)
        Controls.Add(TextBox3)
        Controls.Add(TextBox2)
        Controls.Add(TextBox1)
        Controls.Add(Label3)
        Controls.Add(Label2)
        Controls.Add(Label1)
        Controls.Add(Button1)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Name = "CodigoDir"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Codigos Dir"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents Button1 As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents TextBox1 As TextBox
    Friend WithEvents TextBox2 As TextBox
    Friend WithEvents TextBox3 As TextBox
End Class
