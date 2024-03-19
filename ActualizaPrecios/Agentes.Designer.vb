<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Agentes
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Agentes))
        Label1 = New Label()
        ComboBox1 = New ComboBox()
        Button1 = New Button()
        Label2 = New Label()
        Label3 = New Label()
        ComboBox2 = New ComboBox()
        Label4 = New Label()
        Label5 = New Label()
        Label6 = New Label()
        Label7 = New Label()
        Label8 = New Label()
        SuspendLayout()
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(27, 26)
        Label1.Name = "Label1"
        Label1.Size = New Size(66, 15)
        Label1.TabIndex = 0
        Label1.Text = "Agente Luz"
        ' 
        ' ComboBox1
        ' 
        ComboBox1.AutoCompleteMode = AutoCompleteMode.Suggest
        ComboBox1.AutoCompleteSource = AutoCompleteSource.ListItems
        ComboBox1.FormattingEnabled = True
        ComboBox1.Location = New Point(125, 23)
        ComboBox1.Name = "ComboBox1"
        ComboBox1.Size = New Size(311, 23)
        ComboBox1.TabIndex = 1
        ' 
        ' Button1
        ' 
        Button1.Location = New Point(421, 91)
        Button1.Name = "Button1"
        Button1.Size = New Size(75, 23)
        Button1.TabIndex = 2
        Button1.Text = "Actualizar"
        Button1.UseVisualStyleBackColor = True
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(27, 103)
        Label2.Name = "Label2"
        Label2.Size = New Size(81, 15)
        Label2.TabIndex = 3
        Label2.Text = "Total Agentes:"
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Location = New Point(114, 103)
        Label3.Name = "Label3"
        Label3.Size = New Size(13, 15)
        Label3.TabIndex = 4
        Label3.Text = "0"
        ' 
        ' ComboBox2
        ' 
        ComboBox2.AutoCompleteMode = AutoCompleteMode.Suggest
        ComboBox2.AutoCompleteSource = AutoCompleteSource.ListItems
        ComboBox2.Enabled = False
        ComboBox2.FormattingEnabled = True
        ComboBox2.Location = New Point(125, 62)
        ComboBox2.Name = "ComboBox2"
        ComboBox2.Size = New Size(311, 23)
        ComboBox2.TabIndex = 6
        ' 
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.Location = New Point(27, 65)
        Label4.Name = "Label4"
        Label4.Size = New Size(67, 15)
        Label4.TabIndex = 5
        Label4.Text = "Agente Gas"
        ' 
        ' Label5
        ' 
        Label5.AutoSize = True
        Label5.Location = New Point(451, 26)
        Label5.Name = "Label5"
        Label5.Size = New Size(17, 15)
        Label5.TabIndex = 7
        Label5.Text = "Id"
        ' 
        ' Label6
        ' 
        Label6.AutoSize = True
        Label6.Location = New Point(451, 65)
        Label6.Name = "Label6"
        Label6.Size = New Size(17, 15)
        Label6.TabIndex = 8
        Label6.Text = "Id"
        ' 
        ' Label7
        ' 
        Label7.AutoSize = True
        Label7.Location = New Point(483, 26)
        Label7.Name = "Label7"
        Label7.Size = New Size(13, 15)
        Label7.TabIndex = 9
        Label7.Text = "0"
        ' 
        ' Label8
        ' 
        Label8.AutoSize = True
        Label8.Location = New Point(483, 65)
        Label8.Name = "Label8"
        Label8.Size = New Size(13, 15)
        Label8.TabIndex = 10
        Label8.Text = "0"
        ' 
        ' Agentes
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(536, 126)
        Controls.Add(Label8)
        Controls.Add(Label7)
        Controls.Add(Label6)
        Controls.Add(Label5)
        Controls.Add(ComboBox2)
        Controls.Add(Label4)
        Controls.Add(Label3)
        Controls.Add(Label2)
        Controls.Add(Button1)
        Controls.Add(ComboBox1)
        Controls.Add(Label1)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Name = "Agentes"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Agentes"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents ComboBox1 As ComboBox
    Friend WithEvents Button1 As Button
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents ComboBox2 As ComboBox
    Friend WithEvents Label4 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents Label8 As Label
End Class
