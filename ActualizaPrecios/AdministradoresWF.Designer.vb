<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AdministradoresWF
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(AdministradoresWF))
        Label8 = New Label()
        Label7 = New Label()
        Label6 = New Label()
        Label5 = New Label()
        ComboBox2 = New ComboBox()
        Label4 = New Label()
        Label3 = New Label()
        Label2 = New Label()
        Button1 = New Button()
        ComboBox1 = New ComboBox()
        Label1 = New Label()
        Label9 = New Label()
        Label10 = New Label()
        SuspendLayout()
        ' 
        ' Label8
        ' 
        Label8.AutoSize = True
        Label8.Location = New Point(499, 64)
        Label8.Name = "Label8"
        Label8.Size = New Size(13, 15)
        Label8.TabIndex = 21
        Label8.Text = "0"
        ' 
        ' Label7
        ' 
        Label7.AutoSize = True
        Label7.Location = New Point(499, 25)
        Label7.Name = "Label7"
        Label7.Size = New Size(13, 15)
        Label7.TabIndex = 20
        Label7.Text = "0"
        ' 
        ' Label6
        ' 
        Label6.AutoSize = True
        Label6.Location = New Point(467, 64)
        Label6.Name = "Label6"
        Label6.Size = New Size(17, 15)
        Label6.TabIndex = 19
        Label6.Text = "Id"
        ' 
        ' Label5
        ' 
        Label5.AutoSize = True
        Label5.Location = New Point(467, 25)
        Label5.Name = "Label5"
        Label5.Size = New Size(17, 15)
        Label5.TabIndex = 18
        Label5.Text = "Id"
        ' 
        ' ComboBox2
        ' 
        ComboBox2.AutoCompleteMode = AutoCompleteMode.Suggest
        ComboBox2.AutoCompleteSource = AutoCompleteSource.ListItems
        ComboBox2.Enabled = False
        ComboBox2.FormattingEnabled = True
        ComboBox2.Location = New Point(141, 61)
        ComboBox2.Name = "ComboBox2"
        ComboBox2.Size = New Size(311, 23)
        ComboBox2.TabIndex = 17
        ' 
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.Location = New Point(43, 64)
        Label4.Name = "Label4"
        Label4.Size = New Size(65, 15)
        Label4.TabIndex = 16
        Label4.Text = "Admin Gas"
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Location = New Point(130, 102)
        Label3.Name = "Label3"
        Label3.Size = New Size(13, 15)
        Label3.TabIndex = 15
        Label3.Text = "0"
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(43, 102)
        Label2.Name = "Label2"
        Label2.Size = New Size(81, 15)
        Label2.TabIndex = 14
        Label2.Text = "Total Agentes:"
        ' 
        ' Button1
        ' 
        Button1.Location = New Point(437, 90)
        Button1.Name = "Button1"
        Button1.Size = New Size(75, 23)
        Button1.TabIndex = 13
        Button1.Text = "Actualizar"
        Button1.UseVisualStyleBackColor = True
        ' 
        ' ComboBox1
        ' 
        ComboBox1.AutoCompleteMode = AutoCompleteMode.Suggest
        ComboBox1.AutoCompleteSource = AutoCompleteSource.ListItems
        ComboBox1.FormattingEnabled = True
        ComboBox1.Location = New Point(141, 22)
        ComboBox1.Name = "ComboBox1"
        ComboBox1.Size = New Size(311, 23)
        ComboBox1.TabIndex = 12
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(43, 25)
        Label1.Name = "Label1"
        Label1.Size = New Size(64, 15)
        Label1.TabIndex = 11
        Label1.Text = "Admin Luz"
        ' 
        ' Label9
        ' 
        Label9.AutoSize = True
        Label9.Location = New Point(296, 102)
        Label9.Name = "Label9"
        Label9.Size = New Size(13, 15)
        Label9.TabIndex = 23
        Label9.Text = "0"
        ' 
        ' Label10
        ' 
        Label10.AutoSize = True
        Label10.Location = New Point(191, 102)
        Label10.Name = "Label10"
        Label10.Size = New Size(90, 15)
        Label10.TabIndex = 22
        Label10.Text = "Total Contratos:"
        ' 
        ' AdministradoresWF
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(527, 134)
        Controls.Add(Label9)
        Controls.Add(Label10)
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
        Name = "AdministradoresWF"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Administradores"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents Label8 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents ComboBox2 As ComboBox
    Friend WithEvents Label4 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Button1 As Button
    Friend WithEvents ComboBox1 As ComboBox
    Friend WithEvents Label1 As Label
    Friend WithEvents Label9 As Label
    Friend WithEvents Label10 As Label
End Class
