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
        LabelGestorDatosSIGE = New Label()
        ExitPicture = New PictureBox()
        Label4 = New Label()
        Label5 = New Label()
        PictureBox1 = New PictureBox()
        Label1 = New Label()
        ValidacionLabel = New Label()
        pblBorde = New Panel()
        pnlContenido = New Panel()
        Clavelbl = New Label()
        UsuarioLbl = New Label()
        TextoValidar = New Label()
        UsuarioBox = New TextBox()
        PasswordBox = New TextBox()
        Panel2 = New Panel()
        Button1 = New Button()
        Panel1 = New Panel()
        Panel6 = New Panel()
        Panel7 = New Panel()
        Panel8 = New Panel()
        TextBox5 = New TextBox()
        TextBox6 = New TextBox()
        Panel11 = New Panel()
        Button4 = New Button()
        Panel12 = New Panel()
        LabelSesion = New Label()
        CType(ExitPicture, ComponentModel.ISupportInitialize).BeginInit()
        CType(PictureBox1, ComponentModel.ISupportInitialize).BeginInit()
        pblBorde.SuspendLayout()
        pnlContenido.SuspendLayout()
        Panel6.SuspendLayout()
        Panel7.SuspendLayout()
        Panel8.SuspendLayout()
        SuspendLayout()
        ' 
        ' LabelGestorDatosSIGE
        ' 
        LabelGestorDatosSIGE.AutoSize = True
        LabelGestorDatosSIGE.BackColor = Color.Transparent
        LabelGestorDatosSIGE.Font = New Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point)
        LabelGestorDatosSIGE.ForeColor = Color.White
        LabelGestorDatosSIGE.Location = New Point(15, 139)
        LabelGestorDatosSIGE.Name = "LabelGestorDatosSIGE"
        LabelGestorDatosSIGE.Size = New Size(135, 17)
        LabelGestorDatosSIGE.TabIndex = 5
        LabelGestorDatosSIGE.Text = "GESTOR DATOS SIGE"
        LabelGestorDatosSIGE.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' ExitPicture
        ' 
        ExitPicture.BackColor = Color.Transparent
        ExitPicture.Image = CType(resources.GetObject("ExitPicture.Image"), Image)
        ExitPicture.Location = New Point(413, 9)
        ExitPicture.Name = "ExitPicture"
        ExitPicture.Size = New Size(39, 34)
        ExitPicture.TabIndex = 6
        ExitPicture.TabStop = False
        ' 
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.BackColor = Color.Transparent
        Label4.Font = New Font("Segoe UI", 6.0F, FontStyle.Regular, GraphicsUnit.Point)
        Label4.ForeColor = Color.Black
        Label4.Location = New Point(323, 252)
        Label4.Name = "Label4"
        Label4.Size = New Size(42, 11)
        Label4.TabIndex = 7
        Label4.Text = "Copyright"
        ' 
        ' Label5
        ' 
        Label5.AutoSize = True
        Label5.BackColor = Color.Transparent
        Label5.Font = New Font("Segoe UI", 6.0F, FontStyle.Regular, GraphicsUnit.Point)
        Label5.ForeColor = Color.Black
        Label5.Location = New Point(363, 252)
        Label5.Name = "Label5"
        Label5.Size = New Size(60, 11)
        Label5.TabIndex = 8
        Label5.Text = "Erick Cupuerán"
        ' 
        ' PictureBox1
        ' 
        PictureBox1.BackColor = Color.Transparent
        PictureBox1.Image = CType(resources.GetObject("PictureBox1.Image"), Image)
        PictureBox1.Location = New Point(45, 53)
        PictureBox1.Name = "PictureBox1"
        PictureBox1.Size = New Size(76, 73)
        PictureBox1.TabIndex = 9
        PictureBox1.TabStop = False
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.BackColor = Color.Transparent
        Label1.Font = New Font("Segoe UI", 6.0F, FontStyle.Regular, GraphicsUnit.Point)
        Label1.ForeColor = Color.White
        Label1.Location = New Point(56, 163)
        Label1.Name = "Label1"
        Label1.Size = New Size(43, 11)
        Label1.TabIndex = 10
        Label1.Text = "version 2.0"
        ' 
        ' ValidacionLabel
        ' 
        ValidacionLabel.AutoSize = True
        ValidacionLabel.BackColor = Color.Transparent
        ValidacionLabel.Font = New Font("Segoe UI", 9.0F, FontStyle.Regular, GraphicsUnit.Point)
        ValidacionLabel.ForeColor = Color.Black
        ValidacionLabel.Location = New Point(165, 169)
        ValidacionLabel.Name = "ValidacionLabel"
        ValidacionLabel.Size = New Size(33, 15)
        ValidacionLabel.TabIndex = 13
        ValidacionLabel.Text = "texto"
        ' 
        ' pblBorde
        ' 
        pblBorde.BackColor = Color.FromArgb(CByte(218), CByte(220), CByte(224))
        pblBorde.Controls.Add(pnlContenido)
        pblBorde.Location = New Point(186, 69)
        pblBorde.Name = "pblBorde"
        pblBorde.Padding = New Padding(1)
        pblBorde.Size = New Size(237, 143)
        pblBorde.TabIndex = 52
        ' 
        ' pnlContenido
        ' 
        pnlContenido.BackColor = Color.White
        pnlContenido.Controls.Add(Clavelbl)
        pnlContenido.Controls.Add(UsuarioLbl)
        pnlContenido.Controls.Add(TextoValidar)
        pnlContenido.Controls.Add(UsuarioBox)
        pnlContenido.Controls.Add(PasswordBox)
        pnlContenido.Controls.Add(Panel2)
        pnlContenido.Controls.Add(Button1)
        pnlContenido.Controls.Add(Panel1)
        pnlContenido.Dock = DockStyle.Fill
        pnlContenido.Location = New Point(1, 1)
        pnlContenido.Name = "pnlContenido"
        pnlContenido.Size = New Size(235, 141)
        pnlContenido.TabIndex = 0
        ' 
        ' Clavelbl
        ' 
        Clavelbl.AutoSize = True
        Clavelbl.BackColor = Color.Transparent
        Clavelbl.Font = New Font("Segoe UI", 6.0F, FontStyle.Regular, GraphicsUnit.Point)
        Clavelbl.ForeColor = Color.Black
        Clavelbl.Location = New Point(15, 49)
        Clavelbl.Name = "Clavelbl"
        Clavelbl.Size = New Size(24, 11)
        Clavelbl.TabIndex = 55
        Clavelbl.Text = "Clave"
        ' 
        ' UsuarioLbl
        ' 
        UsuarioLbl.AutoSize = True
        UsuarioLbl.BackColor = Color.Transparent
        UsuarioLbl.Font = New Font("Segoe UI", 6.0F, FontStyle.Regular, GraphicsUnit.Point)
        UsuarioLbl.ForeColor = Color.Black
        UsuarioLbl.Location = New Point(15, 8)
        UsuarioLbl.Name = "UsuarioLbl"
        UsuarioLbl.Size = New Size(33, 11)
        UsuarioLbl.TabIndex = 54
        UsuarioLbl.Text = "Usuario"
        ' 
        ' TextoValidar
        ' 
        TextoValidar.AutoSize = True
        TextoValidar.BackColor = Color.Transparent
        TextoValidar.Font = New Font("Segoe UI", 6.0F, FontStyle.Regular, GraphicsUnit.Point)
        TextoValidar.ForeColor = Color.Black
        TextoValidar.Location = New Point(76, 124)
        TextoValidar.Name = "TextoValidar"
        TextoValidar.Size = New Size(0, 11)
        TextoValidar.TabIndex = 54
        ' 
        ' UsuarioBox
        ' 
        UsuarioBox.BackColor = Color.White
        UsuarioBox.BorderStyle = BorderStyle.FixedSingle
        UsuarioBox.ForeColor = Color.Black
        UsuarioBox.Location = New Point(15, 22)
        UsuarioBox.Name = "UsuarioBox"
        UsuarioBox.PlaceholderText = "   usuario..."
        UsuarioBox.Size = New Size(203, 23)
        UsuarioBox.TabIndex = 0
        ' 
        ' PasswordBox
        ' 
        PasswordBox.BackColor = Color.White
        PasswordBox.BorderStyle = BorderStyle.FixedSingle
        PasswordBox.ForeColor = Color.Black
        PasswordBox.Location = New Point(15, 63)
        PasswordBox.Name = "PasswordBox"
        PasswordBox.PlaceholderText = "   clave..."
        PasswordBox.Size = New Size(203, 23)
        PasswordBox.TabIndex = 1
        ' 
        ' Panel2
        ' 
        Panel2.BackColor = Color.White
        Panel2.Location = New Point(15, 75)
        Panel2.Name = "Panel2"
        Panel2.Size = New Size(203, 2)
        Panel2.TabIndex = 12
        ' 
        ' Button1
        ' 
        Button1.BackColor = Color.FromArgb(CByte(25), CByte(118), CByte(210))
        Button1.FlatAppearance.BorderSize = 0
        Button1.FlatStyle = FlatStyle.Flat
        Button1.ForeColor = Color.White
        Button1.Location = New Point(15, 97)
        Button1.Name = "Button1"
        Button1.Size = New Size(203, 23)
        Button1.TabIndex = 4
        Button1.Text = "Entrar"
        Button1.UseVisualStyleBackColor = False
        ' 
        ' Panel1
        ' 
        Panel1.BackColor = Color.White
        Panel1.Location = New Point(15, 38)
        Panel1.Name = "Panel1"
        Panel1.Size = New Size(203, 2)
        Panel1.TabIndex = 11
        ' 
        ' Panel6
        ' 
        Panel6.BackColor = Color.FromArgb(CByte(218), CByte(220), CByte(224))
        Panel6.Controls.Add(Panel7)
        Panel6.Location = New Point(-2, -1)
        Panel6.Name = "Panel6"
        Panel6.Padding = New Padding(1)
        Panel6.Size = New Size(160, 310)
        Panel6.TabIndex = 53
        ' 
        ' Panel7
        ' 
        Panel7.BackColor = Color.White
        Panel7.Controls.Add(Panel8)
        Panel7.Controls.Add(TextBox5)
        Panel7.Controls.Add(TextBox6)
        Panel7.Controls.Add(Panel11)
        Panel7.Controls.Add(Button4)
        Panel7.Controls.Add(Panel12)
        Panel7.Dock = DockStyle.Fill
        Panel7.Location = New Point(1, 1)
        Panel7.Name = "Panel7"
        Panel7.Size = New Size(158, 308)
        Panel7.TabIndex = 0
        ' 
        ' Panel8
        ' 
        Panel8.BackColor = Color.White
        Panel8.BackgroundImage = CType(resources.GetObject("Panel8.BackgroundImage"), Image)
        Panel8.Controls.Add(PictureBox1)
        Panel8.Controls.Add(Label1)
        Panel8.Controls.Add(ValidacionLabel)
        Panel8.Controls.Add(LabelGestorDatosSIGE)
        Panel8.Dock = DockStyle.Fill
        Panel8.Location = New Point(0, 0)
        Panel8.Name = "Panel8"
        Panel8.Size = New Size(158, 308)
        Panel8.TabIndex = 13
        ' 
        ' TextBox5
        ' 
        TextBox5.BackColor = Color.White
        TextBox5.BorderStyle = BorderStyle.None
        TextBox5.ForeColor = Color.Black
        TextBox5.Location = New Point(15, 22)
        TextBox5.Name = "TextBox5"
        TextBox5.PlaceholderText = "usuario..."
        TextBox5.Size = New Size(203, 16)
        TextBox5.TabIndex = 0
        ' 
        ' TextBox6
        ' 
        TextBox6.BackColor = Color.White
        TextBox6.BorderStyle = BorderStyle.None
        TextBox6.ForeColor = Color.Black
        TextBox6.Location = New Point(15, 59)
        TextBox6.Name = "TextBox6"
        TextBox6.PlaceholderText = "clave..."
        TextBox6.Size = New Size(203, 16)
        TextBox6.TabIndex = 1
        ' 
        ' Panel11
        ' 
        Panel11.BackColor = Color.White
        Panel11.Location = New Point(15, 75)
        Panel11.Name = "Panel11"
        Panel11.Size = New Size(203, 2)
        Panel11.TabIndex = 12
        ' 
        ' Button4
        ' 
        Button4.BackColor = Color.FromArgb(CByte(25), CByte(118), CByte(210))
        Button4.FlatAppearance.BorderSize = 0
        Button4.FlatStyle = FlatStyle.Flat
        Button4.ForeColor = Color.White
        Button4.Location = New Point(15, 97)
        Button4.Name = "Button4"
        Button4.Size = New Size(203, 23)
        Button4.TabIndex = 4
        Button4.Text = "Entrar"
        Button4.UseVisualStyleBackColor = False
        ' 
        ' Panel12
        ' 
        Panel12.BackColor = Color.White
        Panel12.Location = New Point(15, 38)
        Panel12.Name = "Panel12"
        Panel12.Size = New Size(203, 2)
        Panel12.TabIndex = 11
        ' 
        ' LabelSesion
        ' 
        LabelSesion.AutoSize = True
        LabelSesion.BackColor = Color.Transparent
        LabelSesion.Font = New Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point)
        LabelSesion.ForeColor = Color.Black
        LabelSesion.Location = New Point(186, 37)
        LabelSesion.Name = "LabelSesion"
        LabelSesion.Size = New Size(89, 17)
        LabelSesion.TabIndex = 14
        LabelSesion.Text = "Iniciar sesión"
        LabelSesion.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' Login
        ' 
        AutoScaleDimensions = New SizeF(7.0F, 15.0F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(244), CByte(246), CByte(248))
        ClientSize = New Size(459, 270)
        Controls.Add(LabelSesion)
        Controls.Add(Panel6)
        Controls.Add(pblBorde)
        Controls.Add(Label5)
        Controls.Add(Label4)
        Controls.Add(ExitPicture)
        FormBorderStyle = FormBorderStyle.None
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Name = "Login"
        Opacity = 0.97R
        StartPosition = FormStartPosition.CenterScreen
        Text = "Login"
        CType(ExitPicture, ComponentModel.ISupportInitialize).EndInit()
        CType(PictureBox1, ComponentModel.ISupportInitialize).EndInit()
        pblBorde.ResumeLayout(False)
        pnlContenido.ResumeLayout(False)
        pnlContenido.PerformLayout()
        Panel6.ResumeLayout(False)
        Panel7.ResumeLayout(False)
        Panel7.PerformLayout()
        Panel8.ResumeLayout(False)
        Panel8.PerformLayout()
        ResumeLayout(False)
        PerformLayout()
    End Sub
    Friend WithEvents LabelGestorDatosSIGE As Label
    Friend WithEvents ExitPicture As PictureBox
    Friend WithEvents Label4 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents Label1 As Label
    Friend WithEvents ValidacionLabel As Label
    Friend WithEvents pblBorde As Panel
    Friend WithEvents Panel6 As Panel
    Friend WithEvents Panel7 As Panel
    Friend WithEvents Panel8 As Panel
    Friend WithEvents TextBox5 As TextBox
    Friend WithEvents TextBox6 As TextBox
    Friend WithEvents Panel11 As Panel
    Friend WithEvents Button4 As Button
    Friend WithEvents Panel12 As Panel
    Friend WithEvents LabelSesion As Label
    Friend WithEvents pnlContenido As Panel
    Friend WithEvents UsuarioBox As TextBox
    Friend WithEvents PasswordBox As TextBox
    Friend WithEvents Panel2 As Panel
    Friend WithEvents Button1 As Button
    Friend WithEvents Panel1 As Panel
    Friend WithEvents TextoValidar As Label
    Friend WithEvents UsuarioLbl As Label
    Friend WithEvents Clavelbl As Label
End Class
