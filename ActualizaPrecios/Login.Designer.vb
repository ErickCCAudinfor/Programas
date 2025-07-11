<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Login
    Inherits System.Windows.Forms.Form

    'Form reemplaza a Dispose para limpiar la lista de componentes.
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

    'Requerido por el Diseñador de Windows Forms
    Private components As System.ComponentModel.IContainer

    'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
    'Se puede modificar usando el Diseñador de Windows Forms.  
    'No lo modifique con el editor de código.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Login))
        UsuarioBox = New TextBox()
        PasswordBox = New TextBox()
        LabelUsuario = New Label()
        LabelClave = New Label()
        Button1 = New Button()
        LabelGestorDatosSIGE = New Label()
        ExitPicture = New PictureBox()
        Label4 = New Label()
        Label5 = New Label()
        PictureBox1 = New PictureBox()
        Label1 = New Label()
        CType(ExitPicture, ComponentModel.ISupportInitialize).BeginInit()
        CType(PictureBox1, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' UsuarioBox
        ' 
        UsuarioBox.Location = New Point(209, 63)
        UsuarioBox.Name = "UsuarioBox"
        UsuarioBox.Size = New Size(203, 23)
        UsuarioBox.TabIndex = 0
        ' 
        ' PasswordBox
        ' 
        PasswordBox.Location = New Point(209, 104)
        PasswordBox.Name = "PasswordBox"
        PasswordBox.Size = New Size(203, 23)
        PasswordBox.TabIndex = 1
        ' 
        ' LabelUsuario
        ' 
        LabelUsuario.AutoSize = True
        LabelUsuario.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point)
        LabelUsuario.ForeColor = Color.FromArgb(CByte(233), CByte(231), CByte(226))
        LabelUsuario.Location = New Point(150, 66)
        LabelUsuario.Name = "LabelUsuario"
        LabelUsuario.Size = New Size(49, 15)
        LabelUsuario.TabIndex = 2
        LabelUsuario.Text = "Usuario"
        ' 
        ' LabelClave
        ' 
        LabelClave.AutoSize = True
        LabelClave.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point)
        LabelClave.ForeColor = Color.FromArgb(CByte(233), CByte(231), CByte(226))
        LabelClave.Location = New Point(150, 104)
        LabelClave.Name = "LabelClave"
        LabelClave.Size = New Size(37, 15)
        LabelClave.TabIndex = 3
        LabelClave.Text = "Clave"
        ' 
        ' Button1
        ' 
        Button1.BackColor = Color.FromArgb(CByte(233), CByte(231), CByte(226))
        Button1.FlatAppearance.BorderSize = 0
        Button1.FlatStyle = FlatStyle.Flat
        Button1.Location = New Point(209, 142)
        Button1.Name = "Button1"
        Button1.Size = New Size(203, 23)
        Button1.TabIndex = 4
        Button1.Text = "Entrar"
        Button1.UseVisualStyleBackColor = False
        ' 
        ' LabelGestorDatosSIGE
        ' 
        LabelGestorDatosSIGE.AutoSize = True
        LabelGestorDatosSIGE.Font = New Font("Segoe UI", 9.75F, FontStyle.Bold Or FontStyle.Italic, GraphicsUnit.Point)
        LabelGestorDatosSIGE.ForeColor = Color.FromArgb(CByte(233), CByte(231), CByte(226))
        LabelGestorDatosSIGE.Location = New Point(235, 20)
        LabelGestorDatosSIGE.Name = "LabelGestorDatosSIGE"
        LabelGestorDatosSIGE.Size = New Size(136, 17)
        LabelGestorDatosSIGE.TabIndex = 5
        LabelGestorDatosSIGE.Text = "GESTOR DATOS SIGE"
        LabelGestorDatosSIGE.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' ExitPicture
        ' 
        ExitPicture.Image = CType(resources.GetObject("ExitPicture.Image"), Image)
        ExitPicture.Location = New Point(459, 12)
        ExitPicture.Name = "ExitPicture"
        ExitPicture.Size = New Size(39, 34)
        ExitPicture.TabIndex = 6
        ExitPicture.TabStop = False
        ' 
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.Font = New Font("Segoe UI", 6F, FontStyle.Regular, GraphicsUnit.Point)
        Label4.ForeColor = Color.FromArgb(CByte(233), CByte(231), CByte(226))
        Label4.Location = New Point(398, 198)
        Label4.Name = "Label4"
        Label4.Size = New Size(42, 11)
        Label4.TabIndex = 7
        Label4.Text = "Copyright"
        ' 
        ' Label5
        ' 
        Label5.AutoSize = True
        Label5.Font = New Font("Segoe UI", 6F, FontStyle.Regular, GraphicsUnit.Point)
        Label5.ForeColor = Color.FromArgb(CByte(233), CByte(231), CByte(226))
        Label5.Location = New Point(438, 198)
        Label5.Name = "Label5"
        Label5.Size = New Size(60, 11)
        Label5.TabIndex = 8
        Label5.Text = "Erick Cupuerán"
        ' 
        ' PictureBox1
        ' 
        PictureBox1.Image = CType(resources.GetObject("PictureBox1.Image"), Image)
        PictureBox1.Location = New Point(39, 77)
        PictureBox1.Name = "PictureBox1"
        PictureBox1.Size = New Size(64, 72)
        PictureBox1.TabIndex = 9
        PictureBox1.TabStop = False
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Font = New Font("Segoe UI", 6F, FontStyle.Regular, GraphicsUnit.Point)
        Label1.ForeColor = Color.FromArgb(CByte(233), CByte(231), CByte(226))
        Label1.Location = New Point(7, 9)
        Label1.Name = "Label1"
        Label1.Size = New Size(43, 11)
        Label1.TabIndex = 10
        Label1.Text = "version 1.2"
        ' 
        ' Login
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(130), CByte(30), CByte(34))
        ClientSize = New Size(525, 219)
        Controls.Add(Label1)
        Controls.Add(PictureBox1)
        Controls.Add(Label5)
        Controls.Add(Label4)
        Controls.Add(ExitPicture)
        Controls.Add(LabelGestorDatosSIGE)
        Controls.Add(Button1)
        Controls.Add(LabelClave)
        Controls.Add(LabelUsuario)
        Controls.Add(PasswordBox)
        Controls.Add(UsuarioBox)
        FormBorderStyle = FormBorderStyle.None
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Name = "Login"
        Opacity = 0.95R
        StartPosition = FormStartPosition.CenterScreen
        Text = "Login"
        CType(ExitPicture, ComponentModel.ISupportInitialize).EndInit()
        CType(PictureBox1, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents UsuarioBox As TextBox
    Friend WithEvents PasswordBox As TextBox
    Friend WithEvents LabelUsuario As Label
    Friend WithEvents LabelClave As Label
    Friend WithEvents Button1 As Button
    Friend WithEvents LabelGestorDatosSIGE As Label
    Friend WithEvents ExitPicture As PictureBox
    Friend WithEvents Label4 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents Label1 As Label
End Class
