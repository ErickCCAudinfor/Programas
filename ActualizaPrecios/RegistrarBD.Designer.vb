<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class RegistrarBD
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(RegistrarBD))
        NombreBox = New TextBox()
        ServidorBox = New TextBox()
        BDBox = New TextBox()
        UsuarioBox = New TextBox()
        NombreLabel = New Label()
        AgregarBDBoton = New Button()
        ServidorLabel = New Label()
        BDLabel = New Label()
        UsuarioLabel = New Label()
        ClaveLabel = New Label()
        ClaveBox = New TextBox()
        VPNCheck = New CheckBox()
        SuspendLayout()
        ' 
        ' NombreBox
        ' 
        NombreBox.Location = New Point(84, 22)
        NombreBox.Name = "NombreBox"
        NombreBox.Size = New Size(100, 23)
        NombreBox.TabIndex = 0
        ' 
        ' ServidorBox
        ' 
        ServidorBox.Location = New Point(84, 51)
        ServidorBox.Name = "ServidorBox"
        ServidorBox.Size = New Size(100, 23)
        ServidorBox.TabIndex = 1
        ' 
        ' BDBox
        ' 
        BDBox.Location = New Point(84, 80)
        BDBox.Name = "BDBox"
        BDBox.Size = New Size(100, 23)
        BDBox.TabIndex = 2
        ' 
        ' UsuarioBox
        ' 
        UsuarioBox.Location = New Point(84, 109)
        UsuarioBox.Name = "UsuarioBox"
        UsuarioBox.Size = New Size(100, 23)
        UsuarioBox.TabIndex = 3
        ' 
        ' NombreLabel
        ' 
        NombreLabel.AutoSize = True
        NombreLabel.Location = New Point(13, 29)
        NombreLabel.Name = "NombreLabel"
        NombreLabel.Size = New Size(51, 15)
        NombreLabel.TabIndex = 4
        NombreLabel.Text = "Nombre"
        ' 
        ' AgregarBDBoton
        ' 
        AgregarBDBoton.BackColor = Color.FromArgb(CByte(25), CByte(118), CByte(210))
        AgregarBDBoton.FlatAppearance.BorderColor = Color.FromArgb(CByte(25), CByte(118), CByte(210))
        AgregarBDBoton.FlatStyle = FlatStyle.Popup
        AgregarBDBoton.ForeColor = SystemColors.Control
        AgregarBDBoton.Location = New Point(190, 172)
        AgregarBDBoton.Name = "AgregarBDBoton"
        AgregarBDBoton.Size = New Size(87, 23)
        AgregarBDBoton.TabIndex = 5
        AgregarBDBoton.Text = "Agregar"
        AgregarBDBoton.UseVisualStyleBackColor = False
        ' 
        ' ServidorLabel
        ' 
        ServidorLabel.AutoSize = True
        ServidorLabel.Location = New Point(13, 59)
        ServidorLabel.Name = "ServidorLabel"
        ServidorLabel.Size = New Size(50, 15)
        ServidorLabel.TabIndex = 6
        ServidorLabel.Text = "Servidor"
        ' 
        ' BDLabel
        ' 
        BDLabel.AutoSize = True
        BDLabel.Location = New Point(13, 83)
        BDLabel.Name = "BDLabel"
        BDLabel.Size = New Size(64, 15)
        BDLabel.TabIndex = 7
        BDLabel.Text = "Base Datos"
        ' 
        ' UsuarioLabel
        ' 
        UsuarioLabel.AutoSize = True
        UsuarioLabel.Location = New Point(12, 112)
        UsuarioLabel.Name = "UsuarioLabel"
        UsuarioLabel.Size = New Size(47, 15)
        UsuarioLabel.TabIndex = 8
        UsuarioLabel.Text = "Usuario"
        ' 
        ' ClaveLabel
        ' 
        ClaveLabel.AutoSize = True
        ClaveLabel.Location = New Point(13, 146)
        ClaveLabel.Name = "ClaveLabel"
        ClaveLabel.Size = New Size(36, 15)
        ClaveLabel.TabIndex = 9
        ClaveLabel.Text = "Clave"
        ' 
        ' ClaveBox
        ' 
        ClaveBox.Location = New Point(84, 143)
        ClaveBox.Name = "ClaveBox"
        ClaveBox.Size = New Size(100, 23)
        ClaveBox.TabIndex = 10
        ' 
        ' VPNCheck
        ' 
        VPNCheck.AutoSize = True
        VPNCheck.Location = New Point(84, 172)
        VPNCheck.Name = "VPNCheck"
        VPNCheck.Size = New Size(50, 19)
        VPNCheck.TabIndex = 11
        VPNCheck.Text = "VPN"
        VPNCheck.UseVisualStyleBackColor = True
        ' 
        ' RegistrarBD
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(213), CByte(220), CByte(227))
        ClientSize = New Size(289, 210)
        Controls.Add(VPNCheck)
        Controls.Add(ClaveBox)
        Controls.Add(ClaveLabel)
        Controls.Add(UsuarioLabel)
        Controls.Add(BDLabel)
        Controls.Add(ServidorLabel)
        Controls.Add(AgregarBDBoton)
        Controls.Add(NombreLabel)
        Controls.Add(UsuarioBox)
        Controls.Add(BDBox)
        Controls.Add(ServidorBox)
        Controls.Add(NombreBox)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Name = "RegistrarBD"
        Text = "RegistrarBD"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents NombreBox As TextBox
    Friend WithEvents ServidorBox As TextBox
    Friend WithEvents BDBox As TextBox
    Friend WithEvents UsuarioBox As TextBox
    Friend WithEvents NombreLabel As Label
    Friend WithEvents AgregarBDBoton As Button
    Friend WithEvents ServidorLabel As Label
    Friend WithEvents BDLabel As Label
    Friend WithEvents UsuarioLabel As Label
    Friend WithEvents ClaveLabel As Label
    Friend WithEvents ClaveBox As TextBox
    Friend WithEvents VPNCheck As CheckBox
End Class
